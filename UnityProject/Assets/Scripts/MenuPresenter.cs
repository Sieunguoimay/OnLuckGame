using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.DataMarts;
namespace Assets.Scripts
{
    public class MenuPresenter
    {


        private static MenuPresenter s_instance = null;

        //public enum LoginStates
        //{
        //    NOT_LOGGED_IN,
        //    LOGGED_IN,
        //    LOGGED_IN_WITH_FB,
        //}

        private Menu rMenu;
        //public UserData m_userData { get; private set; }
        //private bool m_loginState;
        private bool m_firstTime;
        public bool m_loginState;
        private string m_cachedEmail;
        private string m_cachedPassword;
        private bool m_shouldRefreshScoreboard;
        private MenuPresenter()
        {
            //m_userData = new UserData();
            //m_loginState = false;
            m_firstTime = true;
            m_loginState = false;// LoginStates.NOT_LOGGED_IN;

            //Utils.Instance.GetRequest("localhost:8000/api/onluck");

            Debug.Log("MenuPresenter Created");
        }

        public static MenuPresenter Instance
        {
            get {
                if (s_instance == null)
                {
                    s_instance = new MenuPresenter();
                }
                return s_instance;
            }
        }
        private Utils.Neuron hideSplashSceneNeuron;
        public MenuPresenter Init(Menu menu)
        {
            this.rMenu = menu;

            m_shouldRefreshScoreboard = true;

            if (m_firstTime)
            {
                m_firstTime = false;
                rMenu.ShowSplashScene();

                if (UserDataMart.Instance.m_isUserDataValid)
                {
                    OnLoggedIn();
                    UpdateMenuUiWithUserData();
                }

                UserDataMart.Instance.m_notifyToRefreshCallback += UpdateMenuUiWithUserData;
                UserDataMart.Instance.m_userDataFromServerReadyCallback += UpdateMenuUiWithUserData;
                UserDataMart.Instance.m_userDataFromServerReadyCallback += OnLoggedIn;

                QuestionDataMart.Instance.progressPublisher.progressListener += (value) => rMenu.SetProgressBar(value);
                QuestionDataMart.Instance.m_askForPermissionCallback += ()=> rMenu.ShowPopupToAskForPermission();

                //QuestionDataMart.Instance.m_gameDataReadyCallback += () => ;
                hideSplashSceneNeuron = new Utils.Neuron(2);
                hideSplashSceneNeuron.output = () => rMenu.HideSplashScene();
                QuestionDataMart.Instance.m_gameDataReadyCallback += () => { UpdateMenuUiOnQuestionDataLoaded(); showQuestionPacks(); rMenu.HideLogText(); };
                QuestionDataMart.Instance.m_gameDataCompletedCallback += () => { hideSplashSceneNeuron.inputs[0].Signal(); Debug.Log("hideSplashSceneNeuron 1"); };
                rMenu.UiProgressBar.DoneCallback = () => { hideSplashSceneNeuron.inputs[1].Signal(); Debug.Log("hideSplashSceneNeuron 2"); };
                Utils.Instance.networkErrorCallback += () => { Debug.Log("No Internet!!"); rMenu.ShowPopupCheckYourNetwork(); };
                LoginLogic.Instance.fbLoginCallback += (status) => rMenu.ToggleSpinnerAtLoginButtonPanel(status);
                LoginLogic.Instance.fbLoginDoneCallback += (status) => rMenu.ToggleSpinnerAtLoginButtonPanel(false);
            }
            else
            {
                UpdateMenuUiWithUserData();
                UpdateMenuUiOnQuestionDataLoaded();
            }
            rMenu.SetUpAskForPermissionPopup(QuestionDataMart.Instance.OnPermissionGranted);
            rMenu.SetUpCheckYourNetwork(() => { Debug.Log("Refresh the game"); Main.Instance.Reset(); });
            rMenu.ToggleUiAccordingToLoginState(UserDataMart.Instance.m_isUserDataValid);
            Debug.Log("MenuPresenter Initialized");
            return this;
        }


