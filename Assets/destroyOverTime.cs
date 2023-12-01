using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyOverTime : MonoBehaviour
{
    public int timeToDestroy;

    // Start is called before the first frame update
    void Start()
    {

        if (timeToDestroy == 0)
            {
                timeToDestroy = 3;
            }

        Destroy(this.gameObject, 3);
    }
}
