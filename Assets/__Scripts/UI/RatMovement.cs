using UnityEngine;

public class RatMovement : MonoBehaviour
{
    public float speed = 45;
    public bool onLeft = true;
    public float yLevel;
    private int setX = -40;
    public Vector3 movement;
    public RectTransform canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = this.transform.root.GetComponent<RectTransform>();
        speed *= canvas.transform.localScale.x;
        movement = transform.position;
        if (!onLeft)
        {
            speed *= -1;
            setX = Screen.width;
        }
        transform.position = new Vector3(setX, yLevel, 0);
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(speed * Time.deltaTime, 0, 0);
        transform.position = transform.position + movement;

        if (onLeft)
        {
            if (transform.position.x > Screen.width)
            {
                Destroy(this.gameObject);
            }
        }
        else if (!onLeft)
        {
            if (transform.position.x < -40)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
