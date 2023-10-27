using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float rotY = 5;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotY, 0) * Time.deltaTime);
    }
}
