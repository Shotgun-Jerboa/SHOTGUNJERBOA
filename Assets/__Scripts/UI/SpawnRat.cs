using UnityEngine;

public class SpawnRat : MonoBehaviour
{
    public GameObject rat;
    public float spawnInterval;
    public Vector3 position;
    public Vector3 rotation;
    private float time = 0;
    public RatMovement movement;
    public bool right;
    public bool spawnRats = true;
    public GameObject[] signs;
    public int ymin = -32;
    public int ymax = -60;
    private float yLevel;
    public Canvas canvas;
    public RectTransform panel;

    public Camera mainCamera;
    public Vector3 pos;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

        if (!right)
        {
            position = GetBottomLeftCorner(panel) - new Vector3(panel.rect.x, Random.Range(0, panel.rect.y / 2), 0);
            rotation = new Vector3(0, 0, 0);
        }
        else
        {
            position = new Vector3(Screen.width, Random.Range(0, panel.rect.y / 2), 0);

            rotation = new Vector3(0, 180, 0);
        }
        time += Time.deltaTime;
        if (time >= spawnInterval)
        {
            if (spawnRats)
            {
                RatSpawn();
            }
            time -= spawnInterval;

        }
    }

    public void RatSpawn()
    {

        yLevel = UnityEngine.Random.Range((30 * canvas.transform.localScale.y), Screen.height / 2);
        position.y = yLevel;
        GameObject newRat = Instantiate(rat, position, Quaternion.Euler(rotation)) as GameObject;

        newRat.transform.SetParent(this.transform, false);
        if (yLevel > signs[4].transform.position.y - (30 * canvas.transform.localScale.y * signs[4].transform.localScale.y))
        {
            newRat.transform.SetParent(signs[4].transform);
        }
        else if (yLevel > signs[3].transform.position.y - (30 * canvas.transform.localScale.y * signs[3].transform.localScale.y))
        {
            newRat.transform.SetParent(signs[3].transform);
        }
        else if (yLevel > signs[2].transform.position.y - (30 * canvas.transform.localScale.y * signs[2].transform.localScale.y))
        {
            newRat.transform.SetParent(signs[2].transform);
        }
        else if (yLevel > signs[1].transform.position.y - (30 * canvas.transform.localScale.y * signs[1].transform.localScale.y))
        {
            newRat.transform.SetParent(signs[1].transform);
        }
        else if (yLevel > signs[0].transform.position.y - (30 * canvas.transform.localScale.y * signs[0].transform.localScale.y))
        {

            newRat.transform.SetParent(signs[0].transform);
        }
        else
        {
            newRat.transform.SetParent(this.transform, false);
        }
        movement = newRat.GetComponent<RatMovement>();
        if (right)
        {
            movement.onLeft = false;
            right = false;
        }
        else
        {
            movement.onLeft = true;
            right = true;
        }
        movement.yLevel = yLevel;
    }

    Vector3 GetBottomLeftCorner(RectTransform rt)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        return v[0];
    }
}
