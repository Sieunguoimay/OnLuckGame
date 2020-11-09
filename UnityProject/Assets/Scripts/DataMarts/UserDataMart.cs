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
            public string profile_picture = null;

            public string phone;
            public string address;

            public string active_vendor;

            [NonSerialized]
            public Texture2D texProfilePicture = null;
        }

        public UserData m_userData { get; private set; } = new UserData();

        public Action m_userDataFromServerReadyCallback = delegate { };
        public Action m_userDataFromLocalReadyCallback = delegate { };
        public Action OnUserDataUpdated = delegate { };

        public Action<bool> UpdateProfileResultReturned = delegate { };

        public bool m_isUserDataValid { get; private set; } = false;

        public bool userDataChanged { get; private set; } = false;


        public void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                if (userDataChanged)
                {
                    LocalProvider.Instance.SaveUserData(m_userData, m_isUserDataValid);
                }
                Debug.Log("MenuPreseneter:OnPause");
            }
        }

        public bool LoadLocalUserData()
        {
            var userData = LocalProvider.Instance.LoadUserData();

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


        public void UpdateProfile(string name, string phone, string address)
        {
            var user = new UserData()
            {
                id = m_userData.id,
                email = m_userData.email,
                name =  name,
                phone = phone,
                address = address,
            };

            HttpClient.Instance.UpdateProfile(user,(result, uptodate_token)=> {

                if (result)
                {
                    m_userData.uptodate_token = uptodate_token;
                    m_userData.name = name;
                    m_userData.phone = phone;
                    m_userData.address = address;
                }

                UpdateProfileResultReturned?.Invoke(result);
            });
        }

        public void UploadPhoto()
        {
            HttpClient.Instance.UploadPhoto(m_userData.id, m_userData.texProfilePicture.EncodeToPNG(), (response) =>
            {
                if (response.status.Equals("OK"))
                {
                    m_userData.uptodate_token = response.data.uptodate_token;
                    Debug.Log("Uploaded user profile picture");
                }
            });
        }

        public void NotifyNewData(bool isNotifyPrivately = false)
        {
            if (!isNotifyPrivately)
            {
                OnUserDataUpdated?.Invoke();
            }
            Debug.Log("User Data Loading");
        }

        public void NotifyDataFromServerValid(bool valid = true)
        {
            m_isUserDataValid = valid;

            m_userDataFromServerReadyCallback?.Invoke();

            OnUserDataUpdated?.Invoke();

            Debug.Log("User Data Loaded");
        }

        public void NotifyDataFromLocalValid()
        {
            m_isUserDataValid = true;

            m_userDataFromLocalReadyCallback?.Invoke();

            OnUserDataUpdated?.Invoke();

            Debug.Log("User Data Loaded");
        }

        public void InvalidateUserData()
        {
            m_isUserDataValid = false;

            OnUserDataUpdated?.Invoke();

            LocalProvider.Instance.SaveUserData(m_userData,false);
        }
    }
}
