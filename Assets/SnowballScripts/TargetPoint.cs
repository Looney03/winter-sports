using UnityEngine;
using UnityEngine.UI;

public class TargetPoint : MonoBehaviour
{
    public GameObject crosshairPanel;

    void Start()
    {
        CreateCrosshair();
    }

    void CreateCrosshair()
    {
        float yOffset = 5f;


        GameObject verticalLine = new GameObject("VerticalLine");
        verticalLine.transform.SetParent(crosshairPanel.transform, false);
        Image verticalImage = verticalLine.AddComponent<Image>();
        verticalImage.color = Color.white;
        RectTransform vRect = verticalLine.GetComponent<RectTransform>();
        vRect.sizeDelta = new Vector2(2, 20);
        vRect.anchoredPosition = new Vector2(0, yOffset);


        GameObject horizontalLine = new GameObject("HorizontalLine");
        horizontalLine.transform.SetParent(crosshairPanel.transform, false);
        Image horizontalImage = horizontalLine.AddComponent<Image>();
        horizontalImage.color = Color.white;
        RectTransform hRect = horizontalLine.GetComponent<RectTransform>();
        hRect.sizeDelta = new Vector2(20, 2);
        hRect.anchoredPosition = new Vector2(0, yOffset);
    }
}