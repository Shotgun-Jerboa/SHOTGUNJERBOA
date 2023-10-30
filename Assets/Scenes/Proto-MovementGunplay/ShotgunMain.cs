using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShotgunMain : MonoBehaviour
{
    public int ammo;
    public bool infiniteAmmo = false;

    private SettingVars settings;
    private PlayerScript player;
    private IShotgun[] shotguns;
    private new Transform camera;
    private CrossHairManager crosshairManager;

    public enum gunHand {
        right,
        left
    }

    void Start()
    {
        settings = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();
        player = Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>();
        camera = Global.instance.sceneTree.Get("Camera/Main Camera").transform;
        crosshairManager = Global.instance.sceneTree.Get("Canvas").GetComponent<CrossHairManager>();
        updateGuns();
    }

    void Update()
    {
        if (infiniteAmmo)
        {
            ammo = 0x7FFFFFFF;
        }

        if(settings.input.Gameplay.MenuOPEN.WasPressedThisFrame()){
            player.state = PlayerScript.PlayerState.UI_Pause;
        } else if(settings.input.UI.MenuCLOSE.WasPressedThisFrame()){
            if(player.onGround){
                player.state = PlayerScript.PlayerState.Gameplay_Ground;
            } else {
                player.state = PlayerScript.PlayerState.Gameplay_Air;
            }
        }

        switch (player.state)
        {
            case PlayerScript.PlayerState.UI_Pause:
                break;
            case PlayerScript.PlayerState.Gameplay_Air:
            case PlayerScript.PlayerState.Gameplay_Ground:
                if (shotguns.Length > 1)
                {
                    if (settings.input.Gameplay.LeftShoot.WasPressedThisFrame())
                    {
                        shoot(gunHand.left);
                    }
                    if (settings.input.Gameplay.RightShoot.WasPressedThisFrame())
                    {
                        shoot(gunHand.right);
                    }
                }
                else if (shotguns.Length == 1)
                {
                    if (
                        settings.input.Gameplay.LeftShoot.WasPressedThisFrame() ||
                        settings.input.Gameplay.RightShoot.WasPressedThisFrame()
                    )
                    {
                        shoot();
                    }
                }
                break;
        }

        for(int i = 0; i < shotguns.Length; i++)
        {
            crosshairManager.ChangeCrosshair(
                shotguns[i].getStats()[0] / shotguns[i].getStats()[1],
                i == 1
            );
        }
    }

    public void shoot(int gunHand = 0)
    {
        if (shotguns[gunHand].isReady())
        {
            Rigidbody physbody = player.physbody;

            if (player.state == PlayerScript.PlayerState.Gameplay_Ground && Vector3.Dot(camera.rotation * Vector3.forward, physbody.velocity) > 0 && Vector3.Dot(camera.rotation * Vector3.forward, physbody.velocity) + player.physbody.velocity.y >= 1f)
            {
                physbody.velocity = new(
                    0,
                    player.physbody.velocity.y,
                    0
                );
            }

            if (player.state == PlayerScript.PlayerState.Gameplay_Ground || player.physbody.velocity.y < 0)
            {
                physbody.velocity = new(
                    player.physbody.velocity.x,
                    0,
                    player.physbody.velocity.z
                );
            }

            player.physbody.drag = 0;
            player.state = PlayerScript.PlayerState.Gameplay_Air;
            shotguns[gunHand].shoot(ref camera, ref ammo);

            if(shotguns.Length > 1)
            {
                if (gunHand == 1)
                {
                    crosshairManager.LeftCrossHairInteraction();
                }
                else
                {
                    crosshairManager.RigthCrossHairInteraction();
                }
            } else
            {
                crosshairManager.LeftCrossHairInteraction();
                crosshairManager.RigthCrossHairInteraction();
            }
        } else {
            if(ammo > 0){
                shotguns[gunHand].reload(ref ammo);
            }
        }
    }
    public void shoot(gunHand gunHand)
    {

        if (shotguns[(int)gunHand].isReady())
        {
            Rigidbody physbody = player.physbody;

            if (player.state == PlayerScript.PlayerState.Gameplay_Ground && Vector3.Dot(camera.rotation * Vector3.forward, physbody.velocity) > 0 && Vector3.Dot(camera.rotation * Vector3.forward, physbody.velocity) + player.physbody.velocity.y >= 1f)
            {
                physbody.velocity = new(
                    0,
                    player.physbody.velocity.y,
                    0
                );
            }

            if (player.state == PlayerScript.PlayerState.Gameplay_Ground || player.physbody.velocity.y < 0)
            {
                physbody.velocity = new(
                    player.physbody.velocity.x,
                    0,
                    player.physbody.velocity.z
                );
            }

            player.physbody.drag = 0;
            player.state = PlayerScript.PlayerState.Gameplay_Air;
            shotguns[(int)gunHand].shoot(ref camera, ref ammo);

            if (shotguns.Length > 1)
            {
                if (gunHand == gunHand.left)
                {
                    crosshairManager.LeftCrossHairInteraction();
                }
                else
                {
                    crosshairManager.RigthCrossHairInteraction();
                }
            }
            else
            {
                crosshairManager.LeftCrossHairInteraction();
                crosshairManager.RigthCrossHairInteraction();
            }
        } else {
            if(ammo > 0){
                shotguns[(int)gunHand].reload(ref ammo);
            }
        }
    }

    public void updateGuns(bool instance=true)
    {
        if (instance)
        {
            shotguns = transform.GetComponentsInChildren<IShotgun>();

            if (shotguns.Length > 2)
            {
                int i = shotguns.Length;
                while (i > 2)
                {
                    Destroy(shotguns[i - 1].getObj());
                    i--;
                }
                updateGuns();
                return;
            }
        }

        if(shotguns.Length > 0)
        {
            shotguns[0].getObj().transform.position = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons/RHandPointer").transform.position;

            if(shotguns.Length > 1)
            {
                shotguns[1].getObj().transform.position = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons/LHandPointer").transform.position;
            }
        }
    }

    public void swap()
    {
        if(shotguns.Length > 0)
        {
            shotguns[1].getObj().transform.SetSiblingIndex(shotguns[0].getObj().transform.GetSiblingIndex());
            updateGuns();
        }
    }

    public void addGun(string weapon_id, int? hand=null)
    {
        GameObject gunObj = Global.instance.sceneTree.Get($"Weapons/{weapon_id}");
        if(gunObj is not null)
        {
            IShotgun justIgnoreThis;
            if (gunObj.TryGetComponent(out justIgnoreThis))
            {
                GameObject instance = Instantiate(gunObj, transform);
                instance.name = weapon_id;
                instance.SetActive(true);
                updateGuns();
                if (hand is not null)
                {
                    if(hand > 0)
                    {
                        swap();
                    }
                }
            } else
            {
                Debug.LogError($"Could not find gun derived script in {gunObj.name}");
            }
        } else
        {
            Debug.LogError($"{weapon_id} could not be found under Weapons");
        }
    }
    public void addGun(string weapon_id, gunHand hand)
    {
        GameObject gunObj = Global.instance.sceneTree.Get($"Weapons/{weapon_id}");
        if (gunObj is not null)
        {
            IShotgun justIgnoreThis;
            if (gunObj.TryGetComponent(out justIgnoreThis))
            {
                GameObject instance = Instantiate(gunObj, transform);
                instance.name = weapon_id;
                instance.SetActive(true);
                updateGuns();

                if ((int)hand > 0)
                {
                    swap();
                }
            }
            else
            {
                Debug.LogError($"Could not find gun derived script in {gunObj.name}");
            }
        }
        else
        {
            Debug.LogError($"{weapon_id} could not be found under Weapons");
        }
    }

    public void removeGun(int hand)
    {
        if(shotguns.Length >= hand)
        {
            Destroy(shotguns[hand].getObj());
            updateGuns();
        }
    }
    public void removeGun(string weapon_id, bool removeAllInstancesOf=false)
    {
        for(int i = 0; i < shotguns.Length; i++)
        {
            string name = shotguns[i].getObj().name;
            if (name.Equals(weapon_id))
            {
                Destroy(shotguns[i].getObj());

                if(!removeAllInstancesOf)
                {
                    break;
                }
            }
        }
        updateGuns();
    }
}

interface IShotgun
{
    bool isReady();
    GameObject getObj();
    void shoot(ref Transform camera, ref int ammo, int? useAmmo=null);
    void reload(ref int ammo);
    void empty(ref int ammo);
    float[] getStats();
}
