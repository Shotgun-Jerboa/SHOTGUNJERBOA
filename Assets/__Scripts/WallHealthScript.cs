using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHealthScript : MonoBehaviour
{
    public float  WallHealth = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DamageWall(float damage)
    {

        if (WallHealth > 0)
        {
            WallHealth-= damage;
        }

        if (WallHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
