using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DataMarts
{
    public class UserDataMart:MonoBehaviourSingleton<UserDataMart>
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

            public string phone_number;
            public string address;

            [NonSerialized]
            public Texture2D texProfilePicture = null;
        }


        public UserData m_userData { get; private set; } 
        
        public Action m_userDataFromServerReadyCallback = delegate { };
        public Action m_userDataFromLocalReadyCallback = delegate { };
        public Action m_notifyToRefreshCallback = delegate { };

        public bool m_isUserDataValid { get; private set; }

        public bool userDataChanged { get; private set; } = false;

        //make sure that this function gets called once at the very beginning.
        public void Init()
        {
            m_userData = new UserData();

            m_isUserDataValid = false;
        }

        public bool LoadLocalUserData()
        {
            UserData userData = LocalProvider.Instance.LoadUserData();

            if (userData != null)
            {
                UserDataMart.Instance.SetUserData(userData);

                UserDataMart.Instance.NotifyDataFromLocalValid();

                if (userData.active_vendor.Equals("sieunguoimay"))
                {
                    Debug.Log("Local UserData is available, let's check for the uptodate status");
                }
                else
                {
                    Debug.Log("Userdata existed in local and active as facebook. Not much help here");
                }
                return true;
            }
            else
            {
                //end game
                return false;
            }
        }

        public void SetUserData(UserData userData)
        {
            m_userData = userData;

            userDataChanged = true;
        }

        public void UpdateUserInfo(string name, string phone, string address)
        {

        }

        public void NotifyNewData(bool isNotifyPrivately = false)
        {
            if (!isNotifyPrivately)
            {
                m_notifyToRefreshCallback?.Invoke();
            }
            Debug.Log("User Data Loading");
        }

        public void NotifyDataFromServerValid(bool valid = true)
        {
            m_isUserDataValid = valid;

            m_userDataFromServerReadyCallback?.Invoke();
            
            Debug.Log("User Data Loaded");
        }

        public void NotifyDataFromLocalValid()
        {
            m_isUserDataValid = true;

            m_userDataFromLocalReadyCallback?.Invoke();
            
            Debug.Log("User Data Loaded");
        }

        public void InvalidateUserData()
        {
            m_isUserDataValid = false;

            LocalProvider.Instance.SaveUserData(m_userData,false);
        }
    }
}
