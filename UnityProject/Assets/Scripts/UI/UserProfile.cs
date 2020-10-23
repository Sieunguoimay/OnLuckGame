using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;

public class UserProfile : MonoBehaviour
{
    [SerializeField] private InputField textName;
    [SerializeField] private InputField textPhone;
    [SerializeField] private InputField textAddress;

    void Start()
    {
        
    }

    public void HandleUpdateButtonClicked()
    {
        UserDataMart.Instance.UpdateUserInfo(textName.text, textPhone.text, textAddress.text);
    }

    public void HandleLogoutButtonClicked()
    {
        MenuPresenter.Instance.Logout();
    }

}
