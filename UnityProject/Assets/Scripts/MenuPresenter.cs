using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class MenuPresenter
    {
        private static MenuPresenter s_instance = null;


        private FBLogin facebookLogin;
        private Menu rMenu;
        private FBUserData m_fbUserData;
        private bool m_loginState;
        private bool m_firstTime;
        public bool m_isSoundOn;

        private MenuPresenter()
        {
            m_fbUserData = new FBUserData();
            m_fbUserData.what = -1;
            m_loginState = false;
            m_firstTime = true;
            m_isSoundOn = true;

            facebookLogin = new FBLogin(OnFBLoginResult);
            facebookLogin.Init();
            Debug.Log("MenuPresenter Created");
        }

        public static MenuPresenter Instance
        {
            get{
                if (s_instance == null)
                {
                    s_instance = new MenuPresenter();
                }
                return s_instance;
            }
        }

        public MenuPresenter Init(Menu menu)
        {
            this.rMenu = menu;
            rMenu.ToggleSoundButtonUi(m_isSoundOn);

            if (m_firstTime)
            {
                m_firstTime = false;
                rMenu.ToggleSplashScene(true);
            }
            else
            {
                updateMenuUiWithUserData(m_fbUserData);
                rMenu.ToggleUiAccordingToLoginState(m_loginState);
            }
            Debug.Log("MenuPresenter Initialized");
            return this;
        }



        public void VerifyUser(string email, string password)
        {
            Debug.Log("Verifying User");

            //Fake verification
            bool result = (email.CompareTo("vuduydu1997@gmail.com")==0) && (password.CompareTo("1234")==0);
            if (result)
            {
                m_loginState = true;
                rMenu.ToggleUiAccordingToLoginState(m_loginState);
                rMenu.HidePopupPanel();
                rMenu.SetUserName("Vu duy du");
            }

        }
        public void Signup(string userName, string email, string password)
        {
            Debug.Log("Verifying User");

            //Fake verification
            bool result = !((email.CompareTo("vuduydu1997@gmail.com") == 0) && (password.CompareTo("1234") == 0));

            if (result)
            {
                m_loginState = true;
                rMenu.ToggleUiAccordingToLoginState(m_loginState);
                rMenu.HidePopupPanel();
                rMenu.SetUserName(userName);
            }

        }

        public void LoginWithFB()
        {
            facebookLogin.Login(OnFBLoginResult);
        }

        public int OnFBLoginResult(bool result)
        {
            if (result)
            {
                Debug.Log("Logged in to facebook babe: " + m_loginState);
                m_loginState = true;
                rMenu.ToggleUiAccordingToLoginState(m_loginState);
                facebookLogin.FetchUserData(OnFBUserDataFetched);
            }
            else
            {
                m_loginState = false;
                Debug.Log("Not logged in to facebook yet");
            }
            return 0;
        }

        public int OnFBUserDataFetched(FBUserData userData)
        {
            if(m_fbUserData.what == -1)
            {
                m_fbUserData.what = userData.what;
            }else
            {
                m_fbUserData.what = 2;
            }

            if (userData.what == 0)
            {
                m_fbUserData.name = userData.name;
            }else if(userData.what == 1)
            {
                m_fbUserData.avatar = userData.avatar;
            }
            updateMenuUiWithUserData(m_fbUserData);
            return 0;
        }
        private void updateMenuUiWithUserData(FBUserData userData) 
        {
            if (userData.what == 0)
            {
                rMenu.SetUserName(userData.name);
            }else if (userData.what == 1)
            {
                rMenu.SetAvatar(userData.avatar);
            }else if(userData.what == 2)
            {
                rMenu.SetAvatar(userData.avatar);
                rMenu.SetUserName(userData.name);
            }
        }
        public void Logout()
        {
            facebookLogin.Logout();
            m_loginState = false;
            rMenu.ToggleUiAccordingToLoginState(m_loginState);
        }
        public void ToggleSound()
        {
            m_isSoundOn = !m_isSoundOn;
            rMenu.ToggleSoundButtonUi(m_isSoundOn);
        }
    }
}
