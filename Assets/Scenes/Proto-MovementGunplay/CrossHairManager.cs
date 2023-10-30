using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairManager : MonoBehaviour
{
    private GameObject leftCrossHairFull;
    private GameObject rightCrossHairFull;
    private GameObject leftCrossHairEmpty;
    private GameObject rightCrossHairEmpty;

    //[SerializeField] GameObject spriteLeftCrossHair;
    //[SerializeField] GameObject spriteRightCrossHair;
    // Duration for how long the crosshair move should last
    public float duration = 0.5f;

    // How far the crosshair move
    public float intensity = 0.1f;

    // Time it takes for the crosshair to smoothly return to its original position
    public float returnDuration = 0.5f;

    private Vector3 leftOriginalPosition;
    private Vector3 rightOriginalPosition;

    [SerializeField] Sprite loadedLeftCrossHair; // Sprite when the left gun is loaded
    [SerializeField] Sprite loadedRightCrossHair; // Sprite when the right gun is loaded

    [SerializeField] Sprite emptyLeftCrossHair;// Sprite when the left gun is empty
    [SerializeField] Sprite emptyRightCrossHair;// Sprite when the right gun is empty

    private void Start()
    {
        leftCrossHairFull = Global.instance.sceneTree.Get("Canvas/Left_CrosshairFull");
        rightCrossHairFull = Global.instance.sceneTree.Get("Canvas/Right_CrosshairFull");
        leftCrossHairEmpty = Global.instance.sceneTree.Get("Canvas/Left_CrosshairEmpty");
        rightCrossHairEmpty = Global.instance.sceneTree.Get("Canvas/Right_CrosshairEmpty");
    }
    private void Awake()
    {


       // leftOriginalPosition = leftCrossHair.transform.position;
        //rightOriginalPosition = rightCrossHair.transform.position;
    }

    public void LeftCrossHairInteraction()
    {
        StartCoroutine(MoveCrossHair(leftCrossHairFull, -intensity, duration));
        StartCoroutine(MoveCrossHair(leftCrossHairEmpty, -intensity, duration));
    }

    public void RigthCrossHairInteraction()
    {
        StartCoroutine(MoveCrossHair(rightCrossHairFull, intensity, duration));
        StartCoroutine(MoveCrossHair(rightCrossHairEmpty, intensity, duration));
    }

    IEnumerator MoveCrossHair(GameObject crossHair, float direction, float moveDuration)
    {
        float elapsedTime = 0;
        Vector3 startPosition = crossHair.transform.position;

        while (elapsedTime < moveDuration)
        {
            float xPosition = Mathf.Lerp(startPosition.x, startPosition.x + direction, elapsedTime / moveDuration);
            crossHair.transform.position = new Vector3(xPosition, startPosition.y, startPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        crossHair.transform.position = new Vector3(startPosition.x + direction, startPosition.y, startPosition.z);

        //Return the cross hair back to the center
        StartCoroutine(ReturnCrosshair(crossHair, startPosition, returnDuration));

    }

    IEnumerator ReturnCrosshair(GameObject crossHair, Vector3 startPosition, float returnDuration)
    {
        float elapsedTime = 0;

        while (elapsedTime < returnDuration)
        {
            float xPosition = Mathf.Lerp(crossHair.transform.position.x, startPosition.x, elapsedTime / returnDuration);
            crossHair.transform.position = new Vector3(xPosition, startPosition.y, startPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        crossHair.transform.position = startPosition;
    }

    public void ChangeCrosshair(float clipPercent, bool isLeftgun)
    {
        Image fullImage;
        Image emptyImage;

        if (isLeftgun)
        {
            fullImage = leftCrossHairFull.GetComponent<Image>();
            emptyImage = leftCrossHairEmpty.GetComponent<Image>();
        } else
        {
            fullImage = rightCrossHairFull.GetComponent<Image>();
            emptyImage = rightCrossHairEmpty.GetComponent<Image>();
        }

        fullImage.color = new Color(
            fullImage.color.r,
            fullImage.color.g,
            fullImage.color.b,
            clipPercent
        );

        emptyImage.color = new Color(
            emptyImage.color.r,
            emptyImage.color.g,
            emptyImage.color.b,
            1f - clipPercent
        );

        //GameObject crossHair = isLeftgun ? leftCrossHairFull : rightCrossHairFull;
        //Sprite loadedSprite = isLeftgun ? loadedLeftCrossHair : loadedRightCrossHair;
        //Sprite emptySprite = isLeftgun ? emptyLeftCrossHair : emptyRightCrossHair;

        //Image crossHairImage = crossHair.GetComponent<Image>(); // Assuming the GameObject has an Image component
        //if (crossHairImage != null)
        //{
        //    crossHairImage.sprite = isLoaded ? loadedSprite : emptySprite;
        //}
        //else
        //{
        //    Debug.LogWarning("No Image component found in the crosshair GameObject.");
        //}
    }
}
