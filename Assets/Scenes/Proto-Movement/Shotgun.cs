using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun : MonoBehaviour
{
    public SettingVars inputActions;

    // Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
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
    public PlayerScript playerRef;
    public Text AmmoCounter;

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
            playerRef.Ammo--;
            AmmoCounter.text = "Ammo: " + playerRef.Ammo;
            inputActions.input.Gameplay.LeftHandReleased.performed += ctx => isShooting = false;
        }
        else if (inputMethod == InputMethod.RightClick)
        {
            inputActions.input.Gameplay.RightHandPressed.performed += ctx => Shoot(InputMethod.RightClick);
            playerRef.Ammo--;
            AmmoCounter.text = "Ammo: " + playerRef.Ammo;
            inputActions.input.Gameplay.RightHandReleased.performed += ctx => isShooting = false;

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
        if (readyToShoot && !reloading && bulletsLeft > 0)
        {

            // Calculate the time since the last shot
            float timeSinceLastShot = Time.time - lastShotTime;

            if (timeSinceLastShot >= timeBetweenShooting)
            {

                bulletsShot = bulletsPerTap;
                readyToShoot = false;
                Debug.Log(inputMethod);

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
                        Debug.Log(rayHit.collider.name);

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

                    Invoke("ResetShot", timeBetweenShooting);
                    if (isRecoil)
                    {
                        if (!playerRef.onGround)
                        {

                            
                                //Get the gravity force acting on the player
                                Vector3 gravityForce = Physics.gravity * playerRB.mass;
                                //Add a counter force that is equal and opposite to the gravity force
                                playerRB.AddForce(-gravityForce, ForceMode.Impulse);
                                //playerRB.velocity += (-direction.normalized * recoilForce) + (-gravityForce * Time.deltaTime);
    

                        }

                        // Add the recoil force as before
                       // playerRB.AddForce(-direction.normalized * recoilForce, ForceMode.VelocityChange);
                        playerRB.velocity += -direction.normalized * recoilForce;

                        Invoke("ResetShot", reloadTime);
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
    }

    public void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        reloading = false;
    }
}
