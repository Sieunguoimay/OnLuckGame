using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.DataMarts;
using Assets.Scripts;
using System;

public class Main : MonoBehaviourSingleton<Main>
{

    public bool soundEnabled = true;

    private Utils.Neuron parsePlayingDataNeuron;

    private AsyncOperation asyncOperation;

    public Action OnGameReady = delegate { };

    public int currentPackIndex = -1;

    private void Awake()
    {
        if (!IsAwakened)
        {
            DontDestroyOnLoad(this);

            Application.runInBackground = false;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Init();

            StartCoroutine(loadScene());

        }
    }

    IEnumerator loadScene()
    {
        yield return null;

        asyncOperation = SceneManager.LoadSceneAsync("menu");

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        yield return null;
    }

    public void ActivateAsyncScene()
    {
        asyncOperation.allowSceneActivation = true;
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

        Utils.Instance.Init(this);

        LoginLogic.Instance.Init();

        QuestionDataMart.Instance.Init();

        FBLogin.Instance.Init(_ => { });

        parsePlayingDataNeuron = new Utils.Neuron(2)
        {
            output = () =>
            {
                Debug.Log("parsePlayingDataNeuron.output");
                PlayingDataMart.Instance.ParsePlayingData();

                OnGameReady?.Invoke();
            }
        };

        OnGameReady += UserDataMart.Instance.UploadPhoto;

        QuestionDataMart.Instance.m_gameDataReadyCallback += parsePlayingDataNeuron.inputs[1].Signal;

        QuestionDataMart.Instance.LoadMetadata();

        if (UserDataMart.Instance.LoadLocalUserData())
        {
            bool localRawPlayingDataExisted = PlayingDataMart.Instance.LoadLocalRawPlayingData();

            if (!localRawPlayingDataExisted)
            {
                PlayingDataMart.Instance.LoadRawPlayingDataFromServer(parsePlayingDataNeuron.inputs[0]);
            }

            HttpClient.Instance.NotifyServerOnGameStart(UserDataMart.Instance.m_userData,
            (response) =>
            {
                Debug.Log("NotifyServerOnGameStart");

                //1.get metadata
                QuestionDataMart.Instance.SetFromServerOnluckMetadata(this, response.data.metadata);

                if (response.status.Equals("OK"))
                {
                    //2.check for user uptodate
                    if (response.data.user_data.uptodate_token != UserDataMart.Instance.m_userData.uptodate_token)
                    {
                        //this user data is small, thus it's attached to this uptodate_token
                        if (response.data.user_data != null)
                        {
                            LoginLogic.Instance.OnUserDataFromServer(response.data.user_data);
                        }
                        else
                        {
                            Debug.Log("User data invalid");
                            UserDataMart.Instance.InvalidateUserData();
                            return;
                        }
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
                    //MenuPresenter.Instance.Logout();
                    UserDataMart.Instance.InvalidateUserData();
                }
            });

        }
        else
        {
            //wait for user to login by any means
            HttpClient.Instance.GetOnluckMetadata((response) =>
            {

                if (response.status.Equals("OK"))
                {
                    QuestionDataMart.Instance.SetFromServerOnluckMetadata(this, response.data);
                }

            });
        }

        //if previously logged in then I must know about it. otherwise no !.
    }


    public void PreparePlayingDataOnUserDataFromServerReady(int playingDataUptodateToken)
    {
        bool localRawPlayingDataExisted = PlayingDataMart.Instance.LoadLocalRawPlayingData();
        if (!localRawPlayingDataExisted)
        {
            Debug.Log("LoadRawPlayingDataFromServer");
            PlayingDataMart.Instance.LoadRawPlayingDataFromServer(parsePlayingDataNeuron.inputs[0]);
        }
        else
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

    public void OpenGameScene(int packIndex)
    {
        currentPackIndex = packIndex;

        SceneManager.LoadScene("main_game");
    }
    public void OpenMenuScene()
    {
        SceneManager.LoadScene("menu");
    }

}