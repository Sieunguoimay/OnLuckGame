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

        public enum LoginStates
        {
            NOT_LOGGED_IN,
            LOGGED_IN,
            LOGGED_IN_WITH_FB,
        }

        private Menu rMenu;
        //public UserData m_userData { get; private set; }
        //private bool m_loginState;
        private bool m_firstTime;
        public bool m_isSoundOn;
        public LoginStates m_loginState;
        private string m_cachedEmail;
        private string m_cachedPassword;
        private bool m_shouldRefreshScoreboard;
        private MenuPresenter()
        {
            //m_userData = new UserData();
            //m_loginState = false;
            m_firstTime = true;
            m_isSoundOn = true;
            m_loginState = LoginStates.NOT_LOGGED_IN;

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

        public MenuPresenter Init(Menu menu)
        {
            this.rMenu = menu;
            //Utils.Instance.GetRequest("localhost:8000/api/onluck/login?email=phamthuyduong@gmail.com&password=nguoiyeu"
            //    ,(response)=>{
            //        Debug.Log(response);
            //    });

            m_shouldRefreshScoreboard = true;

            if (m_firstTime)
            {
                m_firstTime = false;
                rMenu.ToggleSplashScene(true);

                if (UserDataMart.Instance.m_isUserDataValid)
                {
                    UpdateMenuUiWithUserData();
                }
                else
                {
                    UserDataMart.Instance.m_notifyToRefreshCallback += UpdateMenuUiWithUserData;
                    UserDataMart.Instance.m_userDataReadyCallback += UpdateMenuUiWithUserData;
                    UserDataMart.Instance.m_userDataReadyCallback += OnLoggedIn;
                }
            }
            else
            {
                UpdateMenuUiWithUserData();
            }
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
                    onHttpClientLoginResult(response.data);
                }
                else
                {
                    rMenu.ToggleUiAccordingToLoginState(UserDataMart.Instance.m_isUserDataValid);
                    rMenu.HidePopupPanel();
                }
                rMenu.HideSpinner();
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
            HttpClient.Instance.LogIn(email, password, (response) => {
                if (response.status.Equals("OK"))
                {
                    onHttpClientLoginResult(response.data);
                }else if(response.status.Equals("email_not_verified"))
                {
                    rMenu.ShowVerificationPanel();
                }else
                {
                    rMenu.SetLoginStatusText(response.status);
                }
                rMenu.HideSpinner();
            });
            return true;
        }
        public bool Signup(string userName, string email, string password)
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
            HttpClient.Instance.SignUp(userName, email, password, (response) => {
                if (response.status.Equals("OK"))
                {
                    onHttpClientLoginResult(response.data);
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
        private void onHttpClientLoginResult(HttpClient.LogInResponseData result)
        {
            int id = result.user_id;
            UserDataMart.Instance.m_userData.id = id;
            UserDataMart.Instance.m_userData.name = result.user_name;
            UserDataMart.Instance.m_userData.email = m_cachedEmail;
            UserDataMart.Instance.m_userData.profilePicture = HttpClient.Instance.BaseUrl + result.profile_picture;
            UserDataMart.Instance.m_userData.score = result.score;
            UserDataMart.Instance.m_userData.activeVendor = result.active_vendor;

            Utils.Instance.LoadImage(UserDataMart.Instance.m_userData.profilePicture, (texture) =>
            {
                UserDataMart.Instance.m_userData.texProfilePicture = texture;
                //rMenu.SetAvatar(UserDataMart.Instance.m_userData.texProfilePicture);
                UserDataMart.Instance.NotifyDataValid();
            });

            UserDataMart.Instance.NotifyNewData();

            //updateMenuUiWithUserData();
            //m_loginState = true;
            m_loginState = LoginStates.LOGGED_IN;
            rMenu.HidePopupPanel();
            Debug.Log(UserDataMart.Instance.m_userData.profilePicture);
        }
        public void SignInWithFB()
        {
            //facebookLogin.SignIn(OnFBSignedIn);

            FBLogin.Instance.GetFBUserData((fbUserData)=>{
                
                UserDataMart.Instance.m_userData.name = fbUserData.name;
                UserDataMart.Instance.m_userData.email = fbUserData.email;
                UserDataMart.Instance.m_userData.texProfilePicture = fbUserData.avatar;
                UserDataMart.Instance.NotifyDataValid();

                HttpClient.Instance.SignIn(
                    UserDataMart.Instance.m_userData.name,
                    UserDataMart.Instance.m_userData.email,
                    UserDataMart.Instance.m_userData.texProfilePicture.EncodeToPNG(), (response) =>{
                        if (response.status.Equals("OK")){
                            UserDataMart.Instance.m_userData.id = response.data.user_id;
                            UserDataMart.Instance.m_userData.score = response.data.score;
                            UserDataMart.Instance.m_userData.activeVendor = response.data.active_vendor;
                            //rMenu.SetScore(UserDataMart.Instance.m_userData.score);
                            PlayingDataMart.Instance.m_score = UserDataMart.Instance.m_userData.score;

                            if (response.data.has_profile_picture)
                            {
                                UserDataMart.Instance.m_userData.profilePicture = response.data.profile_picture;
                            }
                            UserDataMart.Instance.NotifyNewData();
                            m_loginState = LoginStates.LOGGED_IN_WITH_FB;
                        }
                    }, (response) =>{
                        if (response.status.Equals("OK"))
                        {
                            UserDataMart.Instance.m_userData.profilePicture = response.data.profile_picture;
                            UserDataMart.Instance.NotifyNewData();
                            m_loginState = LoginStates.LOGGED_IN_WITH_FB;
                        }
                    });
            });
        }

        //public int OnFBSignedIn(bool result)
        //{
        //    if (result)
        //    {
        //        Debug.Log("Logged in to facebook babe: " + m_loginState);
        //        m_loginState = true;
        //        m_IsNormalLogin = false;
        //        rMenu.ToggleUiAccordingToLoginState(m_loginState);

        //        facebookLogin.FetchFBUserInfo((fbUserInfo)=> {
        //            m_userData.name = fbUserInfo.name;
        //            m_userData.email = fbUserInfo.email;
        //            rMenu.SetUserName(m_userData.name);

        //            facebookLogin.FetchFBProfilePicture((profilePicture) => {
        //                //m_userData.avatar = profilePicture;
        //                rMenu.SetAvatar(profilePicture);
        //                m_userData.texProfilePicture = profilePicture.texture;
        //                //notify the server to signin
        //                HttpClient.Instance.SignIn(m_userData.name, m_userData.email, m_userData.texProfilePicture.EncodeToPNG(),(response)=> {
        //                    if (response.status.Equals("OK"))
        //                    {
        //                        m_userData.id = response.data.user_id;
        //                        m_userData.score = response.data.score;
        //                        m_userData.activeVendor = response.data.active_vendor;
        //                        rMenu.SetScore(m_userData.score);

        //                        if (response.data.has_profile_picture)
        //                        {
        //                            m_userData.profilePicture = response.data.profile_picture;
        //                        }
        //                    }
        //                },(response)=> {
        //                    if (response.status.Equals("OK"))
        //                    {
        //                        m_userData.profilePicture = response.data.profile_picture;
        //                    }
        //                });
        //            });

        //        });
        //    }
        //    else
        //    {
        //        m_loginState = false;
        //        Debug.Log("Not logged in to facebook yet");
        //    }
        //    return 0;
        //}

        //public int OnFBUserDataFetched(FBUserData userData)
        //{
        //    if(m_fbUserData.what == -1)
        //    {
        //        m_fbUserData.what = userData.what;
        //    }else
        //    {
        //        m_fbUserData.what = 2;
        //    }

        //    if (userData.what == 0)
        //    {
        //        m_fbUserData.name = userData.name;
        //    }else if(userData.what == 1)
        //    {
        //        m_fbUserData.avatar = userData.avatar;
        //    }
        //    updateMenuUiWithUserData(m_fbUserData);
        //    return 0;
        //}
        public void UpdateMenuUiWithUserData() 
        {
            rMenu.ToggleSoundButtonUi(m_isSoundOn);
            if (UserDataMart.Instance.m_userData.texProfilePicture!=null)
                rMenu.SetAvatar(UserDataMart.Instance.m_userData.texProfilePicture);
            rMenu.SetUserName(UserDataMart.Instance.m_userData.name);
            rMenu.SetScore(UserDataMart.Instance.m_userData.score);
            rMenu.ToggleUiAccordingToLoginState(UserDataMart.Instance.m_isUserDataValid);
            if (UserDataMart.Instance.m_isUserDataValid)
            {
                rMenu.SetQuestionPacks(QuestionDataMart.Instance.GetQuestionPacksMetadata().Count, (item, index) =>
                {
                    item.SetIcon(QuestionDataMart.Instance.GetQuestionPacksMetadata()[index].texIcon);
                    item.SetTitle(QuestionDataMart.Instance.GetQuestionPacksMetadata()[index].title);
                    item.SetSubText(QuestionDataMart.Instance.GetQuestionPacksMetadata()[index].subText);
                    item.Index = index;
                });
            }
            //notify the server
            //HttpClient.Instance.SignIn(userData.name,userData.);
        }
        public void OnLoggedIn()
        {
            //not sure here...
            PlayingDataMart.Instance.m_score = UserDataMart.Instance.m_userData.score;

        }
        public void Logout()
        {
            FBLogin.Instance.Logout();
            UserDataMart.Instance.InvalidateUserData();
            rMenu.ToggleUiAccordingToLoginState(UserDataMart.Instance.m_isUserDataValid);
        }
        public void ToggleSound()
        {
            m_isSoundOn = !m_isSoundOn;
            rMenu.ToggleSoundButtonUi(m_isSoundOn);
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
                        UserDataMart.Instance.m_userData.profilePicture = response.data.profile_picture;
                    }
                });
            });
        }
        public void Rename(string newName)
        {
            UserDataMart.Instance.m_userData.name = newName;
            rMenu.SetUserName(UserDataMart.Instance.m_userData.name);
            HttpClient.Instance.Rename(UserDataMart.Instance.m_userData.id, UserDataMart.Instance.m_userData.name);
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

        /*Question Pack number depended from here...*/
        private bool[] m_openedQuestionPackForTheFirstTime = { true, true, true };
        public void OpenQuestionPack(int pack)
        {
            int currentQuestionIndex;
            string startupText;
            string buttonText;
            switch (pack)
            {
                case 0:
                    //if (m_openedQuestionPackForTheFirstTime[pack])
                    //{
                        currentQuestionIndex = PlayingDataMart.Instance.m_currentQuestionIndex80;
                        startupText = "GOI CAU HOI THU THACH\nBan da mo khoa cau so " + (currentQuestionIndex + 1);
                        buttonText = "CAU TIEP THEO";
                        if (currentQuestionIndex == 0)
                        {
                            startupText = "GOI CAU HOI THU THACH";
                            buttonText = "BAT DAU";
                        }
                        rMenu.OpenStartupPanel(startupText, buttonText, (status) =>
                        {
                            if (status&& QuestionDataMart.Instance.m_80QuestionPack.Count>0)
                            {
                                //m_openedQuestionPackForTheFirstTime[pack] = false;
                                GameScene.MainGamePresenter.Instance.SetToTypingMode(QuestionDataMart.Instance.m_80QuestionPack, PlayingDataMart.Instance.m_questionImage80, currentQuestionIndex, pack);
                                SceneManager.LoadScene("main_game");
                            }
                        });
                    //}
                    //else
                    //{
                    //    GameScene.MainGamePresenter.Instance.SetToTypingMode(QuestionDataMart.Instance.m_80QuestionPack, PlayingDataMart.Instance.m_questionImage80, PlayingDataMart.Instance.m_currentQuestionIndex80, pack);
                    //    SceneManager.LoadScene("main_game");
                    //}
                    break;
                case 1:
                    //if (m_openedQuestionPackForTheFirstTime[pack])
                    //{
                        currentQuestionIndex = PlayingDataMart.Instance.m_currentQuestionIndex1000;
                        startupText = "GOI CAU HOI KIEN THUC\nBan da mo khoa cau so " + (currentQuestionIndex + 1);
                        buttonText = "CAU TIEP THEO";
                        if (currentQuestionIndex == 0)
                        {
                            startupText = "GOI CAU HOI KIEN THUC";
                            buttonText = "BAT DAU";
                        }
                        rMenu.OpenStartupPanel(startupText, buttonText, (status) =>
                        {
                            if (status && QuestionDataMart.Instance.m_1000QuestionPack.Count > 0)
                            {
                                //m_openedQuestionPackForTheFirstTime[pack] = false;
                                GameScene.MainGamePresenter.Instance.SetToTypingMode(QuestionDataMart.Instance.m_1000QuestionPack, PlayingDataMart.Instance.m_questionImage1000, currentQuestionIndex, pack);
                                SceneManager.LoadScene("main_game");
                            }
                        });
                    //}
                    //else
                    //{
                    //    GameScene.MainGamePresenter.Instance.SetToTypingMode(QuestionDataMart.Instance.m_1000QuestionPack, PlayingDataMart.Instance.m_questionImage1000, PlayingDataMart.Instance.m_currentQuestionIndex1000, pack);
                    //    SceneManager.LoadScene("main_game");
                    //}
                    break;
                case 2:
                    //if (m_openedQuestionPackForTheFirstTime[pack])
                    //{
                        currentQuestionIndex = PlayingDataMart.Instance.m_currentQuestionIndex30;
                        startupText = "GOI CAU HOI THUONG HIEU\nBan da mo khoa cau so " + (currentQuestionIndex + 1);
                        buttonText = "CAU TIEP THEO";
                        if (currentQuestionIndex == 0)
                        {
                            startupText = "GOI CAU HOI THUONG HIEU";
                            buttonText = "BAT DAU";
                        }
                        rMenu.OpenStartupPanel(startupText, buttonText, (status) =>
                        {
                            if (status && QuestionDataMart.Instance.m_30QuestionPack.Count > 0)
                            {
                                //m_openedQuestionPackForTheFirstTime[pack] = false;
                                GameScene.MainGamePresenter.Instance.SetToMCQMode(QuestionDataMart.Instance.m_30QuestionPack, PlayingDataMart.Instance.m_questionImage30, currentQuestionIndex, pack);
                                SceneManager.LoadScene("main_game");
                            }
                        });
                    //}
                    //else
                    //{
                    //    GameScene.MainGamePresenter.Instance.SetToMCQMode(QuestionDataMart.Instance.m_30QuestionPack, PlayingDataMart.Instance.m_questionImage30, PlayingDataMart.Instance.m_currentQuestionIndex30, pack);
                    //    SceneManager.LoadScene("main_game");
                    //}
                    break;
            }
            Debug.Log("Open Question Pack " + pack);
        }
    }
}
