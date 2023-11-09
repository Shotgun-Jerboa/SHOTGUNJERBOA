using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    GameObject[] EnemyArray;
    int enemyCount;
    public TMP_Text enemyUICount;

    // Start is called before the first frame update
    void Start()
    {
        if (enemyUICount == null)
        {
            Debug.Log("Warning, no Enemy Game Counter UI is linked to the script!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = EnemyArray.Length;
        enemyUICount.text = "Enemies Remaining: " + enemyCount.ToString();
    }
}
