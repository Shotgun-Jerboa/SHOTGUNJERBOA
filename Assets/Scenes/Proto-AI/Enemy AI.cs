using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Components")]
    private NavMeshAgent agent;
    private Rigidbody rb;
    public Transform player;
    public FieldOfView fieldOfView;

    [Header("Health")]
    public float health;

    [Header("Stationary")]
    public bool isStationary = false; // Add this flag to decide whether the enemy should patrol or remain stationary

    [Header("Patroling")]
    Vector3 walkPoint;
    public float walkPointRange;
    public float patrolRadius;
    private Vector3 patrolCenter;
    private bool walkPointSet;
    public float patrolDelay = 5f; // The delay between reaching a point and moving to the next one
    private bool isPatrolling = false; // New flag to check whether the coroutine is running
    public float chasingSpeed;

    [Header("Alerting")]
    public float alertingRadius;
    private GameObject alertAgents;
    public LayerMask enemyLayer;

    [Header("Attacking")]
    public float attackRange;
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public Collider damageCollider;

    [Header("Defeat Effect")]
    [SerializeField] GameObject sleepEffect;
    private bool sleepEffectSpawned = false; // Add a flag to check if the sleep effect has been spawned
    public bool isDefeated = false;

    [Header("Animation Control")]
    Animator animator;

    public PlayerScript playerScript;

    [Header("Layers")]
    public LayerMask whatIsGround, whatIsPlayer;

    private bool playerInAttackRange;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        fieldOfView = GetComponent<FieldOfView>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        patrolCenter = transform.position; // Set the patrol center to the enemy's starting position
        playerScript = Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>();
    }
    private void Update()
    {
        healthCheck();

        if (fieldOfView.hasSpottedPlayer)
        {
            isStationary = false;
            AlertNearbyEnemies(); // Call this when the enemy spots the player
        }

    }
    private void FixedUpdate()
    {

        if (playerScript.health <= 0)
        {
            // Player is defeated, stop chasing and attacking
            return;
        }

        if (health >0 && !isStationary)
        {
            // Check for sight and attack range
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (fieldOfView.hasSpottedPlayer)
            {
                patrolDelay = 0;
                agent.destination = fieldOfView.playerRef.transform.position;
                agent.speed = chasingSpeed;
                // Set "Run" to true while chasing the player
                animator.SetBool("Run", agent.remainingDistance > attackRange);
                isPatrolling = false;
                agent.isStopped = false; // Allow the agent to move

                StopCoroutine(Patrolling()); // Stop the patrolling
                if (!playerInAttackRange)
                {
                    Vector3 directionToPlayer = fieldOfView.playerRef.transform.position - transform.position;
                    float distanceToPlayer = directionToPlayer.magnitude;

                    if (distanceToPlayer > attackRange) // Only move towards the player if outside the stopping distance
                    {
                        agent.stoppingDistance = attackRange;
                    }
                    else
                    {
                        agent.isStopped = true; // Stop the agent from moving
                    }

                    directionToPlayer.y = 0; // This line keeps the AI from tilting head up or down
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10); // Adjust the 5f value to make rotation faster or slower
                    StopCoroutine(Patrolling()); // Stop the patrolling
                    isPatrolling = false;
                }
            }

            else if (!fieldOfView.playerInSightRange && !playerInAttackRange && !isPatrolling)
            {
                agent.stoppingDistance = 0; // Reset the stopping distance when not chasing the player
                agent.isStopped = false; // Allow the agent to move
                StartCoroutine(Patrolling());
            }
            if (fieldOfView.playerInSightRange && fieldOfView.playerInSightRange)
            {
                AttackPlayer();

            }
        }

    }

    // This method will be called by the animation event at the start of the animation
    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    // This method will be called by the animation event at the end of the animation
    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }
    IEnumerator Patrolling()
    {
        isPatrolling = true; // Set the flag when the coroutine starts

        if (isStationary)
        {
            yield break; // Exit the coroutine if the enemy is stationary
        }

        if (!fieldOfView.hasSpottedPlayer || !agent.isOnNavMesh)
        {
            if (!agent.enabled) yield break; // Exit if the agent is disabled

            if (!walkPointSet) SearchWalkPoint(); // Find a new patrol point if not already set

            agent.SetDestination(walkPoint); // Set the enemy's destination to the walk point
            // Set "Run" to true while moving to the walk point
            animator.SetBool("Run", true);

            // Check if the enemy is close to the walk point
            if (Vector3.Distance(transform.position, walkPoint) < 0.5f)
            {

                walkPointSet = false; // Reset walk point for the next loop
                agent.isStopped = true; // Stop the agent
                // Set Run parameter to false when waiting at a patrol point
                animator.SetBool("Run", false);

                yield return new WaitForSeconds(patrolDelay); // Wait for 1 second before finding the next point
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = false; // Resume the agent if it's safe to do so
                }
            }

            else
            {
                yield return new WaitForEndOfFrame(); // If not at the walk point, wait for the next frame
            }
        }

        // Set "Run" to false when patrolling stops because the player was spotted
        animator.SetBool("Run", true);
        isPatrolling = false; // Reset the flag when the coroutine finishes
    }

    void healthCheck()
    {
        if (health <= 0)
        {
            animator.SetBool("Death", true);
            // Check the flag before spawning the sleep effect
            if (!sleepEffectSpawned)
            {
                sleepEffect.gameObject.SetActive(true);
                sleepEffectSpawned = true; // Set the flag to true to avoid spawning the effect again
            }
            StartCoroutine(DisableEnemy());
        }
    }

    void SearchWalkPoint()
    {
        float randomAngle = Random.Range(0, 2 * Mathf.PI);
        float randomRadius = Random.Range(0, patrolRadius);
        float x = patrolCenter.x + randomRadius * Mathf.Cos(randomAngle);
        float z = patrolCenter.z + randomRadius * Mathf.Sin(randomAngle);

        walkPoint = new Vector3(x, transform.position.y, z);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        fieldOfView.hasSpottedPlayer = true;
    }

    private IEnumerator DisableEnemy()
    {
        isDefeated = true;
        agent.enabled = false;
        yield return new WaitForSeconds(3);
        // Replace with the correct layer name so the player can't interact with the Enemy
        // after they're defeated
        gameObject.layer = LayerMask.NameToLayer("DeadEnemies");
    }

    private void AttackPlayer()
    {
        // Call this method when it's time for the enemy to attack the player
        if (playerInAttackRange && !alreadyAttacked)
        {
            // Trigger the Bite animation
            animator.Play("Bite");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void AlertNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, alertingRadius, enemyLayer);
        foreach (var hitCollider in hitColliders)
        {
            EnemyAI enemyAI = hitCollider.GetComponent<EnemyAI>();
            if (enemyAI != null && enemyAI != this) // Make sure not to alert itself
            {
                enemyAI.BecomeAlerted();
            }
        }
    }

    public void BecomeAlerted()
    {
        if (!fieldOfView.hasSpottedPlayer)
        {
            fieldOfView.hasSpottedPlayer = true;
        }
    }
    private void ResetAttack()
    {
        // Reset the attack flag so the enemy can attack again
        alreadyAttacked = false;
    }
    //To Visualize the attack range (not important, can be deleted)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertingRadius);


    }
}