        public void LogInWithLastEnteredAccount()
        {
            rMenu.ShowSpinnerAtPopupPanel();
            HttpClient.Instance.LogIn(m_cachedEmail, m_cachedPassword, (response) => {
                if (response.status.Equals("OK"))
                {
                    Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);
                    LoginLogic.Instance.userDataFromServerCallback(response.data.user_data);
                    UserDataMart.Instance.NotifyDataFromServerValid();
                }
                else
                {
                    rMenu.ToggleUiAccordingToLoginState(UserDataMart.Instance.m_isUserDataValid);
                }
                rMenu.HideSpinner();
                rMenu.HidePopupPanel();
            }, (response) =>
            {
                rMenu.HideSpinner();
                rMenu.HidePopupPanel();
            });
        }
    public bool LogIn(string email, string password)
        {
            if (email.Equals("") || password.Equals("")) { 
                rMenu.SetLoginStatusText("Missing email or password");
                return false; 
            }
            Debug.Log("Verifying User");
            m_cachedEmail = email;
            m_cachedPassword = password;
            rMenu.ShowSpinnerAtPopupPanel();
            HttpClient.Instance.LogIn(email, password,(response) =>
            {
                if (response.status.Equals("OK"))
                {
                    //onHttpClientLoginResult(response.data);
                    Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);
                    LoginLogic.Instance.userDataFromServerCallback(response.data.user_data);
                    UserDataMart.Instance.NotifyDataFromServerValid();
                    rMenu.HidePopupPanel();
                }
                else if (response.status.Equals("email_not_verified"))
                {
                    rMenu.ShowVerificationPanel();
                }
                else
                {
                    rMenu.SetLoginStatusText(response.status);
                }
                rMenu.HideSpinner();
            }, (response) =>
            {
                string fbUserName = response.data.name;
                string fbProfilePicture = response.data.profile_picture;

                rMenu.ShowSignupPanelWithPredata(fbUserName, email, (signupUserName, signupEmail, signupPassword) => {
                    Signup(signupUserName, signupEmail, signupPassword,fbProfilePicture);
                });
                rMenu.HideSpinner();
            });


