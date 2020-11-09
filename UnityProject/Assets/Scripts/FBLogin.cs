using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FBUserInfo
{
    public string id;
    public string name;
    public string email;

    public Texture2D avatar = null;
}

//public class FBUserData
//{
//    public int what;//-1: error, 0:user name, 1:avatar
//    public string name;
//    public string email;
//    public Sprite avatar;
//}

public class FBLogin /*: MonoBehaviour*/
{
    //private Func<FBUserData, int> userDataCallback;//0 for
    //FBUserData userData;

    private static FBLogin s_instance = null;
    public static FBLogin Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new FBLogin();
            }
            return s_instance;
        }
    }
    //Initialize FB API, check login state,
    //callback to notify the login state
    private FBLogin() { }

    public delegate void FBLoginCallback(bool state);

    private AccessToken m_fbAccessToken;
    public FBLoginCallback m_fbLoginCallback;


    public void Init(Action<bool> callback)
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            FB.Init(() => InitCallback(callback), OnHideUnity);
        }
    }
    private void InitCallback(Action<bool> callback)
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            Debug.Log("Initialized the Facebook SDK. Continue here..");
            if (FB.IsLoggedIn)
            {
                m_fbAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            }
            callback(FB.IsLoggedIn);
        }
        else
        {
            Debug.Log("Failed to Initialized the Facebook SDK");
            callback(false);
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("HEY");
        if (!isGameShown)
        {
            //Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            //Resume the game - we're getting the focus again
            Time.timeScale = 1;
        }
    }

    public bool IsLoggedIn()
    {
        return FB.IsLoggedIn;
    }
    //Called to login to facebook,
    //Signal out for the result.
    //public delegate void FBSignInCallback(bool status);
    public void SignIn(Action<bool> callback)
    {
        var permissions = new List<string>() { "public_profile", "email" };

        FB.LogInWithReadPermissions(permissions, (result) =>
        {
            if (FB.IsLoggedIn)
            {
                // Facebook.Unity.AccessToken.CurrentAccessToken;
                m_fbAccessToken = result.AccessToken;

                Debug.Log(m_fbAccessToken.UserId);

                foreach (string permission in m_fbAccessToken.Permissions)
                {
                    Debug.Log(permission);
                }

                /*Goto Main Menu*/

                callback?.Invoke(true);
            }
            else
            {
                Debug.Log("User Cancelled login");

                callback?.Invoke(false);
            }
        });
    }

    //private void LoginStatusCallback(ILoginStatusResult result){
    //	if(!String.IsNullOrEmpty(result.Error)){
    //		Debug.Log("Error: "+result.Error);
    //	}else if(result.Failed){
    //		Debug.Log("Failure: Access token couldnot be retrieved");


    //	}else{
    //		Debug.Log("Success: "+result.AccessToken.UserId);
    //	}
    //}


    public void FetchFBUserInfo(Action<FBUserInfo> callback)
    {
        FB.API("/" + m_fbAccessToken.UserId + "?fields=id,name,email", Facebook.Unity.HttpMethod.GET, (result) =>
        {

            if (result.Error != null)
            {
                Debug.Log("Problem with fetching user name");
                callback?.Invoke(null);
                return;
            }
            var userInfo = JsonUtility.FromJson<FBUserInfo>(result.RawResult);
            Debug.Log("Fetched user:" + result.RawResult);

            callback?.Invoke(userInfo);
        });
    }
    public void FetchFBProfilePicture(Action<Texture2D> callback)
    {
        FB.API("/" + m_fbAccessToken.UserId + "/picture?width=128&height=128", Facebook.Unity.HttpMethod.GET, (result) =>
        {
            if (result.Error != null)
            {
                Debug.Log("Problem with fetching profile picture");
                callback?.Invoke(null);
                return;
            }
            Debug.Log("Fetched profile picture");
            callback?.Invoke(result.Texture);// Sprite.Create(, new Rect(0, 0, 128, 128), new Vector2(0, 0)));
        });
    }

    //public void FetchUserData(Func<FBUserData, int> callback)
    //{
    //    this.userDataCallback = callback;

    //    Debug.Log("EYY");

    //    FB.API("/"+ m_fbAccessToken.UserId+"/picture?width=128&height=128", Facebook.Unity.HttpMethod.GET, (result) =>
    //    {
    //        if (result.Error != null)
    //        {
    //            Debug.Log("Problem with fetching profile picture");
    //            userData.what = -1;
    //            userDataCallback(userData);
    //            return;
    //        }
    //        Debug.Log("Fetched profile picture");
    //        //UnityEngine.UI.Image imgAvatar = UIComponentAvatar.GetComponent<UnityEngine.UI.Image>();
    //        //imgAvatar.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0, 0));

    //        userData.what = 1;
    //        userData.avatar = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0, 0));
    //        this.userDataCallback(userData);
    //    });

    //    FB.API("/" + m_fbAccessToken.UserId + "?fields=id,name,email", Facebook.Unity.HttpMethod.GET, (result) =>
    //    {

    //        if (result.Error != null)
    //        {
    //            Debug.Log("Problem with fetching user name");
    //            userData.what = -1;
    //            userDataCallback(userData);
    //            return;
    //        }
    //        FBUserInfo userInfo = JsonUtility.FromJson<FBUserInfo>(result.RawResult);
    //        Debug.Log("Fetched user:" + result.RawResult);

    //        //UnityEngine.UI.Text textName = UIComponentUserName.GetComponent<UnityEngine.UI.Text>();
    //        //textName.text = userInfo.name;


    //        userData.what = 0;
    //        userData.name = userInfo.name;
    //        userData.email = userInfo.email;
    //        this.userDataCallback(userData);

    //    });
    //}

    //public delegate void FBUserDataCallback(FBUserInfo data);
    //public void GetFBUserData(FBUserDataCallback callback)
    //{
    //SignIn(() => {
    //    FetchFBUserInfo((userInfo) => {
    //        FetchFBProfilePicture((profilePicture) => {
    //            userInfo.avatar = profilePicture;
    //            callback(userInfo);
    //        });
    //    });
    //});
    //if (FB.IsLoggedIn){
    //    FetchFBUserInfo((userInfo)=> {
    //        FetchFBProfilePicture((profilePicture) =>{
    //            userInfo.avatar = profilePicture;
    //            callback(userInfo);
    //        });
    //    });
    //}
    //else{
    //    SignIn(() =>{
    //        FetchFBUserInfo((userInfo) => {
    //            FetchFBProfilePicture((profilePicture) => {
    //                userInfo.avatar = profilePicture;
    //                callback(userInfo);
    //            });
    //        });
    //    });
    //}
    //}
    //public void GetFBUserDataIfLoggedIn(FBUserDataCallback callback)
    //{
    //    if (FB.IsLoggedIn)
    //    {
    //        FetchFBUserInfo((userInfo) => {
    //            FetchFBProfilePicture((profilePicture) => {
    //                userInfo.avatar = profilePicture;
    //                callback(userInfo);
    //            });
    //        });
    //    }
    //}

    public void ShareWithFriends()
    {
        //FB.FeedShare("Told",new Uri("http://apps.facebook.com/"+FB.AppId+"/?challenge_brag="+(FB.IsLoggedIn?m_fbAccessToken.UserId:"guest")),"Link Name","Caption","Description",null,"Media Source",(result)=> {
        //    Debug.Log("Share Result:" + result.RawResult);
        //});
        FB.FeedShare(
            linkCaption: "Something Cool",
            picture: new Uri("http://sieunguoimay.website/assets/textures/stick_hero_go.png"),
            linkName: "Check it out babe",
            link: new Uri("http://apps.facebook.com/" + FB.AppId + "/?challenge_brag" + (FB.IsLoggedIn ? m_fbAccessToken.UserId : "guest"))
            );
    }


    public void Logout()
    {
        FB.LogOut();
        //ToggleLoginUiComponent(true);
    }


}
