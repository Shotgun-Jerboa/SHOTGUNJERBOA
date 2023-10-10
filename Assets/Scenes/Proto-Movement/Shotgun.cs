using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] SettingVars inputActions;

    [Header("Gun Stas")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int bulletsPerTap;
    [SerializeField] float recoilForce;


    [Header("Amount Stas")]
    public int ammo;
    public bool isRecoil;

    [Header("Particle Effect")]
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject BulletHoleEffect;
    [SerializeField] TrailRenderer bulletTrail;

    // Bools 
    bool readyToShoot, reloading;

    [Header("Reference")]
    [SerializeField] Camera fpsCam;
    public Transform shootingPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    Rigidbody playerRB;
    public PlayerScript playerRef;

    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    //[SerializeField] float impactForce;
    public bool isShooting = false;
    private int shootPressCount = 0;

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


    void Start()
    {
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    private bool hasShot = false;

    private void Shoot(InputMethod inputMethod)
    {
        if (!reloading)
        {
            hasShot = true;
            shootPressCount++;

            if (shootPressCount % 2 != 0)
            {
                if (ammo > 0)
                {
                    // Calculate the time since the last shot
                    float timeSinceLastShot = Time.time - lastShotTime;

                    if (timeSinceLastShot >= timeBetweenShooting)
                    {

                        // Update the last shot time
                        lastShotTime = Time.time;
                        Vector3 direction = fpsCam.transform.forward;
                        if (inputMethod == InputMethod.LeftClick ||
                            (inputMethod == InputMethod.RightClick))
                        {
                            Debug.Log(playerRef.onGround ? "Player is grounded." : "Player is not grounded.");

                            // Consume just one bullet
                            ammo--;

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
                                RecoilStateHandler();

                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("No bullets left!");
                    // You might want to reset shootPressCount here to ensure the next press attempts to shoot again.
                    shootPressCount = 0;
                }
            }

            else
            {
                Reload();
            }
        }

    }
    void RecoilStateHandler()
    {

        if (playerRef.currentState == PlayerScript.PlayerState.OnGround)
        {
            playerRef.DisableMovementForces();

            StartCoroutine(EnableMovementAfterDelay(0.5f));
        }
        else
        {
            playerRef.DisableMovementForces();

            StartCoroutine(EnableMovementAfterDelay(0.4f));
        }

            Vector3 direction = fpsCam.transform.forward;

            // Regular recoil
            Vector3 regularRecoil = -direction.normalized * recoilForce;

            // Enhanced recoil when moving or hopping in air
            float recoilMultiplier = IsPlayerMoving() ? 1.5f : 1.0f;
            Vector3 enhancedRecoil = -direction.normalized * recoilForce * recoilMultiplier;
            bool hitSomething = Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range);
            float currentGravityEffect = Vector3.Dot(playerRB.velocity, Vector3.up);
            Vector3 effectiveRecoilForce = -direction.normalized * (recoilForce - currentGravityEffect);
            // Determine if the shot hits a wall or the ground

            if (rayHit.collider != null && hitSomething)
            {
                if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))

                {
                    Debug.Log("[Tag 101] Shot the ground.");

                }
                else if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    Debug.Log("[Tag 102] Shot a wall.");
                }
                // Check Player's State
                switch (playerRef.currentState)
                {
                    case PlayerScript.PlayerState.OnGround:
                        if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                        }
                        else
                            playerRB.AddForce(regularRecoil, ForceMode.VelocityChange);

                        Debug.Log("Tag 01");
                        break;

                    case PlayerScript.PlayerState.Hopping:
                        if (!playerRef.onGround) // Hopping and not on ground
                        {
                            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                            {
                                playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                                Debug.Log("Tag 02-1");

                            }
                            else
                            {
                                playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                                playerRB.AddForce(regularRecoil * 0.25f, ForceMode.Impulse);
                                Debug.Log("Tag 02-2");

                            }
                        }
                        else
                        {
                            if (rayHit.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
                            {
                                Debug.Log("Tag 03-1");
                            playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                            playerRB.AddForce(regularRecoil * 0.25f, ForceMode.Impulse);
                        }
                            else
                            {
                                playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                                Debug.Log("Tag 03-2");
                            }
                        }
                        break;

                    case PlayerScript.PlayerState.OnAir:
                        if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            Debug.Log("Tag 04-1");
                            playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                        }
                        else
                        {
                            Debug.Log("Tag 04-2");
                        if (!IsPlayerMoving())
                        {
                            playerRB.AddForce(regularRecoil, ForceMode.Impulse);
                            playerRB.AddForce(regularRecoil*0.25f, ForceMode.VelocityChange);

                        }

                        else
                            {
                            playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                            playerRB.AddForce(regularRecoil * 0.25f, ForceMode.Impulse);
                        }
                        }



                        break;
                    case PlayerScript.PlayerState.Sprinting:
                        playerRB.AddForce(enhancedRecoil, ForceMode.Impulse);
                        playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                        Debug.Log("Tag 05");
                        break;
                    default:
                        Debug.Log("[Tag 06] Applying regular recoil in default case.");
                        playerRB.AddForce(regularRecoil, ForceMode.Impulse);
                        break;
                }
            }
            else
            {
                Debug.Log("Tag 07");
                // Here, you can handle the recoil behavior when you shoot the sky or other non-collider objects.
                // For instance, just apply the regular recoil.
                playerRB.AddForce(regularRecoil, ForceMode.Impulse);
                // playerRB.AddForce(enhancedRecoil * 0.15f, ForceMode.VelocityChange);

            }
     
    }

    IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerRef.EnableMovementForces();
    }
    private bool IsPlayerMoving()
    {
        return inputActions.input.Gameplay.Move.ReadValue<Vector2>() != Vector2.zero; // Adjust the threshold if necessary.
    }
    private void ResetShot()
    {
        reloading = false;
        // Reset shootPressCount to ensure the next press will attempt to shoot.
        shootPressCount = 0;
    }
    public void Reload()
    {
        if (!reloading)  // Prevent multiple reload triggers
        {
            reloading = true;
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