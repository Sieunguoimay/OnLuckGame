using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataMarts;
using Assets.Scripts;

public class Main:MonoBehaviourSingleton<Main>
{

    public bool soundEnabled = true;
        
    private Utils.Neuron parsePlayingDataNeuron;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        Application.runInBackground = true;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Init();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //save
        }
        else
        {
            //load
        }
    }

    public void Init()
    {
        /*here we go. this is where we load everything once and for all...*/
        Debug.Log("Never go here again until u reopen the app");

        //Debug.unityLogger.logEnabled = false;

        HttpClient.Instance.Init();
        HttpClient.Instance.BaseUrl = AssetsDataMart.Instance.constantsSO.base_url;
        HttpClient.Instance.BaseApiUrl = AssetsDataMart.Instance.constantsSO.base_api_url;

        UserDataMart.Instance.Init();

        LoginLogic.Instance.Init();

        QuestionDataMart.Instance.Init();

        FBLogin.Instance.Init((response)=> { });

        parsePlayingDataNeuron = new Utils.Neuron(2)
        {
            output = () =>
            {
                Debug.Log("parsePlayingDataNeuron.output");
                PlayingDataMart.Instance.ParsePlayingData();
            }
        };

        QuestionDataMart.Instance.m_gameDataReadyCallback += () => parsePlayingDataNeuron.inputs[1].Signal();

        QuestionDataMart.Instance.LoadMetadata();

        if (UserDataMart.Instance.LoadLocalUserData())
        {
            PreparePlayingDataOnUserFromLocalReady();
        }
        else {
            //wait for user to login by any means
            HttpClient.Instance.GetOnluckMetadata((response) => {

                if (response.status.Equals("OK"))
                {
                    QuestionDataMart.Instance.SetFromServerOnluckMetadata(this, response.data);
                }

            });
        }

        //if previously logged in then I must know about it. otherwise no !.
    }


    public void PreparePlayingDataOnUserFromLocalReady()
    {
        bool localRawPlayingDataExisted = PlayingDataMart.Instance.LoadLocalRawPlayingData();
        if (!localRawPlayingDataExisted)
        {
            Debug.Log("LoadRawPlayingDataFromServer");
            PlayingDataMart.Instance.LoadRawPlayingDataFromServer(parsePlayingDataNeuron.inputs[0]);
        }

        HttpClient.Instance.NotifyServerOnGameStart(UserDataMart.Instance.m_userData,
        (response) => {
            Debug.Log("NotifyServerOnGameStart");
            //1.get metadata
            QuestionDataMart.Instance.SetFromServerOnluckMetadata(this,response.data.metadata);
            if (response.status.Equals("OK"))
            {
                //2.check for user uptodate
                if (response.data.user_data.uptodate_token != UserDataMart.Instance.m_userData.uptodate_token)
                {
                    //this user data is small, thus it's attached to this uptodate_token
                    LoginLogic.Instance.userDataFromServerCallback(response.data.user_data);
                }
                else
                {
                    Debug.Log("Local UserData is Uptodate.yeyehhh");
                }

                if (localRawPlayingDataExisted)
                {
                    //3.check for playing data uptodate
                    if (response.data.playing_data_uptodate_token != PlayingDataMart.Instance.playingData.uptodate_token)
                    {
                            //since this playing data is huge, it is downloaded from another request
                            Debug.Log("LoadRawPlayingDataFromServer");
                        PlayingDataMart.Instance.LoadRawPlayingDataFromServer(parsePlayingDataNeuron.inputs[0]);
                    }
                    else
                    {
                        Debug.Log("Local PlayingData is Uptodate.yeyehhh");
                        parsePlayingDataNeuron.inputs[0].Signal();
                    }
                }
            }
            else
            {
                Debug.Log("User data invalid");
                //UserDataMart.Instance.SetUserData(new UserDataMart.UserData());
                //UserDataMart.Instance.NotifyDataFromServerValid(false);
                MenuPresenter.Instance.Logout();
            }
        });

    }
    public void PreparePlayingDataOnUserDataFromServerReady(int playingDataUptodateToken)
    {
        bool localRawPlayingDataExisted = PlayingDataMart.Instance.LoadLocalRawPlayingData();
        if (!localRawPlayingDataExisted)
        {
            Debug.Log("LoadRawPlayingDataFromServer");
            PlayingDataMart.Instance.LoadRawPlayingDataFromServer(parsePlayingDataNeuron.inputs[0]);
        }else
        {
            //3.check for playing data uptodate
            if (playingDataUptodateToken != PlayingDataMart.Instance.playingData.uptodate_token)
            {
                //since this playing data is huge, it is downloaded from another request
                Debug.Log("LoadRawPlayingDataFromServer");
                PlayingDataMart.Instance.LoadRawPlayingDataFromServer(parsePlayingDataNeuron.inputs[0]);
            }
            else
            {
                Debug.Log("Local PlayingData is Uptodate.yeyehhh");
                parsePlayingDataNeuron.inputs[0].Signal();
            }
        }
    }
}