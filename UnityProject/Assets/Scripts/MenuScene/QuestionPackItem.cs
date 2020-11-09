using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestionPackItem : MonoBehaviour
{

    [SerializeField] private Text UiTitleText;
    [SerializeField] private Text UiButtonText;

    public int Index;

    public Action<int> OnClicked = delegate { };

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(()=>OnClicked(Index));
    }

    //public void SetIcon(Sprite sprite)
    //{
    //    if (sprite != null)
    //        UiIconImage.sprite = sprite;
    //}
    //public void SetIcon(Texture2D texture)
    //{
    //    if(texture!=null)
    //        UiIconImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    //}
    public void SetTitle(string title)
    {
        UiTitleText.text = title;
    }
    public void SetSubText(string button)
    {
        UiButtonText.text = button;
    }
}
