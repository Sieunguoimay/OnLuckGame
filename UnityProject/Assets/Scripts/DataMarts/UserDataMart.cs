using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DataMarts
{
    public class UserDataMart
    {
        [Serializable]
        public class UserData
        {
            public int id;
            public int uptodate_token;
            public string name = "No Name";
            public string email;
            public string active_vendor;
            public string profile_picture = null;


            [NonSerialized]
            public Texture2D texProfilePicture = null;
        }

        private static UserDataMart s_instance = null;
        public static UserDataMart Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new UserDataMart();
                }
                return s_instance;
            }
        }
        private UserDataMart()
        {
            //gets called once
        }

        public delegate void UserDataReadyCallback();


        public UserData m_userData { get; private set; } 
        public UserDataReadyCallback m_userDataFromServerReadyCallback;
        public UserDataReadyCallback m_userDataFromLocalReadyCallback;
        public UserDataReadyCallback m_notifyToRefreshCallback;
        public bool m_isUserDataValid { get; private set; }

        //make sure that this function gets called once at the very beginning.
        public void Init()
        {
            m_userData = new UserData();
            m_isUserDataValid = false;
            m_userDataFromServerReadyCallback = null;
            m_userDataFromLocalReadyCallback = null;
            m_notifyToRefreshCallback = null;
        }

        //public delegate void LoadLocalUserDataFoundFBLoggedInCallback();
        //public LoadLocalUserDataFoundFBLoggedInCallback localUserDataFBLoggedInCallback = null;
        public bool LoadLocalUserData()
        {
            UserData userData = LocalProvider.Instance.LoadUserData();
            if (userData != null)
            {
                UserDataMart.Instance.SetUserData(userData);
                UserDataMart.Instance.NotifyDataFromLocalValid();

                //Logged in
                if (userData.active_vendor.Equals("sieunguoimay"))
                {
                    Debug.Log("Local UserData is available, let's check for the uptodate status");
                    //HttpClient.Instance.CheckForUptodateUserDataFromServer(userData, checkForUptodateUserDataCallback);
                    //return;
                }
                else
                {
                    Debug.Log("Userdata existed in local and active as facebook. Not much help here");
                    //if (localUserDataFBLoggedInCallback != null)
                    //    localUserDataFBLoggedInCallback();
                }
                return true;
            }
            else
            {

                //end game
                return false;
            }
            Debug.Log("Goto manual login");
        }

        public void SetUserData(UserData userData)
        {
            //save this user data to local 
            //and trigger the UserDataReadyCallback
            //m_isUserDataValid = true;
            m_userData = userData;
        }

        public void NotifyNewData(bool isFinal = false,bool isNotifyPrivately = false)
        {
            if (isFinal)
            {
                //what we gonna do?
                //->save data to local!!
                Debug.Log("Saved User Data to Local");
                //LocalProvider.Instance.SaveUserData(m_userData);
            }

            if (!isNotifyPrivately)
            {
                if(m_notifyToRefreshCallback!=null)
                    m_notifyToRefreshCallback();
            }
            Debug.Log("User Data Loading");
        }

        public void NotifyDataFromServerValid()
        {
            m_isUserDataValid = true;
            if(m_userDataFromServerReadyCallback!=null)
                m_userDataFromServerReadyCallback();
            Debug.Log("User Data Loaded");
        }

        public void NotifyDataFromLocalValid()
        {
            m_isUserDataValid = true;
            if (m_userDataFromLocalReadyCallback != null)
                m_userDataFromLocalReadyCallback();
            Debug.Log("User Data Loaded");
        }

        public void InvalidateUserData()
        {
            m_isUserDataValid = false;
            LocalProvider.Instance.SaveUserData(m_userData,false);
        }
    }
}
