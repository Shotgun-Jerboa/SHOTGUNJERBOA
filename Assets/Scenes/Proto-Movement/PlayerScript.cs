using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public string state;
    private bool sprinting = false;

    public SettingVars settings;
    private Dictionary<string, GameObject> gameObjects;
    public LayerMask groundLayer;

    public Text HealthCounter;
    public Text AmmoCounter;

    private Rigidbody physbody;
    private float height;
    public bool onGround = false;

    private List<interpolate> interpolates;
    private AnimationCurve movementMagnitude;
    private float jumpMagnitudeMultiplier = 0;

    public class interpolate
    {
        public AnimationCurve curve;
        public float elapsedTime;
        public int iteration = 0;

        private System.Action<interpolate> func;

        public interpolate(AnimationCurve curve, System.Action<interpolate> func, float time=0f)
        {
            if(curve.length < 2)
            {
                throw new System.Exception("Not enough keyframes provided for curve");
            }
            this.curve = curve;
            this.elapsedTime = time;
            this.func = func;
        }

        public bool iterate()
        {
            func(this);
            iteration++;
            return elapsedTime >= curve[curve.length - 1].time;
        }
    }

    // Debug Feature
    // private float maxHeight = 0;

    
    public int Ammo = 3;
    public int Health = 100;

    void Start()
    {
        interpolates = new();

        movementMagnitude = new AnimationCurve(new Keyframe[] {
            new Keyframe()
            {
                time = 0,
                value = 0,
                inWeight = 0,
                outWeight = 0,
                inTangent = 0,
                outTangent = 0
            },
            new Keyframe()
            {
                time = 1f,
                value = 1,
                inWeight = 0,
                outWeight = 0,
                inTangent = 2,
                outTangent = 0
            }
        });

        physbody = gameObject.GetComponent<Rigidbody>();
        gameObjects = new();
        for(int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            gameObjects[child.name] = child.gameObject;
        }

        height = gameObjects["Collision"].GetComponent<CapsuleCollider>().height;
    }

    private void Update()
    {
        sprinting = settings.input.Gameplay.Sprint.IsPressed();

        if (Input.GetKeyDown("z"))
        {
            Health = Health + 10;
            HealthCounter.text = "Health: " + Health;
            Ammo--;
            AmmoCounter.text = "Ammo: " + Ammo;
        }
    }

    private void FixedUpdate()
    {
        // Debug Snippet
        // if (gameObject.transform.position.y > maxHeight)
        // {
        //    maxHeight = gameObject.transform.position.y;
        //    print(maxHeight);
        // }

        onGround = Physics.Raycast(transform.position, Vector3.down, (height * 0.5f) + 0.01f, groundLayer);
        if (onGround)
        {
            state = "Gameplay_Ground";
            physbody.drag = settings.worldVars.groundDrag;
        } else
        {
            physbody.drag = 0;
        }

        if (settings.input.Gameplay.Jump.IsPressed() && onGround)
        {
            physbody.drag = 0;
            physbody.velocity = new Vector3(physbody.velocity.x, 0, physbody.velocity.z);
            jump(settings.worldVars.jumpHeight, settings.worldVars.jumpTimeToPeak, "quad", 3f);
            state = "Gameplay_Air";
        }

        Vector2 moveDir = settings.input.Gameplay.Move.ReadValue<Vector2>();
        if (moveDir != Vector2.zero)
        {
            switch (state)
            {
                case "Gameplay_Air":
                    physbody.velocity = gameObjects["Orientation"].GetComponent<Transform>().rotation * new Vector3(
                        moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.airTimeMultiplier,
                        physbody.velocity.y,
                        moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.airTimeMultiplier
                    );
                    break;
                case "Gameplay_Ground":
                    if (onGround)
                    {
                        if (sprinting)
                        {
                            physbody.velocity = gameObjects["Orientation"].GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier
                            );
                        }
                        else
                        {
                            physbody.velocity = gameObjects["Orientation"].GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed
                            );
                        }
                        physbody.velocity = new Vector3(physbody.velocity.x, 0, physbody.velocity.z);
                        jumpMagnitudeMultiplier = movementMagnitude.Evaluate(moveDir.magnitude);
                        physbody.drag = 0;
                        if (sprinting)
                        {
                            jump(settings.worldVars.sprintHopHeight, settings.worldVars.sprintTimeToPeak, "log", 0f);
                        }
                        else
                        {
                            jump(settings.worldVars.hopHeight, settings.worldVars.hopTimeToPeak, "log", 0f);
                        }
                    } else
                    {
                        if (sprinting)
                        {
                            physbody.velocity = gameObjects["Orientation"].GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier * settings.worldVars.midHopMultiplier,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier * settings.worldVars.midHopMultiplier
                            );
                        } else
                        {
                            physbody.velocity = gameObjects["Orientation"].GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.midHopMultiplier,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.midHopMultiplier
                            );
                        }
                    }
                    break;
            }
        }

        for(int i = interpolates.Count - 1; i >= 0; i--)
        {
            if (interpolates[i].iterate())
            {
                interpolates.RemoveAt(i);
            }
        }
    }

    public void jump(float height, float time, string curveType="linear", float strMultiplier=0f)
    {
        if(strMultiplier < 0)
        {
            strMultiplier = 0f;
        } else if (strMultiplier > 1)
        {
            strMultiplier = 1f;
        }

        if(time <= 0)
        {
            print("Failed to jump. Time must be greater than 0");
            return;
        }

        float multiplier = 1f;

        if(state == "Gameplay_Ground")
        {
            multiplier = jumpMagnitudeMultiplier;
        } 

        System.Action<interpolate> func = (parent) =>
        {
            if (parent.iteration == 0)
            {
                Vector3 velocity = new Vector3(
                    0, 
                    (parent.curve.Evaluate(parent.elapsedTime) * multiplier) + (-Physics.gravity.y * Time.fixedDeltaTime),
                    0
                );
                physbody.velocity += velocity;
            }
            else
            {
                Vector3 velocityOld = new Vector3(0, parent.curve.Evaluate(parent.elapsedTime) * multiplier, 0);
                physbody.velocity -= velocityOld;
                parent.elapsedTime += Time.fixedDeltaTime;
                Vector3 velocityNew = new Vector3(
                    0, 
                    (parent.curve.Evaluate(parent.elapsedTime) * multiplier) + (-Physics.gravity.y * Time.fixedDeltaTime), 
                    0
                );
                physbody.velocity += velocityNew;
            }
        };

        float m0;
        float m1;
        float p0;

        switch (curveType)
        {
            case "linear":
                interpolates.Add(new interpolate(
                    new AnimationCurve(new Keyframe[] { new Keyframe() {
                            time = 0f,
                            value = 2f * height / time,
                            inWeight = 0f,
                            outWeight = 0f,
                            inTangent = 0f,
                            outTangent = -1f
                        }, new Keyframe(){
                            time = time,
                            value = 0f,
                            inWeight = 0f,
                            outWeight = 0f,
                            inTangent = -1f,
                            outTangent = 0f
                        }
                    }), func
                ));
                break;
            case "quad":
                m0 = -2f - (1f * strMultiplier);
                p0 = -1f * (((-12f * height) + (m0 * time)) / (6f * time));

                interpolates.Add(new interpolate(
                    new AnimationCurve(new Keyframe[] { new Keyframe() {
                            time = 0f,
                            value = p0,
                            inWeight = 0f,
                            outWeight = 0f,
                            inTangent = 0f,
                            outTangent = m0
                        }, new Keyframe(){
                            time = time,
                            value = 0f,
                            inWeight = 0f,
                            outWeight = 0f,
                            inTangent = 0f,
                            outTangent = 0f
                        }
                    }), func
                ));
                break;
            case "log":
                m1 = -2f - (1f * strMultiplier);
                p0 = -1f * (((-12f * height) - (m1 * time)) / (6f * time));

                interpolates.Add(new interpolate(
                    new AnimationCurve(new Keyframe[] { new Keyframe() {
                            time = 0f,
                            value = p0,
                            inWeight = 0f,
                            outWeight = 0f,
                            inTangent = 0f,
                            outTangent = 0f
                        }, new Keyframe(){
                            time = time,
                            value = 0f,
                            inWeight = 0f,
                            outWeight = 0f,
                            inTangent = m1,
                            outTangent = 0f
                        }
                    }), func
                ));
                break;
        }
    }
}
