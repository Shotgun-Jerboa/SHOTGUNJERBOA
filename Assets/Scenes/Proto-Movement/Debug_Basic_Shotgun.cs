using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugBasicShotgun : MonoBehaviour, IShotgun
{
    [Header("Gun Stats")]
    public int ammoPerShot;
    public int maxClipSize;
    public float forceRecoil;
    public float reloadTime;
    public float bulletsPerShot;
    public float spread;
    public float maxRange;

    [Header("Effects")]
    [SerializeField] GameObject muzzleFlash; // Prefab for the muzzle flash effect.
    [SerializeField] GameObject BulletHoleEffect; // Prefab for the bullet hole effect.
    [SerializeField] TrailRenderer bulletTrail; // Prefab for the bullet trail effect.

    private Transform shootPoint;
    private int clip;
    private bool isReloading = false;

    public void empty(ref int ammo)
    {
        throw new System.NotImplementedException();
    }

    public GameObject getObj()
    {
        return gameObject;
    }

    public bool isReady()
    {
        return clip >= ammoPerShot;
    }

    public void reload(ref int ammo)
    {
        if (clip < maxClipSize)
        {
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

                    selfTransform.rotation = Quaternion.Euler(parent.curve.Evaluate(parent.elapsedTime), 0, 0);

                    if(parent.elapsedTime >= parent.curve[parent.curve.length].time)
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
            }

            StartCoroutine(waitForReload());

            if(ammo < maxClipSize)
            {
                clip = ammo;
                ammo -= clip;
            }
            else
            {
                ammo -= (maxClipSize - clip);
                clip = maxClipSize;
            }
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
            if (Physics.Raycast(camera.position, camera.rotation * spreadDirection, out hit))
            {
                if(hit.distance > maxRange)
                {
                    Vector3 maxTrailPosition = camera.transform.position + (camera.rotation * spreadDirection);
                    TrailRenderer trail = Instantiate(bulletTrail, shootPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrailVector(trail, spreadDirection, camera.rotation));
                } else
                {
                    TrailRenderer trail = Instantiate(bulletTrail, shootPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrailRayhit(trail, hit));

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                        GameObject impact = Instantiate(BulletHoleEffect, hit.point, impactRotation);
                        impact.transform.parent = hit.transform;
                    }
                }
            }
            else
            {
                Vector3 maxTrailPosition = camera.transform.position + (camera.rotation * spreadDirection);
                TrailRenderer trail = Instantiate(bulletTrail, shootPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrailVector(trail, spreadDirection, camera.rotation));
            }
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
            Global.instance.Quit();
        }
    }
}
