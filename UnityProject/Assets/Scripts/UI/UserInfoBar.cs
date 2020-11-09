using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;

public class UserInfoBar : MonoBehaviour
{
    [SerializeField] private Text userName;
    [SerializeField] private Image avatar;
    [SerializeField] private Text score;

    public Action AvatarClicked = delegate { };

    private void OnEnable()
    {
        PlayingDataMart.Instance.OnScoreChanged += SetScore;
        UserDataMart.Instance.OnUserDataUpdated += RefreshUI;

        SetScore(PlayingDataMart.Instance.Score);
        RefreshUI();
    }
    private void OnDisable()
    {
        if (PlayingDataMart.Instance != null)
        {
            PlayingDataMart.Instance.OnScoreChanged -= SetScore;
        }
        if (UserDataMart.Instance != null)
        {
            UserDataMart.Instance.OnUserDataUpdated -= RefreshUI;
        }
    }

    public void SetToDefault()
    {
        SetAvatar(AssetsDataMart.Instance.constantsSO.defaultProfilePictureSprite);
        SetUserName(AssetsDataMart.Instance.constantsSO.stringsSO.user_name);
        SetScore(0);
    }
    public void RefreshUI()
    {
        if (UserDataMart.Instance.m_isUserDataValid)
        {
            SetUserName(UserDataMart.Instance.m_userData.name);

            if (UserDataMart.Instance.m_userData.texProfilePicture != null)
            {
                SetAvatar(UserDataMart.Instance.m_userData.texProfilePicture);
            }
        }
        else
        {
            SetToDefault();
        }
    }

    public void SetUserName(string name)
    {
        userName.text = name;
    }
    public void SetAvatar(Sprite sprite)
    {
        avatar.sprite = sprite;
    }
    public void SetAvatar(Texture2D texture)
    {
        avatar.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,0));
    }
    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }
    public void OnAvatarClicked()
    {
        AvatarClicked?.Invoke();
    }
}
