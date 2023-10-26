using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class matGeomApplier : MonoBehaviour
{
    public PhysicMaterial material;

    void Start()
    {
        applyMaterial(gameObject);
    }

    private void applyMaterial(GameObject obj){
        MeshCollider collider;
        if(obj.TryGetComponent(out collider)){
            collider.material = material;
        }
        
        if(obj.transform.childCount != 0){
            for(int i = 0; i < obj.transform.childCount; i++){
                applyMaterial(obj.transform.GetChild(i).gameObject);
            }
        }
    }
}
