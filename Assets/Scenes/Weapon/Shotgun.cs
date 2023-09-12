using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] SettingVars inputActions;

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

    [SerializeField] float recoilForce;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] float impactForce;
    private bool isShooting = false;

    private void Awake()
    {
        readyToShoot = true;

        inputActions.input.Gameplay.RightHandPressed.performed += ctx => Shoot();
        inputActions.input.Gameplay.RightHandReleased.performed += ctx => isShooting = false;
    }

    private void Start()
    {
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.Log(isShooting);
       
    }
    private void Shoot()
    {
        if (readyToShoot && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            readyToShoot = false;

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
                playerRB.AddForce(-direction.normalized * recoilForce, ForceMode.VelocityChange);
            }
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
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
