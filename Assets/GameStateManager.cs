using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set;}
    public delegate void BattleStateChanged(bool isInBattle);
    public event BattleStateChanged OnBattleStateChanged;

    public bool IsInBattle { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterBattleMode()
    {
        IsInBattle = true;
        OnBattleStateChanged?.Invoke(IsInBattle);
    }

    public void ExitBattleMode()
    {
        IsInBattle = false;
        OnBattleStateChanged?.Invoke(IsInBattle);
    }
}
