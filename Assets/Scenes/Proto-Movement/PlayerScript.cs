using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public string state;
    private bool sprinting = false;

    public SettingVars settings;
    private Dictionary<string, GameObject> gameObjects;
    public LayerMask groundLayer;

    public Rigidbody physbody;
    private float height;
    public bool onGround = false;

    private List<interpolate> interpolates;
    private AnimationCurve movementMagnitude;
    private float jumpMagnitudeMultiplier = 0;
    private Vector2 moveDir;
    public enum PlayerState
    {
        OnGround,
        OnAir,
        Hopping,
        Sprinting
    }
    public PlayerState currentState = PlayerState.OnGround;
    private bool applyMovementForces = true;
    private bool justJumped = false;
    private float initialJumpHeight = 0f;
    private float maxJumpHeight = 0f;
    private bool trackingHeight = false;
    float jumpHeightDifference;
    public float maximumHopHeight = 48;
    public class interpolate
    {
        public AnimationCurve curve;
        public float elapsedTime;
        public int iteration = 0;

        private System.Action<interpolate> func;

        public interpolate(AnimationCurve curve, System.Action<interpolate> func, float time = 0f)
        {
            if (curve.length < 2)
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
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            gameObjects[child.name] = child.gameObject;
        }

        height = gameObjects["Collision"].GetComponent<CapsuleCollider>().height;
    }

    private void Update()
    {
        sprinting = settings.input.Gameplay.Sprint.IsPressed();
        PlayerStateHandler();
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
        }
        else
        {
            physbody.drag = 0;
        }
        if (applyMovementForces && settings.input.Gameplay.Jump.IsPressed() && onGround)
        {
            physbody.drag = 0;
            physbody.velocity = new Vector3(physbody.velocity.x, 0, physbody.velocity.z);
            jump(settings.worldVars.jumpHeight, settings.worldVars.jumpTimeToPeak, "quad", 3f);
            state = "Gameplay_Air";
        }

        this.moveDir = settings.input.Gameplay.Move.ReadValue<Vector2>();
        if (applyMovementForces && moveDir != Vector2.zero)
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
                    }
                    else
                    {
                        if (sprinting)
                        {
                            physbody.velocity = gameObjects["Orientation"].GetComponent<Transform>().rotation * new Vector3(
                                moveDir.x * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier * settings.worldVars.midHopMultiplier,
                                physbody.velocity.y,
                                moveDir.y * settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier * settings.worldVars.midHopMultiplier
                            );
                        }
                        else
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

        for (int i = interpolates.Count - 1; i >= 0; i--)
        {
            if (interpolates[i].iterate())
            {
                interpolates.RemoveAt(i);
            }
        }
    }

    public void DisableMovementForces()
    {
        applyMovementForces = false;
        physbody.drag = 0;
    }
    public void EnableMovementForces()
    {
        applyMovementForces = true;
        physbody.drag = settings.worldVars.groundDrag;

    }
    void TrackingHopHeight()
    {
        if (onGround)
        {
            if (settings.input.Gameplay.Move.ReadValue<Vector2>() != Vector2.zero)
            {
                initialJumpHeight = transform.position.y;
                trackingHeight = true;
            }

        }

        // If we are tracking the height, update the max height value
        if (trackingHeight)
        {
            if (transform.position.y > maxJumpHeight)
            {
                maxJumpHeight = transform.position.y;
            }

            // Continuous check for exceeding maxHopHeight
            if (maxJumpHeight - initialJumpHeight >= maximumHopHeight)
            {
                currentState = PlayerState.OnAir;
            }
        }

        // Once the player lands back on the ground
        if (onGround && trackingHeight)
        {
            jumpHeightDifference = maxJumpHeight - initialJumpHeight;

            // Reset tracking variables
            trackingHeight = false;
            maxJumpHeight = 0f;
        }
    }
    public float hopVelocityThreshold = 1f; // Set to your specific needs

    public void PlayerStateHandler()
    {
        if (onGround)
        {
            if (IsPlayerMoving())
            {
                if (IsPlayerSprinting())
                {
                    currentState = PlayerState.Sprinting;
                }
                else
                {
                    currentState = PlayerState.Hopping;
                }
            }
            else
            {
                currentState = PlayerState.OnGround;
            }

            if (justJumped)
            {
                currentState = PlayerState.OnAir;
                justJumped = false;
            }
        }
        else // If not on the ground
        {
            if (currentState != PlayerState.Hopping)
            {
                currentState = PlayerState.OnAir;
            }
        }
        if (Mathf.Abs(physbody.velocity.y) > hopVelocityThreshold)
        {
            currentState = PlayerState.OnAir;
        }
        TrackingHopHeight();

    }
    bool IsPlayerSprinting()
    {
        float regularSpeed = settings.worldVars.moveSpeed * moveDir.magnitude;
        float sprintingSpeed = settings.worldVars.moveSpeed * settings.worldVars.sprintSpeedMultiplier * moveDir.magnitude;
        float currentSpeed = physbody.velocity.magnitude;

        // Calculate the difference between current speed and both regular and sprinting speeds
        float diffRegular = Mathf.Abs(currentSpeed - regularSpeed);
        float diffSprint = Mathf.Abs(currentSpeed - sprintingSpeed);

        // If sprinting, and the difference to the sprinting speed is smaller than the difference to regular speed,
        // then we consider the player is indeed sprinting.
        if (sprinting && diffSprint < diffRegular)
        {
            return true;
        }

        return false;
    }
    private bool IsPlayerMoving()
    {
        return settings.input.Gameplay.Move.ReadValue<Vector2>() != Vector2.zero; // Adjust the threshold if necessary.
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

        if (state == "Gameplay_Ground")
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
