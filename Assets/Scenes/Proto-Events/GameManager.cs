using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance

    {
        get
        {
            if (_instance == null)
                Debug.LogError("Game Manager is Null!");
            return _instance;
        }
    }

    public TextMeshProUGUI GameOver;

    //not implemented yet, for orbiting around player corpse >
    private Camera m_MainCamera;

    private GameObject player;

    private void Awake()
    {
        Debug.Log("GameManagerActive");
        _instance = this;
        if(GameOver != null)
        {
            GameOver.gameObject.SetActive(false);
        }

        m_MainCamera = Camera.main;

        player = GameObject.FindWithTag("Player");
        if (player == null)
            {
                player = GameObject.Find("Player");
            }
    }

    public void PlayerAnnihilation()
    {
        Debug.Log("PlayerDeathComing");
        //insert pause player input
        //camera orbit goes here too?
        if (GameOver != null)
        {
            GameOver.gameObject.SetActive(true);
        }

        StartCoroutine(GameOverTextCoroutine());
    }

    private void PlayerDeathCheckpoint()
    {
        //TODO if needed
    }

    IEnumerator GameOverTextCoroutine()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    void Update()
    {

    }
}
