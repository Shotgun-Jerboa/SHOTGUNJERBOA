using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunTest : MonoBehaviour
{
    [SerializeField] SettingVars inputActions;// SerializeField makes the private field visible in the Unity editor. inputActions holds various input-related variables.

    [Header("Gun Stas")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int bulletsPerTap;
    [SerializeField] float recoilForce;


    [Header("Amount Stas")]
    public int ammo;

    [Header("Particle Effect")]
    [SerializeField] GameObject muzzleFlash; // Prefab for the muzzle flash effect.
    [SerializeField] GameObject BulletHoleEffect; // Prefab for the bullet hole effect.
    [SerializeField] TrailRenderer bulletTrail; // Prefab for the bullet trail effect.

    // These booleans are used to control the shooting and reloading states of the gun.
    bool readyToShoot, reloading;
    public bool isShooting = false;
    public bool isRecoil;


    [Header("Reference")]
    [SerializeField] Camera fpsCam; // Reference to the first-person camera.
    public Transform shootingPoint; // The point from where the bullets are shot.
    public RaycastHit rayHit; // Information about what the bullet hit.
    public LayerMask whatIsEnemy; // LayerMask to filter what the bullet can hit.
    Rigidbody playerRB; // Reference to the player's Rigidbody.
    PlayerScriptBackup playerRef; // Reference to the player's script.

    [Header("Gun Recoil Effect")]
    RecoilCameraShake cameraShake;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    [SerializeField] private float recoilAmount = 0.05f; // How much the gun moves up.
    [SerializeField] private float recoilRotationAmount = -2.0f; // How much the gun rotates (negative values for upwards rotation).
    [SerializeField] private float recoilRecoveryRate = 1.5f; // Rate at which gun returns to original position.
    [SerializeField] float returnDuration;

    [SerializeField] Animator animator;

    CrossHairManager crossHairManager;
    //[SerializeField] float impactForce;

    //USE THIS TO CHECK THE NUMBER OF MOUSE PRESS TO SHOOT
    private int shootPressCount = 0;

    private float lastShotTime;
    public enum InputMethod // Enumeration of possible input methods.
    {
        None,
        LeftClick,
        RightClick,
    }

    [SerializeField] InputMethod inputMethod;
    void Awake()
    {

        readyToShoot = true; // Initialize the gun as ready to shoot.
        shootPressCount = 2;

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
        playerRef = GameObject.Find("Player").GetComponent<PlayerScriptBackup>();
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RecoilCameraShake>();

        crossHairManager = Global.instance.sceneTree.Get("Canvas").GetComponent<CrossHairManager>();

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private bool hasShot = false;

    private void Shoot(InputMethod inputMethod)
    {
        //Only Shoot when not reloading
        if (!reloading)
        {
            hasShot = true;
            shootPressCount++;

            if (shootPressCount % 2 != 0)// if current press index is 2 then we shoot
            {
                if (ammo > 0)
                {
                    // Calculate the time since the last shot
                    float timeSinceLastShot = Time.time - lastShotTime;

                    if (inputMethod == InputMethod.LeftClick)
                    {
                        crossHairManager.ChangeCrosshair(false, true); // Left gun not loaded
                    }
                    else if (inputMethod == InputMethod.RightClick)
                    {
                        crossHairManager.ChangeCrosshair(false, false); // Right gun not loaded
                    }

                    if (timeSinceLastShot >= timeBetweenShooting)
                    {
                        // Update the last shot time
                        lastShotTime = Time.time;
                        Vector3 direction = fpsCam.transform.forward;
                        if (inputMethod == InputMethod.LeftClick ||
                            (inputMethod == InputMethod.RightClick))
                        {
                            //Camera Shake when shoot
                            cameraShake.ShakeCamera();
                            ApplyRecoil();
                            //Cross hair Effect
                            if (inputMethod == InputMethod.LeftClick)
                            {
                                crossHairManager.LeftCrossHairInteraction();
                            }
                            else
                            {
                                crossHairManager.RigthCrossHairInteraction();
                            }
                            // Consume just one bullet
                            ammo--;

                            GameObject MuzzleFlashInstance = Instantiate(muzzleFlash, shootingPoint.position, Quaternion.identity);
                            Destroy particleScript = MuzzleFlashInstance.GetComponent<Destroy>();
                            particleScript.attackPos = shootingPoint;

                            for (int i = 0; i < bulletsPerTap; i++)
                            {
                                float x = Random.Range(-spread, spread);
                                float y = Random.Range(-spread, spread);

                                // Creating a direction vector with the spread applied in the camera's local space
                                Vector3 spreadDirection = new Vector3(x, y, 1);
                                spreadDirection.Normalize();

                                Vector3 bulletDirection = fpsCam.transform.TransformDirection(spreadDirection);

                                if (Physics.Raycast(fpsCam.transform.position, bulletDirection, out rayHit, range, whatIsEnemy))
                                {
                                    TrailRenderer trail = Instantiate(bulletTrail, shootingPoint.position, Quaternion.identity);
                                    StartCoroutine(SpawnTrail(trail, rayHit));

                                    if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                                    {
                                        Quaternion impactRotation = Quaternion.LookRotation(rayHit.normal);
                                        GameObject impact = Instantiate(BulletHoleEffect, rayHit.point, impactRotation);
                                        impact.transform.parent = rayHit.transform;
                                        Debug.Log("Bullet Hole");
                                    }
                                }
                            }

                            if (isRecoil)
                            {
                                RecoilStateHandler();
                                //We disable the movement force for a split second so the recoil force went through
                                if (playerRef.onGround)
                                {
                                    playerRef.DisableMovementForces();
                                    //We activate the movement force after recoil went through
                                    StartCoroutine(EnableMovementAfterDelay(0.5f));
                                }
                                else
                                {
                                    playerRef.DisableMovementForces();
                                    StartCoroutine(EnableMovementAfterDelay(0.5f));
                                }
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
    private float GetReloadAnimationDuration()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Reload") // Replace with your animation's actual name
            {
                return clip.length;
            }
        }
        return 0f;
    }

    void ApplyRecoil()
    {
        // Move gun upwards.
        transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, recoilAmount, 0), 0.5f);

        // Rotate gun upwards.
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(transform.localEulerAngles + new Vector3(recoilRotationAmount, 0, 0)), 0.5f);

        // Start the recovery coroutine.
        StartCoroutine(RecoverFromRecoil());
    }
    IEnumerator RecoverFromRecoil()
    {
        while (Vector3.Distance(transform.localPosition, originalPosition) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, recoilRecoveryRate * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, recoilRecoveryRate * Time.deltaTime);
            yield return null;
        }

        float returnElapsed = 0.0f;
        Vector3 startingPosition = transform.localPosition;

        while (returnElapsed < returnDuration)
        {
            returnElapsed += Time.deltaTime;
            float normalizedTime = returnElapsed / returnDuration;

            // Interpolate position and rotation based on elapsed time to give a smooth return
            transform.localPosition = Vector3.Lerp(startingPosition, originalPosition, normalizedTime);

            yield return null;
        }
        // Snap back to the exact original position and rotation for precision.
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    void RecoilStateHandler()
    {


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
                case PlayerScriptBackup.PlayerState.Gameplay_Ground:
                    if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                        playerRB.AddForce(regularRecoil * 0.1f, ForceMode.VelocityChange);

                        Debug.Log("Tag 01-1");
                    }
                    else
                    {
                        //The drag doesn't work for this case so add more force
                        playerRB.AddForce(enhancedRecoil * 2.25f, ForceMode.VelocityChange);
                        playerRB.AddForce(regularRecoil, ForceMode.Impulse);
                        Debug.Log("Tag 01-2");
                    }

                    break;

                case PlayerScriptBackup.PlayerState.Gameplay_Hopping:
                    if (!playerRef.onGround) // Hopping and not on ground
                    {
                        if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                            playerRB.AddForce(regularRecoil * 0.1f, ForceMode.VelocityChange);
                            Debug.Log("Tag 02-1");

                        }
                        else
                        {
                            playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                            playerRB.AddForce(regularRecoil*2f, ForceMode.Impulse);
                            Debug.Log("Tag 02-2");

                        }
                    }
                    else
                    {
                        if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        {
                            Debug.Log("Tag 03-1");
                            playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                            playerRB.AddForce(regularRecoil, ForceMode.Impulse);
                        }
                        else
                        {
                            playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                            playerRB.AddForce(regularRecoil * 0.1f, ForceMode.VelocityChange);
                            Debug.Log("Tag 03-2");
                        }
                    }
                    break;

                case PlayerScriptBackup.PlayerState.Gameplay_Air:
                    if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {

                        Debug.Log("Tag 04-1");
                        playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                        playerRB.AddForce(regularRecoil * 0.1f, ForceMode.VelocityChange);

                    }
                    else
                    {
                        if (!IsPlayerMoving())
                        {
                            playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                            playerRB.AddForce(regularRecoil*2, ForceMode.VelocityChange);
                            Debug.Log("Tag 04-2");

                        }

                        else
                        {
                            playerRB.AddForce(enhancedRecoil, ForceMode.VelocityChange);
                            playerRB.AddForce(regularRecoil*2, ForceMode.Impulse);
                            Debug.Log("Tag 04-3");

                        }
                    }



                    break;
                case PlayerScriptBackup.PlayerState.Gameplay_Sprinting:
                    if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        playerRB.AddForce(effectiveRecoilForce, ForceMode.Impulse);
                        playerRB.AddForce(regularRecoil * 0.1f, ForceMode.VelocityChange);
                        Debug.Log("Tag 05-1");
                    }
                    else
                    {
                        playerRB.AddForce(enhancedRecoil, ForceMode.Impulse);
                        playerRB.AddForce(enhancedRecoil*2, ForceMode.VelocityChange);
                        Debug.Log("Tag 05-1");
                    }

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
    public void Reload()
    {
        if (!reloading)  // Prevent multiple reload triggers
        {
            reloading = true;
            animator.SetTrigger("Reload");

            float reloadDuration = GetReloadAnimationDuration();
            if (reloadDuration > 0)
            {
                Invoke("FinishReload", reloadDuration);
            }
            else
            {
                Debug.LogWarning("Could not find reload animation duration. Using default reload time.");
                Invoke("FinishReload", reloadTime);
            }
        }
    }
    private void FinishReload()
    {
        reloading = false;
        shootPressCount = 0;

        // Assuming you have a reference to your CrossHairManager script
        // And you know whether it is the left or right gun
        bool isLeftGun = (inputMethod==InputMethod.LeftClick);

        // Change the crosshair back to "loaded" state when the gun is reloaded
        crossHairManager.ChangeCrosshair(true, isLeftGun);
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
