using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public Transform attackPos;
    public bool IsPlayerFlash;
    private void Awake()
    {
    }
    void Start()
    {
        Invoke("Destroyy", 1f);
    }

    private void Update()
    {
        if (IsPlayerFlash)
            transform.position = attackPos.position;
    }

    void Destroyy()
    {
        Destroy(gameObject);
    }
}
