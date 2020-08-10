using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.DataMarts;
public class ZeroScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*here we go. this is where we load everything once and for all...*/
        Debug.Log("Never go here again until u reopen the app");


        //firstly, we try to login here. by asking the local to find the current user.
        //if not found any active user, then leave it to the unlogged in state

        //in the unlogged in state, the user will trigger to login by signup, login or signin.
        //all the method will eventually create an UserData and hand it to the local for saving 
        //the local keep the UserData for further uses. the menu scene access the local to 
        //change it state. -> call it UserDataMart
        Utils.Instance.Init(this);
        HttpClient.Instance.Init();
        UserDataMart.Instance.Init();
        FBLogin.Instance.Init();

        AssetsDataMart.Instance.LoadAssets();

        m_loadPlayingDataLock1 = false;
        m_loadPlayingDataLock2 = false;
        UserDataMart.Instance.m_userDataReadyCallback += () =>
        {
            m_loadPlayingDataLock1 = true;
            if (m_loadPlayingDataLock1 && m_loadPlayingDataLock2)
            {
                PlayingDataMart.Instance.LoadPlayingData();
                m_loadPlayingDataLock1 = false;
                m_loadPlayingDataLock2 = false;
            }
        };
        QuestionDataMart.Instance.m_gameDataReadyCallback += () =>
        {
            m_loadPlayingDataLock2 = true;
            if (m_loadPlayingDataLock1 && m_loadPlayingDataLock2)
            {
                PlayingDataMart.Instance.LoadPlayingData();
                m_loadPlayingDataLock1 = false;
                m_loadPlayingDataLock2 = false;
            }
        };



        FBLogin.Instance.GetFBUserDataIfLoggedIn((fbUserData) =>
        {
            UserDataMart.Instance.m_userData.name = fbUserData.name;
            UserDataMart.Instance.m_userData.email = fbUserData.email;
            UserDataMart.Instance.m_userData.texProfilePicture = fbUserData.avatar;

            HttpClient.Instance.SignIn(
                UserDataMart.Instance.m_userData.name,
                UserDataMart.Instance.m_userData.email,
                UserDataMart.Instance.m_userData.texProfilePicture.EncodeToPNG(), (response) => {
                    if (response.status.Equals("OK"))
                    {
                        UserDataMart.Instance.m_userData.id = response.data.user_id;
                        UserDataMart.Instance.m_userData.score = response.data.score;
                        UserDataMart.Instance.m_userData.activeVendor = response.data.active_vendor;
                        //rMenu.SetScore(UserDataMart.Instance.m_userData.score);

                        if (response.data.has_profile_picture)
                        {
                            UserDataMart.Instance.m_userData.profilePicture = response.data.profile_picture;
                            UserDataMart.Instance.NotifyDataValid();
                        }
                    }
                }, (response) =>
                {
                    if (response.status.Equals("OK"))
                    {
                        UserDataMart.Instance.m_userData.profilePicture = response.data.profile_picture;
                        UserDataMart.Instance.NotifyDataValid();
                    }
                });
        });

        QuestionDataMart.Instance.LoadGameData();



        /*end of loading...*/
        SceneManager.LoadScene("menu");
    }
    private bool m_loadPlayingDataLock1 = false;
    private bool m_loadPlayingDataLock2 = false;
    // Update is called once per frame
    void Update()
    {
        
    }
}
