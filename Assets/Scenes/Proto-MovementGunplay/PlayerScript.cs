using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Stats")]
    public float health;
    public float maxHealth = 100;

    [Header("UI Effect")]
    private Animator healthUIAnimator;

    public enum PlayerState
    {
        Gameplay_Ground,
        Gameplay_Air,
        UI_Pause
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

    private int jumpCount = 0;

    private AnimationCurve movementMagnitude;
    private float jumpMagnitudeMultiplier = 1f;
    private Vector2 moveDir;

    private float maxAirMagnitude;
    private bool doAirLerp = true;

    void Start()
    {
        health = maxHealth;
        physbody = gameObject.GetComponent<Rigidbody>();
        children = new(gameObject);
        settings = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();
        hitboxHeight = children.Get("Collision").GetComponent<CapsuleCollider>().height;
        maxAirMagnitude = Mathf.Sqrt(Mathf.Pow(settings.worldVars.moveSpeed, 2) + Mathf.Pow(settings.worldVars.moveSpeed, 2)) * settings.worldVars.sprintSpeedMultiplier;
        healthUIAnimator = Global.instance.sceneTree.Get("Canvas/Canvas/Health").GetComponent<Animator>();


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
        HealthCheck();
    }

    private void FixedUpdate()
    {
        onGround = Physics.Raycast(transform.position, Vector3.down, (hitboxHeight * 0.5f) + 0.01f, groundLayer);
        if (onGround)
        {
            state = PlayerState.Gameplay_Ground;
            physbody.drag = settings.worldVars.groundDrag;
        }
        else
        {
            Ray checkGroundHeight = new(new Vector3(transform.position.x, transform.position.y - (hitboxHeight * 0.5f), transform.position.z), Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(checkGroundHeight, out hit))
            {
                if(hit.distance > hitboxHeight * 1.5){
                    state = PlayerState.Gameplay_Air;
                }
            } else
            {
                state = PlayerState.Gameplay_Air;
            }
            physbody.drag = 0;
        }

        if ((settings.input.Gameplay.Jump.IsPressed() && state == PlayerState.Gameplay_Ground) || jumpDelay > 0f)
        {
            float dist;
            RaycastHit hit;
            Ray downRay = new(new Vector3(transform.position.x, transform.position.y - (hitboxHeight * 0.5f), transform.position.z), Vector3.down);
            if(Physics.Raycast(downRay, out hit)){
                dist = hit.distance;
            } else {
                dist = settings.worldVars.jumpGroundThreshold + 1f;
            }

            if(onGround){
                jumpDelay = 0;
                physbody.drag = 0;
                jumpMagnitudeMultiplier = 1f;
                physbody.velocity = new Vector3(physbody.velocity.x, 0, physbody.velocity.z);
                jump(settings.worldVars.jumpHeight, settings.worldVars.jumpTimeToPeak, "log", 0f);
                state = PlayerState.Gameplay_Air;
            } else if(dist <= settings.worldVars.jumpGroundThreshold) {
                jumpDelay = 0;
                physbody.drag = 0;
                jumpMagnitudeMultiplier = 1f;
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
        if (moveDir != Vector2.zero)
        {
            switch (state)
            {
                case PlayerState.Gameplay_Air:
                    physbody.velocity += children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                        moveDir.x * settings.worldVars.moveSpeed * Time.fixedDeltaTime * settings.worldVars.airTimeMultiplier,
                        0,
                        moveDir.y * settings.worldVars.moveSpeed * Time.fixedDeltaTime * settings.worldVars.airTimeMultiplier
                    );
                    break;
                case PlayerState.Gameplay_Ground:
                    if (onGround)
                    {
                        if (sprinting)
                        {
                            physbody.velocity += children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.baseAcceleration * Time.fixedDeltaTime * settings.worldVars.sprintSpeedMultiplier,
                                0,
                                moveDir.y * settings.worldVars.baseAcceleration * Time.fixedDeltaTime * settings.worldVars.sprintSpeedMultiplier
                            );
                        }
                        else
                        {
                            physbody.velocity += children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.baseAcceleration * Time.fixedDeltaTime,
                                0,
                                moveDir.y * settings.worldVars.baseAcceleration * Time.fixedDeltaTime
                            );
                        }
                        physbody.velocity = new Vector3(physbody.velocity.x, 0, physbody.velocity.z);
                        jumpMagnitudeMultiplier = movementMagnitude.Evaluate(moveDir.magnitude);
                        physbody.drag = 0;
                        if (sprinting)
                        {
                            jump(settings.worldVars.sprintHopHeight, settings.worldVars.sprintTimeToPeak, "log", 0f, true);
                        }
                        else
                        {
                            jump(settings.worldVars.hopHeight, settings.worldVars.hopTimeToPeak, "log", 0f, true);
                        }
                    }
                    else
                    {
                        if (sprinting)
                        {
                            physbody.velocity += children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.baseAcceleration * Time.fixedDeltaTime * settings.worldVars.sprintSpeedMultiplier,
                                0,
                                moveDir.y * settings.worldVars.baseAcceleration * Time.fixedDeltaTime * settings.worldVars.sprintSpeedMultiplier
                            );
                        }
                        else
                        {
                            physbody.velocity += children.Get("Orientation").GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.baseAcceleration * Time.fixedDeltaTime,
                                0,
                                moveDir.y * settings.worldVars.baseAcceleration * Time.fixedDeltaTime
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

        Vector2 xzVel = new Vector2(
            physbody.velocity.x,
            physbody.velocity.z
        );

        switch (state) {
            case PlayerState.Gameplay_Ground:
                if (sprinting) {
                    xzVel = Vector2.ClampMagnitude(xzVel, Mathf.Sqrt(Mathf.Pow(settings.worldVars.moveSpeed, 2) + Mathf.Pow(settings.worldVars.moveSpeed, 2)) * settings.worldVars.sprintSpeedMultiplier);
                } else {
                    xzVel = Vector2.ClampMagnitude(xzVel, Mathf.Sqrt(Mathf.Pow(settings.worldVars.moveSpeed, 2) + Mathf.Pow(settings.worldVars.moveSpeed, 2)));
                }
                break;
            case PlayerState.Gameplay_Air:
                if(xzVel.magnitude >= settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier && doAirLerp)
                {
                    airTimeLerp(xzVel);
                }
                
                xzVel = Vector2.ClampMagnitude(xzVel, maxAirMagnitude);
                break;
        }
        physbody.velocity = new Vector3(
            xzVel.x,
            Mathf.Clamp(physbody.velocity.y, -53, 53),
            xzVel.y
        );
    }

    public void airTimeLerp(Vector2 xz)
    {
        System.Action<Global.Interpolate, object[]> func = (parent, args) =>
        {
            PlayerScript playerRef = (PlayerScript) args[0];
            playerRef.maxAirMagnitude = parent.curve.Evaluate(parent.elapsedTime);
            parent.elapsedTime += Time.fixedDeltaTime;
            if(parent.elapsedTime >= parent.curve[1].time)
            {
                playerRef.maxAirMagnitude = Mathf.Sqrt(Mathf.Pow(settings.worldVars.moveSpeed, 2) + Mathf.Pow(settings.worldVars.moveSpeed, 2)) * settings.worldVars.sprintSpeedMultiplier;
                playerRef.doAirLerp = true;
            }
        };

        float endTime = (xz.magnitude - (Mathf.Sqrt(Mathf.Pow(settings.worldVars.moveSpeed, 2) + Mathf.Pow(settings.worldVars.moveSpeed, 2)) * settings.worldVars.sprintSpeedMultiplier)) / (settings.worldVars.baseAcceleration * Time.fixedDeltaTime);
        if (endTime < 0.5)
        {
            return;
        }

        doAirLerp = false;
        Global.instance.interpolate.Add(new Global.Interpolate(new AnimationCurve(new Keyframe[]
            {
                new Keyframe() {time = 0f, value = xz.magnitude},
                new Keyframe() {time = endTime, value = Mathf.Sqrt(Mathf.Pow(settings.worldVars.moveSpeed, 2) + Mathf.Pow(settings.worldVars.moveSpeed, 2)) * settings.worldVars.sprintSpeedMultiplier}
            }), func, args: new object[] { this }
        ));
    }

    public void jump(float height, float time, string curveType = "linear", float strMultiplier = 0f, bool isHop=false)
    {
        if(jumpCount > 0 && isHop)
        {
            return;
        }
        jumpCount++;

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

        height = Mathf.Clamp(height * multiplier, 0.5f * height, Mathf.Infinity);
        time = Mathf.Clamp(time * multiplier, 0.5f * time, Mathf.Infinity);

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
                    parent.curve.Evaluate(parent.elapsedTime) + (-Physics.gravity.y * Time.fixedDeltaTime * settings.worldVars.playerGravityMultiplier),
                    0
                );
                playerRef.physbody.velocity += velocity;
            }
            else
            {
                Vector3 velocityOld = new Vector3(0, parent.curve.Evaluate(parent.elapsedTime), 0);
                playerRef.physbody.velocity -= velocityOld;
                parent.elapsedTime += Time.fixedDeltaTime;
                Vector3 velocityNew = new Vector3(
                    0,
                    parent.curve.Evaluate(parent.elapsedTime) + (-Physics.gravity.y * Time.fixedDeltaTime * settings.worldVars.playerGravityMultiplier),
                    0
                );
                playerRef.physbody.velocity += velocityNew;
            }
            if (parent.elapsedTime >= parent.curve[1].time)
            {
                playerRef.jumpCount--;
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

    void HealthCheck()
    {
        if (health <=0)
        {
            Debug.Log("Game Over!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("DamageCollider"))
        {
            DamageDealer damageDealer = other.GetComponent<DamageDealer>();
            if (damageDealer && !damageDealer.HasDealtDamage)
            {
                health -= damageDealer.damage;
                healthUIAnimator.SetTrigger("Damage");
                damageDealer.HasDealtDamage = true; // Set the flag to true after dealing damage

                // Optionally, you can start a coroutine or set a timer to reset this flag after a certain cooldown period
                StartCoroutine(ResetDamageDealer(damageDealer));
            }
        }
    }

    IEnumerator ResetDamageDealer(DamageDealer damageDealer)
    {
        yield return new WaitForSeconds(damageDealer.cooldown); // Assuming DamageDealer has a cooldown property
        damageDealer.HasDealtDamage = false; // Reset the flag
    }
}
