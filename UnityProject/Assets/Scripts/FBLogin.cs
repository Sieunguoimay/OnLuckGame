using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class UserInfo
{
    public string id;
    public string name;
}

public class FBUserData
{
    public int what;//-1: error, 0:user name, 1:avatar
    public string name;
    public Sprite avatar;
}

public class FBLogin /*: MonoBehaviour*/
{
    private AccessToken fbAccessToken;
    private Func<bool, int> loginStateCallback;
    private Func<FBUserData, int> userDataCallback;//0 for
    FBUserData userData;

    //Initialize FB API, check login state,
    //callback to notify the login state
    public FBLogin(Func<bool, int> callback)
    {
        this.loginStateCallback = callback;
        userData = new FBUserData();

	}
	public void Init()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            FB.Init(InitCallback, OnHideUnity);
        }
    }
	private void InitCallback(){
		if(FB.IsInitialized){
			FB.ActivateApp();
			Debug.Log("Initialized the Facebook SDK. Continue here..");

            if (FB.IsLoggedIn)
            {
                Debug.Log("You logged in bro!!");

                fbAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;

                Debug.Log(fbAccessToken.UserId);
                foreach (string permission in fbAccessToken.Permissions)
                {
                    Debug.Log(permission);
                }

                /*Skip the login scene*/
                loginStateCallback(true);
            }
            else
            {
                /*Show the login scene*/
                loginStateCallback(false);
            }

        }
        else{
			Debug.Log("Failed to Initialized the Facebook SDK");
		}
	}
	private void OnHideUnity(bool isGameShown){
        Debug.Log("HEY");
		if(!isGameShown){
			//Pause the game - we will need to hide
			Time.timeScale = 0;
		}else{
			//Resume the game - we're getting the focus again
			Time.timeScale = 1;
		}
	}



    //Called to login to facebook,
    //Signal out for the result.
    public void Login(Func<bool, int> callback)
    {
        this.loginStateCallback = callback;

        var permissions = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(permissions, AuthCallback);
    }
    private void AuthCallback(ILoginResult result){
		if(FB.IsLoggedIn){

            fbAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            Debug.Log(fbAccessToken.UserId);
            foreach (string permission in fbAccessToken.Permissions){
				Debug.Log(permission);
			}
            /*Goto Main Menu*/

            loginStateCallback(true);

        }
        else
        {
			Debug.Log("User Cancelled login");
            loginStateCallback(false);
        }
    }
	private void LoginStatusCallback(ILoginStatusResult result){
		if(!String.IsNullOrEmpty(result.Error)){
			Debug.Log("Error: "+result.Error);
		}else if(result.Failed){
			Debug.Log("Failure: Access token couldnot be retrieved");
			
			
		}else{
			Debug.Log("Success: "+result.AccessToken.UserId);
		}
	}



    public void FetchUserData(Func<FBUserData, int> callback)
    {
        this.userDataCallback = callback;

        Debug.Log("EYY");

        FB.API("/"+ fbAccessToken.UserId+"/picture?width=128&height=128", Facebook.Unity.HttpMethod.GET, (result) =>
        {
            if (result.Error != null)
            {
                Debug.Log("Problem with fetching profile picture");
                userData.what = -1;
                userDataCallback(userData);
                return;
            }
            Debug.Log("Fetched profile picture");
            //UnityEngine.UI.Image imgAvatar = UIComponentAvatar.GetComponent<UnityEngine.UI.Image>();
            //imgAvatar.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0, 0));

            userData.what = 1;
            userData.avatar = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0, 0));
            this.userDataCallback(userData);
        });

        FB.API("/" + fbAccessToken.UserId + "?fields=id,name", Facebook.Unity.HttpMethod.GET, (result) =>
        {
            if (result.Error != null)
            {
                Debug.Log("Problem with fetching user name");
                userData.what = -1;
                userDataCallback(userData);
                return;
            }
            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(result.RawResult);
            Debug.Log("Fetched user name:" + userInfo.name);

            //UnityEngine.UI.Text textName = UIComponentUserName.GetComponent<UnityEngine.UI.Text>();
            //textName.text = userInfo.name;


            userData.what = 0;
            userData.name = userInfo.name;
            this.userDataCallback(userData);

        });
    }




    public void ShareWithFriends()
    {
        //FB.FeedShare("Told",new Uri("http://apps.facebook.com/"+FB.AppId+"/?challenge_brag="+(FB.IsLoggedIn?fbAccessToken.UserId:"guest")),"Link Name","Caption","Description",null,"Media Source",(result)=> {
        //    Debug.Log("Share Result:" + result.RawResult);
        //});
        FB.FeedShare(
            linkCaption:"Something Cool",
            picture:new Uri("http://sieunguoimay.website/assets/textures/stick_hero_go.png"),
            linkName:"Check it out babe",
            link:new Uri("http://apps.facebook.com/"+FB.AppId+"/?challenge_brag"+(FB.IsLoggedIn?fbAccessToken.UserId:"guest"))
            );
    }


    public void Logout()
    {
        FB.LogOut();
        //ToggleLoginUiComponent(true);
    }


}
