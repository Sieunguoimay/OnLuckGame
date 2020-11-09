using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MCQAnswerItem : MonoBehaviour
{
    public Text UiText;

    private Button UiButton;
    private Image UiImage;
    private Color originalButtonColor;
    private Color originalTextColor;

    public ConstantsSO constantsSO;

    //[NonSerialized]
    public int Index;
    public char Choice { private set; get; }


    void Awake()
    {
        Choice = (char)(65+Index);
        UiImage = GetComponent<Image>();
        UiButton = GetComponent<Button>();

        originalButtonColor = UiImage.color;
        originalTextColor = UiText.color;
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
        UiImage.color = constantsSO.mcqCorrectColor;// new Color32(99, 202, 36, 255);
        UiText.color = Color.white;
    }
    public void SetWrongColor()
    {
        //ColorBlock color = UiButton.colors;
        //color.selectedColor = new Color32(99, 202, 36,255); ;
        //UiButton.colors = color;
        //UiButton.Select();
        //UiButton.OnSelect(null);
        UiImage.color = constantsSO.mcqWrongColor;// new Color32(253, 187, 101, 255);
        UiText.color = Color.white;
    }
    public void DisableButton()
    {
        UiButton.interactable = false;
    }
    public void Reset(bool interactable)
    {
        //ColorBlock color = UiButton.colors;
        //color.selectedColor = new Color32(253, 188, 102,255); ;
        //UiButton.colors = color;
        //if (Index % 2 == 0)
        //{
        //    UiImage.color = new Color32(255, 119, 61, 255);
        //}
        //else
        //{
        //    UiImage.color = new Color32(255, 125, 61, 255);
        //}
        UiImage.color = originalButtonColor;
        UiText.color = originalTextColor;
        UiText.text = "";
        UiButton.interactable = interactable;
    }
}
