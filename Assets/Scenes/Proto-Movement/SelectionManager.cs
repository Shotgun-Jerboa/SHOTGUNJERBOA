using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    
    [SerializeField] private string buttonTag = "Button";
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    public bool ButtonPressed;

    public WallHealthScript wallScript;
    public PlayerScript playerRef;

    public Animator anim;
    public Animator buttonAnim;
    
    // Start is called before the first frame update
    void Start()
    {
        Animator anim = GetComponentInParent<Animator>();
        Animator buttonAnim = GetComponentInParent<Animator>();

        ButtonPressed = false;
    }

    private Transform _selection;
    
    // Update is called once per frame
    void Update()
    {
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;
            if (selection.CompareTag(wallTag))
            {
                if ((Input.GetMouseButtonDown(0) | Input.GetMouseButtonDown(1)) /*& (Ammo >= 1)*/)
                {
                    wallScript.DamageWall();
                }
            }
            if (selection.CompareTag(buttonTag))
            {
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightMaterial;
                }
                
                
                if ((Input.GetMouseButtonDown(0) | Input.GetMouseButtonDown(1)) /*& (Ammo >= 1)*/)           //if button is clicked and ammo is left

                {

                    ButtonPressed = true;

                    //gameObject.GetComponent<AudioSource>().Play();

                    //gameObject.GetComponent<Animator>().Play("switch");
                }
                
                if (ButtonPressed)
                {
                    anim.SetBool("ButtonPressed", true);
                    buttonAnim.SetBool("ButtonPressed", true);
                }

                _selection = selection;
            }
        }
    }
}
