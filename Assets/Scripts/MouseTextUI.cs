using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseTextUI : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;
    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;
    private bool isProductSelected;

    private void Start()
    {
        isProductSelected = false;
    }

    private void Awake()
    {
        backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.GetComponent<RectTransform>();

        SetText("Hello world");
    }

    private void SetText(string text)
    {
        textMeshPro.SetText(text);
        textMeshPro.ForceMeshUpdate();

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(8, 8);

        backgroundRectTransform.sizeDelta = textSize + paddingSize;
    }

    public void SetIsSelected(bool isSelected,string productionName)
    {
        isProductSelected = isSelected;

        this.gameObject.SetActive(isProductSelected);
        //backgroundRectTransform.gameObject.SetActive(isSelected);
        //textMeshPro.gameObject.SetActive(isSelected);

        if (isProductSelected) SetText(productionName);
    }

    private void Update()
    {
        if (isProductSelected)
            rectTransform.anchoredPosition = Input.mousePosition + new Vector3(10, 10, 0);
    }

}
