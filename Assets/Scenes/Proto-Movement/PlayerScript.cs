using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public enum PlayerState
    {
        Gameplay_Ground,
        Gameplay_Air
    }
    public PlayerState state = PlayerState.Gameplay_Ground;

    public SettingVars settings;
    private Global.SceneHeirarchy children;
    public LayerMask groundLayer;
    public Rigidbody physbody;

    private float hitboxHeight;
    public bool onGround = false;
    private bool sprinting = false;
    private float jumpDelay = 0f;

    private AnimationCurve movementMagnitude;
    private float jumpMagnitudeMultiplier = 0;
    private Vector2 moveDir;

    // Debug Feature
    // private float maxHeight = 0;

    void Start()
    {
        physbody = gameObject.GetComponent<Rigidbody>();
        children = new(gameObject);
        settings = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();
        hitboxHeight = children.Get("Collision").GetComponent<CapsuleCollider>().height;

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
    }

    private void Update()
    {
        sprinting = settings.input.Gameplay.Sprint.IsPressed();
    }

    private void FixedUpdate()
    {
        // Debug Snippet
        // if (gameObject.transform.position.y > maxHeight)
        // {
        //    maxHeight = gameObject.transform.position.y;
        //    print(maxHeight);
        // }

        onGround = Physics.Raycast(transform.position, Vector3.down, (hitboxHeight * 0.5f) + 0.01f, groundLayer);
        if (onGround)
        {
            state = PlayerState.Gameplay_Ground;
            physbody.drag = settings.worldVars.groundDrag;
        }
        else
        {
            physbody.drag = 0;
        }

        if ((settings.input.Gameplay.Jump.IsPressed() && state == PlayerState.Gameplay_Ground) || jumpDelay > 0f)
        {
            float dist;
            RaycastHit hit;
            Ray downRay = new(transform.position, Vector3.down);
            if(Physics.Raycast(downRay, out hit)){
                dist = hit.distance;
            } else {
                dist = settings.worldVars.jumpGroundThreshold + 1f;
            }

            if(onGround){
                jumpDelay = 0;
                physbody.drag = 0;
                physbody.velocity = new Vector3(physbody.velocity.x, 0, physbody.velocity.z);
                jump(settings.worldVars.jumpHeight, settings.worldVars.jumpTimeToPeak, "log", 0f);
                state = PlayerState.Gameplay_Air;
            } else if(dist <= settings.worldVars.jumpGroundThreshold) {
                jumpDelay = 0;
                physbody.drag = 0;
                physbody.velocity = new Vector3(physbody.velocity.x, 0, physbody.velocity.z);
                jump(settings.worldVars.jumpHeight - dist, settings.worldVars.jumpTimeToPeak, "log", 0f);
                state = PlayerState.Gameplay_Air;
            } else if(jumpDelay > 0f) {
                jumpDelay -= Time.fixedDeltaTime;
            } else {
                jumpDelay = settings.worldVars.jumpWaitForGround;
            }
        }

        moveDir = settings.input.Gameplay.Move.ReadValue<Vector2>();
        // moveDir = new Vector2(0, 1);
        if (moveDir != Vector2.zero)
        {
            switch (state)
            {
                case PlayerState.Gameplay_Air:
                    physbody.velocity = children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                        moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.airTimeMultiplier,
                        physbody.velocity.y,
                        moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.airTimeMultiplier
                    );
                    break;
                case PlayerState.Gameplay_Ground:
                    if (onGround)
                    {
                        if (sprinting)
                        {
                            physbody.velocity = children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier
                            );
                        }
                        else
                        {
                            physbody.velocity = children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
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
                    }
                    else
                    {
                        if (sprinting)
                        {
                            physbody.velocity = children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier * settings.worldVars.midHopMultiplier,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier * settings.worldVars.midHopMultiplier
                            );
                        }
                        else
                        {
                            physbody.velocity = children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.midHopMultiplier,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.midHopMultiplier
                            );
                        }
                    }
                    break;
            }
        }
        physbody.velocity += new Vector3(
            0,
            (-1f + settings.worldVars.playerGravityMultiplier) * Physics.gravity.y * Time.fixedDeltaTime,
            0
        );
    }

    public void jump(float height, float time, string curveType = "linear", float strMultiplier = 0f)
    {
        if (strMultiplier < 0)
        {
            strMultiplier = 0f;
        }
        else if (strMultiplier > 1)
        {
            strMultiplier = 1f;
        }

        if (time <= 0)
        {
            print("Failed to jump. Time must be greater than 0");
            return;
        }

        float multiplier = 1f;

        if (state == PlayerState.Gameplay_Ground)
        {
            multiplier = jumpMagnitudeMultiplier;
        }

        System.Action<Global.Interpolate, object[]> func = (parent, args) =>
        {
            PlayerScript playerRef = (PlayerScript) args[0];
            if(curveType.Equals("const")){
                playerRef.physbody.velocity = new Vector3(
                    playerRef.physbody.velocity.x,
                    parent.curve.Evaluate(parent.elapsedTime),
                    playerRef.physbody.velocity.z
                );
                parent.elapsedTime += Time.fixedDeltaTime;
                if(parent.elapsedTime >= parent.curve[1].time){
                    playerRef.physbody.velocity = new Vector3(
                        playerRef.physbody.velocity.x,
                        0f,
                        playerRef.physbody.velocity.z
                    );
                }
            } else if (parent.iteration == 0)
            {
                Vector3 velocity = new Vector3(
                    0,
                    (parent.curve.Evaluate(parent.elapsedTime) * multiplier) + (-Physics.gravity.y * Time.fixedDeltaTime * settings.worldVars.playerGravityMultiplier),
                    0
                );
                playerRef.physbody.velocity += velocity;
            }
            else
            {
                Vector3 velocityOld = new Vector3(0, parent.curve.Evaluate(parent.elapsedTime) * multiplier, 0);
                playerRef.physbody.velocity -= velocityOld;
                parent.elapsedTime += Time.fixedDeltaTime;
                Vector3 velocityNew = new Vector3(
                    0,
                    (parent.curve.Evaluate(parent.elapsedTime) * multiplier) + (-Physics.gravity.y * Time.fixedDeltaTime * settings.worldVars.playerGravityMultiplier),
                    0
                );
                playerRef.physbody.velocity += velocityNew;
            }
        };

        float m0;
        float m1;
        float p0;

        switch (curveType)
        {
            case "const":
                float constVelocity = height / time;

                Global.instance.interpolate.Add(new Global.Interpolate(
                    new AnimationCurve(new Keyframe[] {
                        new Keyframe() {time = 0f, value = constVelocity},
                        new Keyframe() {time = time, value = constVelocity}
                    }),
                    func, args: new object[] {this}
                ));
                break;
            case "linear":
                Global.instance.interpolate.Add(new Global.Interpolate(
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
                    }), func, args: new object[] {this}
                ));
                break;
            case "quad":
                m0 = -2f - (1f * strMultiplier);
                p0 = -1f * (((-12f * height) + (m0 * time)) / (6f * time));

                Global.instance.interpolate.Add(new Global.Interpolate(
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
                    }), func, args: new object[] {this}
                ));
                break;
            case "log":
                m1 = -2f - (1f * strMultiplier);
                p0 = -1f * (((-12f * height) - (m1 * time)) / (6f * time));

                Global.instance.interpolate.Add(new Global.Interpolate(
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
                    }), func, args: new object[] {this}
                ));
                break;
        }
    }
}
