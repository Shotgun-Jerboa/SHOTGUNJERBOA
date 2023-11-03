using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBasicShotgun : MonoBehaviour, IShotgun
{
    [Header("Gun Stats")]
    public int gunDamage;
    public int clip;
    public int ammoPerShot;
    public int maxClipSize;
    public float forceRecoil;
    public float reloadTime;
    public float bulletsPerShot;
    public float spread;
    public float maxRange;
    public float impulseForce; //This is for knockback force when the Enemy is defeated

    public LayerMask interactWith;

[Header("Effects")]
    [SerializeField] GameObject muzzleFlash; // Prefab for the muzzle flash effect.
    [SerializeField] GameObject BulletHoleEffect; // Prefab for the bullet hole effect.
    [SerializeField] TrailRenderer bulletTrail; // Prefab for the bullet trail effect.

    private Transform shootPoint;
    private bool isReloading = false;

    public void empty(ref int ammo)
    {
        throw new System.NotImplementedException();
    }

    public GameObject getObj()
    {
        return gameObject;
    }

    public float[] getStats()
    {
        return new float[] { clip, maxClipSize };
    }

    public bool isReady()
    {
        return clip >= ammoPerShot;
    }

    public void reload(ref int ammo)
    {
        if (clip < maxClipSize && !isReloading)
        {
            int newClip;

            if (ammo < maxClipSize)
            {
                newClip = ammo;
                ammo -= newClip;
            }
            else
            {
                ammo -= maxClipSize - clip;
                newClip = maxClipSize;
            }

            IEnumerator waitForReload()
            {
                bool checkIsReloading()
                {
                    return isReloading;
                }

                System.Action<Global.Interpolate, object[]> func = (parent, args) =>
                {
                    parent.elapsedTime += Time.fixedDeltaTime;

                    DebugBasicShotgun self = (DebugBasicShotgun)args[0];
                    Transform selfTransform = self.gameObject.transform;

                    selfTransform.Rotate(
                        parent.curve.Evaluate(parent.elapsedTime - Time.fixedDeltaTime) - parent.curve.Evaluate(parent.elapsedTime), 
                        0, 
                        0
                    );

                    if(parent.elapsedTime >= parent.curve[parent.curve.length - 1].time)
                    {
                        self.isReloading = false;
                    }
                };

                Global.instance.interpolate.Add(new Global.Interpolate(
                  new AnimationCurve(new Keyframe[]
                  {
                      new Keyframe()
                      {
                          value = 0,
                          time = 0
                      },
                      new Keyframe()
                      {
                          value = 360,
                          time = reloadTime
                      }
                  }), func, args: new object[] { this }
                ));

                isReloading = true;
                yield return new WaitWhile(checkIsReloading);

                clip = newClip;
            }

            StartCoroutine(waitForReload());
        }
    }


    public void shoot(ref Transform camera, ref int ammo, int? useAmmo = null)
    {
        int ammoUsed;
        if(useAmmo is not null)
        {
            ammoUsed = (int)useAmmo;
        } else
        {
            ammoUsed = ammoPerShot;
        }

        GameObject MuzzleFlashInstance = Instantiate(muzzleFlash, shootPoint);
        muzzleFlash.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Destroy particleScript = MuzzleFlashInstance.GetComponent<Destroy>();
        particleScript.attackPos = shootPoint;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 spreadDirection = new Vector3(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                1
            );

            IEnumerator SpawnTrailRayhit(TrailRenderer trail, RaycastHit hit)
            {
                float time = 0;
                Vector3 startPos = trail.transform.position;
                while (time < Vector3.Distance(hit.point, shootPoint.position) / 300)
                {
                    trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
                    time += Time.deltaTime;
                    yield return null;
                }
                trail.transform.position = hit.point;
                Destroy(trail.gameObject, trail.time);
            }
            IEnumerator SpawnTrailVector(TrailRenderer trail, Vector3 vector, Quaternion camRotation)
            {
   
                float time = 0;
                Vector3 startPos = trail.transform.position;
                Vector3 endPos = camRotation * vector * maxRange;
                while (time < maxRange / 300)
                {
                    trail.transform.position = Vector3.Lerp(startPos, endPos, time);
                    time += Time.deltaTime;
                    yield return null;
                }
                trail.transform.position = endPos;
                Destroy(trail.gameObject, trail.time);
            }

            RaycastHit hit;
            if (Physics.Raycast(camera.position, camera.rotation * spreadDirection, out hit, maxRange, interactWith))
            {
                if(hit.distance > maxRange)
                {
                    TrailRenderer trail = Instantiate(bulletTrail, shootPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrailVector(trail, spreadDirection, camera.rotation));
                } else
                {
                    TrailRenderer trail = Instantiate(bulletTrail, shootPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrailRayhit(trail, hit));

                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                            GameObject impact = Instantiate(BulletHoleEffect, hit.point, impactRotation);
                            impact.transform.parent = hit.transform;
                        }

                        //ENEMY INTERACTION WHEN BEING SHOT
                        //PARTICLE EFFECT, DAMAGE DEALTH, EFFECTS 
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            //DEAL DAMAGE TO ENEMY
                            hit.collider.gameObject.GetComponent<EnemyAI>().TakeDamage(gunDamage);

                            Rigidbody enemyRb = hit.collider.gameObject.GetComponent<Rigidbody>();
                            if (enemyRb != null)
                            {
                                if (hit.collider.gameObject.GetComponent<EnemyAI>().health <=0 && hit.collider.gameObject.GetComponent<EnemyAI>().isDefeated)
                                {
                                    Vector3 forceDirection = (hit.point - camera.position).normalized; // Calculate direction of the force
                                    enemyRb.AddForce(forceDirection * impulseForce, ForceMode.Impulse); // Apply the force as an impulse
                                }
                            }
                            // IMPACT EFFECT ON ENEMY
                                Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                            GameObject impact = Instantiate(BulletHoleEffect, hit.point, Quaternion.identity);
                            impact.transform.parent = hit.transform;
                        }
                    }
                   
                }
            }
            else
            {
                TrailRenderer trail = Instantiate(bulletTrail, shootPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrailVector(trail, spreadDirection, camera.rotation));
            }
        }

        PlayerScript playerRef = Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>();

        RaycastHit mainHit;
        Ray ray = new(camera.position, camera.rotation * Vector3.forward);
        if(Physics.Raycast(ray, out mainHit, maxRange, interactWith)){
            if(mainHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                playerRef.physbody.AddForce(
                    camera.rotation * -Vector3.forward * forceRecoil * 
                    (1.5f - (Mathf.Clamp(mainHit.distance, 0.01f, 20f) / 20f))
               );
            }
            else
            {
                playerRef.physbody.AddForce(camera.rotation * -Vector3.forward * forceRecoil * 0.75f);
            }
        } else
        {
            playerRef.physbody.AddForce(camera.rotation * -Vector3.forward * forceRecoil * 0.5f);
        }

        //camera.GetComponent<CamScript>().shake();

        clip -= ammoUsed;

        if(clip == 0)
        {
            reload(ref ammo);
        }
    }

    void Start()
    {
        Global.SceneHeirarchy children = new Global.SceneHeirarchy(gameObject);
        shootPoint = children.Get("ShootPoint").transform;

        clip = maxClipSize;

        if(maxClipSize < ammoPerShot)
        {
            Debug.LogError("ammoPerShot cannot exceed maxClipSize");
            Global.Quit();
        }
    }
}
