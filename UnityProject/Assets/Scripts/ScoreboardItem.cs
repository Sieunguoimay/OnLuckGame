using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;
public class ScoreboardItem : MonoBehaviour
{
    public Text UiUserNameText;
    public Text UiScoreText;
    public Image UiProfilePictureImage;
    public GameObject UiRankImage;
    void Awake()
    {
        UiRankImage.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetUserName(string name)
    {
        UiUserNameText.text = name;
    }
    public void SetScore(int score)
    {
        UiScoreText.text = score.ToString();
    }
    public void SetProfilePicture(Texture2D texture)
    {
        UiProfilePictureImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    }

    public void SetProfilePicture(string imagePath)
    {
        Utils.Instance.LoadImageIntoImage(imagePath, UiProfilePictureImage);
    }
    public void SetRank(int rank)
    {
        UiRankImage.SetActive(true);
        switch (rank)
        {
            case 0:
                UiRankImage.GetComponent<Image>().sprite = AssetsDataMart.Instance.rank1IconSprite;
                break;
            case 1:
                UiRankImage.GetComponent<Image>().sprite = AssetsDataMart.Instance.rank2IconSprite;
                break;
            case 2:
                UiRankImage.GetComponent<Image>().sprite = AssetsDataMart.Instance.rank3IconSprite;
                break;
        }
    }
}
