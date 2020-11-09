using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.DataMarts;
using UnityEngine;

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

    public Action<bool> fbLoginCallback = delegate { };
    public Action fbLoginDoneCallback = delegate { };
    public Action<UserDataMart.UserData> OnUserDataFromServer = delegate { };
    public Action<HttpClient.Response<UserDataMart.UserData>> checkForUptodateUserDataCallback = delegate { };
    public Action<UserDataMart.UserData> loadLocalUserDataCallback = delegate { };
    public Action<int> fbSignInCallback = delegate { };

    public void Init()
    {

        OnUserDataFromServer = (newUserData) =>
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
                OnUserDataFromServer(response.data);
            }
        };


        fbLoginCallback = (status) =>
        {
            var fbUserData = new UserDataMart.UserData();
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
                            fbUserData.name, fbUserData.email, userInfo.id,
                            /*fbUserData.texProfilePicture.EncodeToPNG(), */(response) =>
                            {
                                if (response.status.Equals("OK"))
                                {
                                    fbUserData.id = response.data.user_id;
                                    fbUserData.profile_picture = "profile_picture_"+fbUserData.email+".png";// response.data.profile_picture;
                                    fbUserData.uptodate_token = response.data.uptodate_token;
                                    fbUserData.name = response.data.name;
                                    fbUserData.phone = response.data.phone;
                                    fbUserData.address = response.data.address;
                                    fbUserData.active_vendor = "facebook";

                                    UserDataMart.Instance.SetUserData(fbUserData);

                                    UserDataMart.Instance.NotifyDataFromServerValid();
                                    //LocalProvider.Instance.SaveUserData(UserDataMart.Instance.m_userData,true);
                                    Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);

                                    fbLoginDoneCallback?.Invoke();

                                    Debug.Log("Signed in with facebook. got the user_id, let's save the user data to local device");
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
