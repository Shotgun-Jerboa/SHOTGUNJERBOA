using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public SettingVars inputActions;

    // Gun stats
    public int damage;
    public float reloadTime, spread, range;
    public int bulletsPerTap;
    public int bulletsLeft, bulletsShot;
    public bool isRecoil;

    // Bools 
    bool readyToShoot, reloading;

    // Reference
    public Camera fpsCam;
    public Transform shootingPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    Rigidbody playerRB;
    [SerializeField] Transform playerOrientation;
    [SerializeField] PlayerScript playerRef;

    [SerializeField] float recoilForce;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] float impactForce;
    public bool isShooting = false;
    float shootingAngle;
    float maxShootingAngle = 180; // Maximum angle to apply counterforce
    float minShootingAngle = 135; // Minimum angle to apply counterforce
    private float lastShotTime;

    public enum InputMethod
    {
        None,
        LeftClick,
        RightClick
    }

    [SerializeField] InputMethod inputMethod;
     void Awake()
    {
        readyToShoot = true;

        inputActions = GameObject.Find("Settings").GetComponent<SettingVars>();
        fpsCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        if (inputMethod == InputMethod.LeftClick)
        {
            inputActions.input.Gameplay.LeftHandPressed.performed += ctx => Shoot(InputMethod.LeftClick);
            inputActions.input.Gameplay.LeftHandReleased.performed += ctx => isShooting = false;
        }
        else if (inputMethod == InputMethod.RightClick)
        {
            inputActions.input.Gameplay.RightHandPressed.performed += ctx => Shoot(InputMethod.RightClick);
            inputActions.input.Gameplay.RightHandReleased.performed += ctx => isShooting = false;

        }
    }

    private void Start()
    {
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
        playerRef = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    private void Update()
    {

    }

    private void Shoot(InputMethod inputMethod)
    {
        if (readyToShoot && bulletsLeft > 0)
        {

            // Calculate the time since the last shot
            float timeSinceLastShot = Time.time - lastShotTime;

            if (timeSinceLastShot >= reloadTime)
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

                    shootingAngle = Vector3.Angle(playerOrientation.transform.up, fpsCam.transform.forward);

                    if (isRecoil)
                    {
                        if (!playerRef.isGrounded)
                        {

                            if (shootingAngle >= minShootingAngle && shootingAngle <= maxShootingAngle)
                            {
                                //Get the gravity force acting on the player
                                Vector3 gravityForce = Physics.gravity * playerRB.mass;
                                //Add a counter force that is equal and opposite to the gravity force
                                playerRB.AddForce(-gravityForce, ForceMode.Impulse);
                            }

                        }

                        // Add the recoil force as before
                        playerRB.AddForce(-direction.normalized * recoilForce, ForceMode.VelocityChange);                    
                        Invoke("ResetShot", reloadTime);

                    }
                }
            }
               
        }      
        
    }
    private void ResetShot()
    {
        readyToShoot = true;
        Debug.Log(transform.gameObject.name + " Reload");

    }

    public void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        reloading = false;
        ResetShot();
    }
}
