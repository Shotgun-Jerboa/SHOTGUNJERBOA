using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateCheck : MonoBehaviour
{
    public float detectionRadius;
    public float timeToExitBattle = 5f; // Time after which battle mode is exited if no enemy is detected
    bool enemyDetected = false;
    private bool inBattle = false;
    private Coroutine checkForExitCoroutine;

    // Update is called once per frame
    void Update()
    {
        // Reset the enemyDetected flag at the beginning of each frame
        enemyDetected = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hitCollider.gameObject.GetComponent<EnemyAI>().isDefeated == false)// check if the enemy is defeated
                {
                    // Enemy detected and not defeated, so we are in battle
                    enemyDetected = true;
                    EnterBattleMode();
                    break;
                }
            }
        }

        // If no enemies detected and we are currently in battle, start the exit timer
        if (!enemyDetected && inBattle)
        {
            if (checkForExitCoroutine == null)
            {
                checkForExitCoroutine = StartCoroutine(CheckForBattleExit());
            }
        }
        else if (enemyDetected && checkForExitCoroutine != null)
        {
            // Stop the exit battle check coroutine because an enemy has been detected
            StopCoroutine(checkForExitCoroutine);
            checkForExitCoroutine = null;
        }
    }

    private void EnterBattleMode()
    {
        if (!inBattle)
        {
            inBattle = true;
            GameStateManager.Instance.EnterBattleMode();
        }
    }

    IEnumerator CheckForBattleExit()
    {
        Debug.Log("Battle exit check started.");

        // Wait for the specified time
        yield return new WaitForSeconds(timeToExitBattle);
        Debug.Log("Battle exit check time elapsed.");
        bool allEnemiesDefeated = true;

        //Check for enemies again after waiting
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        enemyDetected = false;

        // If no enemies are detected, this means all are defeated or out of range
        if (hitColliders.Length == 0)
        {
            Debug.Log("No enemies detected. Exiting battle mode.");
            ExitBattleMode();
            yield break; // Exit the coroutine early
        }

        // Check if any enemy within range is not defeated
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hitCollider.gameObject.GetComponent<EnemyAI>().isDefeated == false)
                {
                    // If any enemy is not defeated, then we should not exit battle mode
                    allEnemiesDefeated = false;
                    break;
                }
    
            }
        }
        // If all enemies are defeated, exit battle mode
        if (allEnemiesDefeated)
        {
            ExitBattleMode();
        }
        // Reset the coroutine reference
        checkForExitCoroutine = null;
    }

    private void ExitBattleMode()
    {
        inBattle = false;
        GameStateManager.Instance.ExitBattleMode();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    }
}
