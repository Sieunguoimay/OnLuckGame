using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MCQAnswerItem : MonoBehaviour
{
    private Button UiButton;
    private Image UiImage;
    public Text UiText;
    public int Index;
    public char Choice { private set; get; }
    // Start is called before the first frame update
    void Awake()
    {
        Choice = (char)(65+Index);
        UiImage = GetComponent<Image>();
        UiButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetAnswer(string answer)
    {
        UiText.text = Choice +":"+ answer;
    }
    public string GetAnswer()
    {
        return UiText.text.Substring(3, UiText.text.Length-3);
    }
    public void SetCorrectColor()
    {
        //ColorBlock color = UiButton.colors;
        //color.selectedColor = new Color32(99, 202, 36,255); ;
        //UiButton.colors = color;
        //UiButton.Select();
        //UiButton.OnSelect(null);
        UiImage.color = new Color32(99, 202, 36, 255);
    }
    public void SetWrongColor()
    {
        //ColorBlock color = UiButton.colors;
        //color.selectedColor = new Color32(99, 202, 36,255); ;
        //UiButton.colors = color;
        //UiButton.Select();
        //UiButton.OnSelect(null);
        UiImage.color = new Color32(253, 187, 101, 255);
    }
    public void DisableButton()
    {
        UiButton.interactable = false;
    }
    public void Reset()
    {
        //ColorBlock color = UiButton.colors;
        //color.selectedColor = new Color32(253, 188, 102,255); ;
        //UiButton.colors = color;
        if (Index % 2 == 0)
        {
            UiImage.color = new Color32(255, 119, 61, 255);
        }
        else
        {
            UiImage.color = new Color32(255, 125, 61, 255);
        }
        UiButton.interactable = true;
    }
}