            return true;
        }
        public bool Signup(string userName, string email, string password,string profilepicture = "")
        {
            if (email.Equals("") || password.Equals(""))
            {
                rMenu.SetSignupStatusText("Missing email or password");
                return false;
            }
            if (!Utils.Instance.IsValidEmail(email))
            {
                rMenu.SetSignupStatusText("Invalid email");
                return false;
            }

            Debug.Log("Verifying User");
            m_cachedEmail = email;
            m_cachedPassword = password;
            rMenu.ShowSpinnerAtPopupPanel();
            HttpClient.Instance.SignUp(userName, email, password, profilepicture, (response) => {
                if (response.status.Equals("OK"))
                {
                    //onHttpClientLoginResult(response.data);
                    Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);
                    LoginLogic.Instance.userDataFromServerCallback(response.data.user_data);
                    UserDataMart.Instance.NotifyDataFromServerValid();
                    rMenu.HidePopupPanel();
                }
                else if (response.status.Equals("email_not_verified"))
                {
                    rMenu.ShowVerificationPanel();
                }
                else
                {
                    rMenu.SetSignupStatusText(response.status);
                }
                rMenu.HideSpinner();
            });
            return true;
        }

        public void SignInWithFB()
        {
            FBLogin.Instance.SignIn(LoginLogic.Instance.fbLoginCallback);
        }
        private void showQuestionPacks()
        {
            if (QuestionDataMart.Instance.packs.Count > 0)
            {
                //rMenu.ToggleSpinnerAtLoginButtonPanel(false,3);
                rMenu.SetQuestionPacks(QuestionDataMart.Instance.packs.Count, (item, index) =>
                {
                    item.SetIcon(QuestionDataMart.Instance.packs[index].icon.sprite);
                    item.SetTitle(QuestionDataMart.Instance.packs[index].title);
                    item.SetSubText(QuestionDataMart.Instance.packs[index].sub_text);
                    item.Index = index;
                });
            }
            else
            {
                //rMenu.ToggleSpinnerAtLoginButtonPanel(true,3);
            }
        }
        public void UpdateMenuUiOnQuestionDataLoaded()
        {
            DateTime seasonDate = DateTime.ParseExact(QuestionDataMart.Instance.season.from, "yyyy-MM-dd HH:mm:ss", null);
            rMenu.SetSeasonText(QuestionDataMart.Instance.season.name + " - " + seasonDate.ToString("dd/MM/yyyy"));
            rMenu.SetQuoteText(QuestionDataMart.Instance.onluckLocalMetadata.quote);
        }
        public void UpdateMenuUiWithUserData() 
        {
            rMenu.ToggleSoundButtonUi(Main.Instance.soundEnabled);
            if (UserDataMart.Instance.m_userData.texProfilePicture!=null)
                rMenu.SetAvatar(UserDataMart.Instance.m_userData.texProfilePicture);
            rMenu.SetUserName(UserDataMart.Instance.m_userData.name);
            rMenu.SetScore(PlayingDataMart.Instance.m_score);
            rMenu.ToggleUiAccordingToLoginState(UserDataMart.Instance.m_isUserDataValid);
            if (UserDataMart.Instance.m_isUserDataValid)
            {
                showQuestionPacks();
            }
            //notify the server
            //HttpClient.Instance.SignIn(userData.name,userData.);
        }
        public void OnLoggedIn()
        {
            //not sure here...
            //PlayingDataMart.Instance.m_score = UserDataMart.Instance.m_userData.score;
            Debug.Log("Logged In BABE");
            m_loginState = true;
        }
        public void OnQuit()
        {
            if (UserDataMart.Instance.userDataChanged)
            {
                LocalProvider.Instance.SaveUserData(UserDataMart.Instance.m_userData, !UserDataMart.Instance.m_isUserDataValid);
            }
            Debug.Log("MenuPreseneter:OnQuit");
        }
        public void Logout()
        {
            FBLogin.Instance.Logout();
            UserDataMart.Instance.InvalidateUserData();
            rMenu.ToggleUiAccordingToLoginState(UserDataMart.Instance.m_isUserDataValid);
            m_loginState = false;
        }
        public void ToggleSound()
        {
            //m_isSoundOn = !m_isSoundOn;
            Main.Instance.soundEnabled = !Main.Instance.soundEnabled;
            rMenu.ToggleSoundButtonUi(Main.Instance.soundEnabled);
        }

        public void UploadProfilePicture(string path)
        {
            Utils.Instance.LoadImage(path, (texture) =>
            {
                Debug.Log(texture.GetPixels().Length);

                float ratio = (float)texture.width / (float)texture.height;
                float newWidth = 180f;
                float newHeight = 180f;
                if (ratio > 1)
                {
                    newWidth = newHeight * ratio;
                }else
                {
                    newHeight = newWidth / ratio;
                }
                texture = Utils.Instance.ResizeTexture(texture,(int)newWidth, (int)newHeight);

                Debug.Log(texture.GetPixels().Length);
                rMenu.SetAvatar(texture);

                HttpClient.Instance.UploadPhoto(UserDataMart.Instance.m_userData.id, texture.EncodeToPNG(), (response) =>
                {
                    if (response.status.Equals("OK"))
                    {
                        UserDataMart.Instance.m_userData.uptodate_token = response.data.uptodate_token;
                        UserDataMart.Instance.m_userData.profile_picture = response.data.profile_picture;
                    }
                });
            });
        }
        public delegate void RenameCallback(bool status);
        public void Rename(string newName, RenameCallback callback)
        {
            //rMenu.SetUserName(UserDataMart.Instance.m_userData.name);
            HttpClient.Instance.Rename(UserDataMart.Instance.m_userData.id, UserDataMart.Instance.m_userData.name,(response)=> {
                if (response.status.Equals("OK"))
                {
                    Debug.Log("Token token: " + response.data.uptodate_token);
                    UserDataMart.Instance.m_userData.uptodate_token = response.data.uptodate_token;
                    UserDataMart.Instance.m_userData.name = newName;
                    UserDataMart.Instance.NotifyNewData();
                    callback(true);
                }else
                    callback(false);
            });
        }
        public bool ShowScoreboard()
        {
            if (m_shouldRefreshScoreboard)
            {
                m_shouldRefreshScoreboard = false;
                HttpClient.Instance.LoadScoreboard((response) => {
                    if (response.status.Equals("OK"))
                    {
                        rMenu.SetScoreboard(response.data.Count(), (item, index) =>
                        {
                            if (index < 3)
                            {
                                item.SetRank(index);
                            }
                            item.SetUserName(response.data[index].name);
                            item.SetScore(response.data[index].score);
                            item.SetProfilePicture(HttpClient.Instance.BaseUrl + response.data[index].profile_picture);
                        });
                    }
                    rMenu.HideSpinner();
                });
                return true;
            }
            return false;
        }

        /*Question Pack number dependency starts from here...*/
        public void OpenQuestionPack(int packIndex)
        {
            QuestionDataMart.Pack pack = QuestionDataMart.Instance.packs[packIndex];
            PlayingDataMart.Pack playingPack = PlayingDataMart.Instance.playingPacks[packIndex];

            string startupText = AssetsDataMart.Instance.assetsData.strings.startup_text_question_pack + " " + pack.title+ "\n"+
                AssetsDataMart.Instance.assetsData.strings.startup_text_question_no + " " + (playingPack.currentIndex + 1);
            string buttonText = AssetsDataMart.Instance.assetsData.strings.resume;
            if (playingPack.currentIndex == 0)
            {
                startupText = AssetsDataMart.Instance.assetsData.strings.startup_text_question_pack+" " + pack.title;
                buttonText = AssetsDataMart.Instance.assetsData.strings.start;
            }else if (playingPack.currentIndex == playingPack.playingQuestions.Count)
            {
                startupText = AssetsDataMart.Instance.assetsData.strings.startup_text_question_pack+" " + pack.title
                    +"\n"+ AssetsDataMart.Instance.assetsData.strings.startup_text_complete;
                buttonText = AssetsDataMart.Instance.assetsData.strings.open;
            }
            rMenu.OpenStartupPanel(startupText, buttonText, (status) =>
            {
                if (status)
                {
                    GameScene.MainGamePresenter.Instance.StartGame(pack, playingPack,packIndex);
                    Debug.Log("Open Question Pack " + packIndex);
                }
            });
        }
        public void DisplayDownloadProgress(string name, string unit, float downloaded, float total)
        {
            //Debug.Log("MenuPresenter::DisplayDownloadProgress " + downloaded + " " + total);
            string str = ""+name + ((int)downloaded).ToString() + "/" + ((int)total).ToString() + unit;
            rMenu.ShowLogText(str);
        }
    }
}
