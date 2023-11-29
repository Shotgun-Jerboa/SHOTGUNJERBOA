using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
  
    // Update is called once per frame
    void Update()
    {
        
        transform.rotation = Camera.main.transform.rotation;
    }
}
