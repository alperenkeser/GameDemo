using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    public GameObject ObjectInformationPanel;
    public Text InformationText;
    public Image InformationImage;
    
    public GameObject ProductionPanel;
    public Button ProductionButton;

    public void Start(){
        ObjectInformationPanel.SetActive(false);
        ProductionPanel.SetActive(false);
    }

    public void isInformationAvailable(bool isAvailable)
    {
        ObjectInformationPanel.SetActive(isAvailable);
    }

    public void SetInformationText(string Text){
        InformationText.text = Text;
    }

    public void SetInformationSprite(Sprite sprite){
        InformationImage.sprite = sprite;
    }

    public void SetProductionSprite(Sprite sprite){
        ProductionButton.image.sprite = sprite;
    }

    public void isProductionAvailable(bool isAvailable)
    {
        ProductionPanel.SetActive(isAvailable);
    }
}
