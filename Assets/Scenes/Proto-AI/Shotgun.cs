using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    SettingVars inputActions;

    // Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int bulletsPerTap;
    public int bulletsLeft;
    int bulletsShot;
    public bool isRecoil;
    
    // Bools 
    bool readyToShoot, reloading;

    // Reference
    Camera fpsCam;
    public Transform shootingPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    Rigidbody playerRB;
    public PlayerScript playerRef;

    [SerializeField] float recoilForce;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] float impactForce;
    public bool isShooting = false;

    private float lastShotTime;

    public enum InputMethod
    {
        None,
        LeftClick,
        RightClick,
        Q,
        E
    }

    [SerializeField] InputMethod inputMethod;
     void Awake()
    {
        readyToShoot = true;
        playerRef = GameObject.Find("Player").GetComponent<PlayerScript>();
        inputActions = GameObject.Find("Settings").GetComponent<SettingVars>();
        fpsCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        if (inputMethod == InputMethod.LeftClick)
        {
            inputActions.input.Gameplay.LeftHandPressed.performed += ctx => Shoot(InputMethod.LeftClick);
            inputActions.input.Gameplay.LeftHandReleased.performed += ctx => isShooting = false;

            inputActions.input.Gameplay.LeftReloadPressed.performed += ctx => MakeReadyToShoot(InputMethod.Q);
        }
        else if (inputMethod == InputMethod.RightClick)
        {
            inputActions.input.Gameplay.RightHandPressed.performed += ctx => Shoot(InputMethod.RightClick);
            inputActions.input.Gameplay.RightHandReleased.performed += ctx => isShooting = false;

            inputActions.input.Gameplay.RightReloadPressed.performed += ctx => MakeReadyToShoot(InputMethod.E);

        }
    }

    protected virtual void Start()
    {
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    private void Update()
    {
       
    }
    private void Shoot(InputMethod inputMethod)
    {
        if (! reloading &&readyToShoot && bulletsLeft > 0)
        {

            // Calculate the time since the last shot
            float timeSinceLastShot = Time.time - lastShotTime;

            if ( timeSinceLastShot >= timeBetweenShooting)
            {

                bulletsShot = bulletsPerTap;
                readyToShoot = false;

                // Update the last shot time
                lastShotTime = Time.time;

                if (inputMethod == InputMethod.LeftClick ||
           (inputMethod == InputMethod.RightClick))
                {
                    float x = Random.Range(-spread, spread);
                    float y = Random.Range(-spread, spread);
                    Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

                    if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
                    {


                            EnemyAI enemy = rayHit.collider.GetComponent<EnemyAI>();
                            if (enemy != null)
                            {
                                enemy.TakeDamage(damage);
                            }
                        /* if (rayHit.collider.CompareTag("Ground") || rayHit.collider.CompareTag("Wall"))
                         {
                             Quaternion impactRotation = Quaternion.LookRotation(rayHit.normal);
                             // GameObject impact = Instantiate(bulletHoleGraphic, rayHit.point, impactRotation);
                             // impact.transform.parent = rayHit.transform;
                         }*/

                        if (rayHit.rigidbody != null)
                        {
                            rayHit.rigidbody.AddForce(-rayHit.normal * impactForce);
                        }
                    }
                    bulletsLeft--;
                    bulletsShot--;

                    if (isRecoil)
                    {
                        if (!playerRef.onGround)
                        {


                            // Calculate the current downward velocity due to gravity
                            float currentGravityEffect = Vector3.Dot(playerRB.velocity, Vector3.up);

                            // Neutralize the gravity effect for the recoil duration (we subtract it from the recoilForce)
                            Vector3 effectiveRecoilForce = -direction.normalized * (recoilForce - currentGravityEffect);

                            playerRB.velocity += effectiveRecoilForce;

                        }
                        else
                        {
                            // When grounded, just apply the recoil as usual
                            playerRB.velocity += -direction.normalized * recoilForce;
                        }
                    }
                    if (bulletsShot > 0 && bulletsLeft > 0)
                        Invoke("Shoot", timeBetweenShots);
                }
            }
               
        }      
        
    }

    private void ResetShot()
    {
        readyToShoot = true;
        reloading = false;
    }
    float lastReloadTime;
    private void MakeReadyToShoot(InputMethod inputMethod)
    {
        if (inputMethod == InputMethod.Q && this.inputMethod == InputMethod.LeftClick && ! reloading)
        {
            reloading = true;
            lastReloadTime = Time.time;

            // After the reload time has passed, the gun is set ready to shoot again
            Invoke("ResetShot", reloadTime);
        }
        else if (inputMethod == InputMethod.E && this.inputMethod == InputMethod.RightClick && !reloading)
        {
            reloading = true;
            lastReloadTime = Time.time;

            // After the reload time has passed, the gun is set ready to shoot again
            Invoke("ResetShot", reloadTime);
        }
    }

}
