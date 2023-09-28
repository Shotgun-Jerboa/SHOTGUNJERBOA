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

    //Particle Effects
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject BulletHoleEffect;
    [SerializeField] TrailRenderer bulletTrail;

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

    void Start()
    {
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
    }


    private void Shoot(InputMethod inputMethod)
    {
        if (!reloading && readyToShoot && bulletsLeft > 0)
        {

            // Calculate the time since the last shot
            float timeSinceLastShot = Time.time - lastShotTime;

            if (timeSinceLastShot >= timeBetweenShooting)
            {

                readyToShoot = false;

                // Update the last shot time
                lastShotTime = Time.time;
                Vector3 direction = fpsCam.transform.forward;
                if (inputMethod == InputMethod.LeftClick ||
           (inputMethod == InputMethod.RightClick))
                {
                    // Consume just one bullet
                    bulletsLeft--;

                    GameObject MuzzleFlashInstance = Instantiate(muzzleFlash, shootingPoint.position, Quaternion.identity);
                    Destroy particleScript = MuzzleFlashInstance.GetComponent<Destroy>();
                    particleScript.attackPos = shootingPoint;

                    for (int i = 0; i < bulletsPerTap; i++)
                    {
                        float x = Random.Range(-spread, spread);
                        float y = Random.Range(-spread, spread);
                        Vector3 bulletDirection = direction + new Vector3(x, y, 0); // Random direction for bullet

                        if (Physics.Raycast(fpsCam.transform.position, bulletDirection, out rayHit, range, whatIsEnemy))
                        {
                            TrailRenderer trail = Instantiate(bulletTrail, shootingPoint.position, Quaternion.identity);
                            StartCoroutine(SpawnTrail(trail, rayHit));

                            if (rayHit.collider.CompareTag("Ground") || rayHit.collider.CompareTag("Wall"))
                            {
                                Quaternion impactRotation = Quaternion.LookRotation(rayHit.normal);
                                GameObject impact = Instantiate(BulletHoleEffect, rayHit.point, impactRotation);
                                impact.transform.parent = rayHit.transform;
                            }
                        }
                    }

                    if (isRecoil)
                    {
                        float recoilMultiplier = IsPlayerMoving() ? 3f : 1.0f; // Increase recoil by 50% when moving
                        Vector3 recoilForceVector = -direction.normalized * recoilForce * recoilMultiplier;



                        if (!playerRef.onGround)
                        {
                            // Calculate the current downward velocity due to gravity
                            float currentGravityEffect = Vector3.Dot(playerRB.velocity, Vector3.up);

                            // Neutralize the gravity effect for the recoil duration (we subtract it from the recoilForce)
                            Vector3 effectiveRecoilForce = -direction.normalized * (recoilForce - currentGravityEffect);

                            //playerRB.velocity += effectiveRecoilForce;
                            playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                        }
                     
                        else
                        {
                            playerRB.AddForce(-direction.normalized * recoilForce, ForceMode.Impulse);
                        }
                    }
                    if (bulletsShot > 0 && bulletsLeft > 0)
                        Invoke("Shoot", timeBetweenShots);
                }
            }

        }

    }

    private bool IsPlayerMoving()
    {
        return playerRef.physbody.velocity.magnitude > 0.1f; // Adjust the threshold if necessary.
    }
    private void ResetShot()
    {
        readyToShoot = true;
        reloading = false;
    }
    float lastReloadTime;
    private void MakeReadyToShoot(InputMethod inputMethod)
    {
        if (inputMethod == InputMethod.Q && this.inputMethod == InputMethod.LeftClick && !reloading)
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
    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        Destroy(trail.gameObject, trail.time);

    }
}