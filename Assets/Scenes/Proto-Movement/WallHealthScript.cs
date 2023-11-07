using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHealthScript : MonoBehaviour
{
    
    public int WallHealth = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageWall()
    {
        if (WallHealth >= 1)
        {
            WallHealth--;
        }

        if (WallHealth == 0)
        {
            Destroy(gameObject);
        }
    }
}
