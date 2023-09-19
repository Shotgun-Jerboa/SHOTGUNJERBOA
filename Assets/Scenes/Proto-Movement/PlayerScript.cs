using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public SettingVars settings;
    public AnimationCurve[] jumpCurve;
    public LayerMask groundLayer;

    private Dictionary<string, GameObject> gameObjects;
    private bool onGround = false;
    private float height;

    private Rigidbody physbody;
    public List<float> jumps = new List<float>(); // This is public so it can be tracked while testing
    
    private float maxHeight = 0; // Deprecated

    private int use = 0;

    void Start()
    {
        jumpCurve = new AnimationCurve[2];

        jumpCurve[0] = new AnimationCurve();
        Keyframe frame1 = new()
        {
            time = 0,
            value = 1,
            inWeight = 0,
            outWeight = 0,
            inTangent = -12f,
            outTangent = -12f
        };
        Keyframe frame2 = new()
        {
            time = 0.2f,
            value = 0,
            inWeight = 0,
            outWeight = 0,
            inTangent = 0,
            outTangent = 0
        };
        jumpCurve[0].AddKey(frame1);
        jumpCurve[0].AddKey(frame2);

        jumpCurve[1] = new AnimationCurve();
        frame1 = new()
        {
            time = 0,
            value = 1,
            inWeight = 0,
            outWeight = 0,
            inTangent = 0f,
            outTangent = 0f
        };
        frame2 = new()
        {
            time = 0.2f,
            value = 0,
            inWeight = 0,
            outWeight = 0,
            inTangent = 0,
            outTangent = 0
        };
        jumpCurve[1].AddKey(frame1);
        jumpCurve[1].AddKey(frame2);


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
        if (settings.input.Gameplay.TestButton.WasPressedThisFrame())
        {
            if (use == 0)
            {
                use++;
            } else
            {
                use--;
            }
            print(use);
        }
    }

    private void FixedUpdate()
    {
        // Deprecated
        //if(gameObject.transform.position.y > maxHeight)
        //{
        //    maxHeight = gameObject.transform.position.y;
        //    print(maxHeight);
        //}

        onGround = Physics.Raycast(transform.position, Vector3.down, (height * 0.5f) + 0.1f, groundLayer);


        Vector2 moveDir = settings.input.Gameplay.Move.ReadValue<Vector2>();
        if (moveDir != Vector2.zero)
        {
            physbody.velocity = gameObjects["Orientation"].GetComponent<Transform>().rotation * new Vector3(moveDir.x * 5, physbody.velocity.y, moveDir.y * 5);
            if (onGround)
            {
                jumps.Add(0);
            }
        }

        int j = 0;
        for(int i = 0; i < jumps.Count; i++)
        {
            if(jumps[j] != 0) {
                Vector3 velocity1 = new Vector3(0, jumpCurve[use].Evaluate(jumps[j]) * 10, 0);
                physbody.velocity -= velocity1;
                jumps[j] += Time.fixedDeltaTime;
                Vector3 velocity2 = new Vector3(0, jumpCurve[use].Evaluate(jumps[j]) * 10, 0);
                physbody.velocity += velocity2;
            } else
            {
                Vector3 velocity1 = new Vector3(0, jumpCurve[use].Evaluate(jumps[j]) * 10, 0);
                physbody.velocity += velocity1;
                jumps[j] += Time.fixedDeltaTime;
            }
            if (jumpCurve[use][jumpCurve[use].length - 1].time <= jumps[j])
            {
                jumps.RemoveAt(j);
                j--;
            }
            j++;
        }
    }
}
