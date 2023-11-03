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

    [Header("Patroling")]
    Vector3 walkPoint;
    public float walkPointRange;
    public float patrolRadius;
    private Vector3 patrolCenter;
    private bool walkPointSet;
    public float patrolDelay = 5f; // The delay between reaching a point and moving to the next one
    private bool isPatrolling = false; // New flag to check whether the coroutine is running
    [SerializeField] float stoppingDistance = 3;
    public float chasingSpeed;

    [Header("Attacking")]
    public float attackRange;
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public Collider damageCollider;

    [Header("Defeat Effect")]
    [SerializeField] GameObject sleepEffect;
    private bool sleepEffectSpawned = false; // Add a flag to check if the sleep effect has been spawned
    public bool isDefeated = false;


    [Header("Layers")]
    public LayerMask whatIsGround, whatIsPlayer;

    private bool playerInAttackRange;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        fieldOfView = GetComponent<FieldOfView>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        patrolCenter = transform.position; // Set the patrol center to the enemy's starting position
    }
    private void Update()
    {
        healthCheck();
    }
    private void FixedUpdate()
    {
        if(health >0)
        {
            // Check for sight and attack range
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (fieldOfView.hasSpottedPlayer)
            {

                agent.destination = fieldOfView.playerRef.transform.position;
                agent.speed = chasingSpeed;
                if (!playerInAttackRange)
                {
                    Vector3 directionToPlayer = fieldOfView.playerRef.transform.position - transform.position;
                    float distanceToPlayer = directionToPlayer.magnitude;

                    if (distanceToPlayer > stoppingDistance) // Only move towards the player if outside the stopping distance
                    {
                        agent.stoppingDistance = stoppingDistance;
                    }
                    else
                    {
                        agent.isStopped = true; // Stop the agent from moving
                    }

                    directionToPlayer.y = 0; // This line keeps the AI from tilting head up or down
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Adjust the 5f value to make rotation faster or slower
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
                // agent.isStopped = true; // Stop the agent from moving

                //Play the attack animation here
                //Or
                //Set the parameter in animator to trigger animation

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

        while (!fieldOfView.hasSpottedPlayer || !agent.isOnNavMesh)
        {
            if (!agent.enabled) yield break; // Exit if the agent is disabled

            if (!walkPointSet) SearchWalkPoint(); // Find a new patrol point if not already set


            agent.SetDestination(walkPoint); // Set the enemy's destination to the walk point

            // Check if the enemy is close to the walk point
            if (Vector3.Distance(transform.position, walkPoint) < 1)
            {
                walkPointSet = false; // Reset walk point for the next loop
                agent.isStopped = true; // Stop the agent
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
        isPatrolling = false; // Reset the flag when the coroutine finishes
    }

    void healthCheck()
    {
        if (health <= 0)
        {
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
        
    }

    private IEnumerator DisableEnemy()
    {
        isDefeated = true;
        agent.enabled = false;
        yield return new WaitForSeconds(3);
        // Reset any current movement and rotational velocity.
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Replace with the correct layer name so the player can't interact with the Enemy
        // after they're defeated
        gameObject.layer = LayerMask.NameToLayer("DeadEnemies");
    }
}
