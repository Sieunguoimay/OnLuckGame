using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataMarts;
using Assets.Scripts;

namespace Assets.Scripts
{
    public class Main
    {
        /*This class is a Singleton*/
        private static Main s_instance = null;
        public static Main Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new Main();
                return s_instance;
            }
        }
        private Main() {
        }
        /*End of Singleton Declaration*/

        MonoBehaviour monoBehaviour;
        private bool firstTime = true;
        public void Init(MonoBehaviour monoBehaviour){
            /*forget this*/
            this.monoBehaviour = monoBehaviour;
            if (firstTime)
            {
                Start();
                firstTime = false;
            }
        }
        public bool soundEnabled = true;
        private Utils.Neuron parsePlayingDataNeuron;
        public void Start()
        {
            /*here we go. this is where we load everything once and for all...*/
            Debug.Log("Never go here again until u reopen the app");

            //Debug.logger.logEnabled = false;

            //firstly, we try to login here. by asking the local to find the current user.
            //if not found any active user, then leave it to the unlogged in state

            //in the unlogged in state, the user will trigger to login by signup, login or signin.
            //all the method will eventually create an UserData and hand it to the local for saving 
            //the local keep the UserData for further uses. the menu scene access the local to 
            //change it state. -> call it UserDataMart
            //Utils.Instance.Init(monoBehaviour);
            AssetsDataMart.Instance.LoadAssets();
            HttpClient.Instance.Init();
            HttpClient.Instance.BaseUrl = AssetsDataMart.Instance.assetsData.base_url;
            HttpClient.Instance.BaseApiUrl = AssetsDataMart.Instance.assetsData.base_api_url;
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
            //UserDataMart.Instance.m_userDataFromServerReadyCallback = () => PreparePlayingDataOnUserDataFromServerReady();

            if (UserDataMart.Instance.LoadLocalUserData())
            {
                PreparePlayingDataOnUserFromLocalReady();
            }
            else {
                //wait for user to login by any means
                HttpClient.Instance.GetOnluckMetadata((response) => {
                    if (response.status.Equals("OK"))
                        QuestionDataMart.Instance.SetFromServerOnluckMetadata(response.data);
                });
            }

            //if previously logged in then I must know about it. otherwise no !.
        }
        public void Reset()
        {
            Start();
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
                if (response.status.Equals("OK"))
                {
                    //1.get metadata
                    QuestionDataMart.Instance.SetFromServerOnluckMetadata(response.data.metadata);
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
}