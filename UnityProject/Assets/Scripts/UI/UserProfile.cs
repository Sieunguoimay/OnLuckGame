using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;
using System;

public class UserProfile : MonoBehaviour
{
    [SerializeField] private InputField textName;
    [SerializeField] private InputField textPhone;
    [SerializeField] private InputField textAddress;

    [SerializeField] private Text statusText;

    public Action LogoutButtonClicked = delegate{};

    void Start()
    {
        UpdateUI(UserDataMart.Instance.m_userData);

        UserDataMart.Instance.UpdateProfileResultReturned += HandleUpdateProfileResult;
    }

    public void Refresh()
    {
        UpdateUI(UserDataMart.Instance.m_userData);
    }

    public void UpdateUI(UserDataMart.UserData userData)
    {
        textName.text = userData.name;
        textPhone.text = userData.phone;
        textAddress.text = userData.address;

        statusText.gameObject.SetActive(false);
    }

    private void HandleUpdateProfileResult(bool success)
    {
        UpdateUI(UserDataMart.Instance.m_userData);

        statusText.gameObject.SetActive(true);

        if (success)
        {
            statusText.color = Color.black;

            statusText.text = AssetsDataMart.Instance.constantsSO.stringsSO.saved;
        }
        else
        {
            statusText.color = Color.red;

            statusText.text = AssetsDataMart.Instance.constantsSO.stringsSO.something_went_wrong;
        }
    }

    public void HandleUpdateButtonClicked()
    {
        UserDataMart.Instance.UpdateProfile(textName.text, textPhone.text, textAddress.text);
    }

    public void HandleLogoutButtonClicked()
    {
        LogoutButtonClicked?.Invoke();
    }

}
