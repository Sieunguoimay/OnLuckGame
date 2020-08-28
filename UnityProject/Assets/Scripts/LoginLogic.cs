using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.DataMarts;
using UnityEngine;

namespace Assets.Scripts
{
    public class LoginLogic
    {
        /*This class is a Singleton*/
        private static LoginLogic s_instance = null;
        public static LoginLogic Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new LoginLogic();
                return s_instance;
            }
        }
        private LoginLogic() { }
        /*End of Singleton Declaration*/

        public FBLogin.FBSignInCallback fbLoginCallback = null;
        public FBLogin.FBSignInCallback fbLoginDoneCallback = null;
        public HttpClient.LoadDataFromServerCallback userDataFromServerCallback = null;
        public HttpClient.HttpClientResponseCallback<UserDataMart.UserData> checkForUptodateUserDataCallback = null;
        public LocalProvider.LoadLocalDataCallback<UserDataMart.UserData> loadLocalUserDataCallback = null;
        public HttpClient.HttpClientResponseCallback<int> fbSignInCallback = null;

        public void Init()
        {

            userDataFromServerCallback = (newUserData) =>
            {
                UserDataMart.Instance.SetUserData(newUserData);
                UserDataMart.Instance.NotifyNewData();
                Debug.Log("Loaded user data from server let's load the profile picture: "+ HttpClient.Instance.BaseApiUrl + newUserData.profile_picture);
                Utils.Instance.LoadImage(HttpClient.Instance.BaseUrl + newUserData.profile_picture, (texture) =>
                {
                    UserDataMart.Instance.m_userData.texProfilePicture = texture;
                    UserDataMart.Instance.NotifyNewData();
                    Debug.Log("Loaded profile picture, let's save user data to local");
                    LocalProvider.Instance.SaveUserData(UserDataMart.Instance.m_userData,true);
                });
                //Done
            };
            checkForUptodateUserDataCallback = (response) =>
            {
                if (response.status.Equals("OK"))
                {
                    //Done
                    Debug.Log("Local UserData is Uptodate.yeyehhh");
                }
                else
                {
                    Debug.Log("Local UserData is not Uptodate. let's load it from server using the userid");
                    //Jump to LoadServerUserData
                    //HttpClient.Instance.LoadUserDataFromServer(
                    //    UserDataMart.Instance.m_userData.id,
                    //    userDataFromServerCallback);
                    userDataFromServerCallback(response.data);
                }
            };


            fbLoginCallback = (status) =>
            {
                UserDataMart.UserData fbUserData = new UserDataMart.UserData();
                if (status)
                {
                    FBLogin.Instance.FetchFBUserInfo((userInfo) =>
                    {
                        fbUserData.name = userInfo.name;
                        fbUserData.email = userInfo.email;

                        Debug.Log("Logged in with facebook, let's load profile picture from local");
                        FBLogin.Instance.FetchFBProfilePicture((texture) =>
                        {
                            fbUserData.texProfilePicture = texture;
                            Debug.Log("Profilepicture is ok now, let's signin to the server");
                            HttpClient.Instance.SignIn(
                                fbUserData.name, fbUserData.email,
                                fbUserData.texProfilePicture.EncodeToPNG(), (response) =>
                                {
                                    if (response.status.Equals("OK"))
                                    {
                                        fbUserData.id = response.data.user_id;
                                        fbUserData.profile_picture = response.data.profile_picture;
                                        fbUserData.uptodate_token = response.data.uptodate_token;
                                        fbUserData.active_vendor = "facebook";
                                        Debug.Log("Signed in with facebook. got the user_id, let's save the user data to local device");
                                        UserDataMart.Instance.SetUserData(fbUserData);
                                        UserDataMart.Instance.NotifyDataFromServerValid();
                                        LocalProvider.Instance.SaveUserData(UserDataMart.Instance.m_userData,false);
                                        Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);
                                        if(fbLoginDoneCallback!=null)
                                            fbLoginDoneCallback(true);
                                    }
                                    else
                                    {
                                        Debug.Log("No internet, not signed in, no NO NO NOOOOO");
                                    }

                                }, (response) =>
                                {
                                });
                        });

                    });
                }
                else
                {
                    Debug.Log("Cannot log in with facebook, let's checkout the localuserdata");

                }
            };

        }
    }
}
