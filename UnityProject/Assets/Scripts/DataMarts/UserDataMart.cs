using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataMarts
{
    class UserDataMart
    {
        public class UserData
        {
            public int id;
            public string name = "No Name";
            public string email;
            public int score;
            public string activeVendor;

            public string profilePicture = null;
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
        public UserDataReadyCallback m_userDataReadyCallback;
        public UserDataReadyCallback m_notifyToRefreshCallback;
        public bool m_isUserDataValid { get; private set; }

        //make sure that this function gets called once at the very beginning.
        public void Init()
        {
            m_isUserDataValid = false;
            m_userData = new UserData();
            m_userDataReadyCallback = null;
        }




        public void LoadLocalActiveUser()
        {
            //if local user is active then take it.
            //and trigger the UserDataReadyCallback

            //otherwise leave it to the Menu scene to provide it.
        }

        public void SetUserData(UserData userData)
        {
            //save this user data to local 
            //and trigger the UserDataReadyCallback
            m_isUserDataValid = true;
            m_userData = userData;

        }

        public void NotifyNewData()
        {
            m_notifyToRefreshCallback();
            Debug.Log("User Data Loading");
        }

        public void NotifyDataValid()
        {
            m_isUserDataValid = true;
            m_userDataReadyCallback();
            Debug.Log("User Data Loaded");
        }

        public void InvalidateUserData()
        {
            m_isUserDataValid = false;
        }
    }
}
