using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.DataMarts;

public class MenuPresenter : MonoBehaviour
{

    private Menu rMenu;

    private string m_cachedEmail;

    private string m_cachedPassword;

    public Action ClosePopupPanel = delegate { };

    private void Awake()
    {
        rMenu = GetComponent<Menu>();


        UpdateMenuUiWithUserData();
        UpdateMenuUiOnQuestionDataLoaded();
        Debug.Log("MenuPresenter Created");
    }
    private void OnEnable()
    {

        UserDataMart.Instance.OnUserDataUpdated += UpdateMenuUiWithUserData;
        QuestionDataMart.Instance.progressCallback += DisplayDownloadProgress;
        QuestionDataMart.Instance.m_gameDataReadyCallback += UpdateMenuUiOnQuestionDataLoaded;
    }
    private void OnDisable()
    {
        if (UserDataMart.Instance!=null)
        {
            UserDataMart.Instance.OnUserDataUpdated -= UpdateMenuUiWithUserData;
        }
        if (QuestionDataMart.Instance!=null)
        {
            QuestionDataMart.Instance.progressCallback -= DisplayDownloadProgress;
            QuestionDataMart.Instance.m_gameDataReadyCallback -= UpdateMenuUiOnQuestionDataLoaded;
        }
    }
    public void LogInWithLastEnteredAccount()
    {
        rMenu.ShowSpinnerAtPopupPanel();
        HttpClient.Instance.LogIn(m_cachedEmail, m_cachedPassword, (response) =>
        {
            if (response.status.Equals("OK"))
            {
                Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);
                LoginLogic.Instance.OnUserDataFromServer(response.data.user_data);
                UserDataMart.Instance.NotifyDataFromServerValid();
            }
            else
            {
                rMenu.SetToDefaultUI();
            }
            rMenu.HideSpinner();

            ClosePopupPanel?.Invoke();
        }, (response) =>
        {
            rMenu.HideSpinner();

            ClosePopupPanel?.Invoke();
        });
    }

    public bool LogIn(string email, string password)
    {
        if (email.Equals("") || password.Equals(""))
        {
            rMenu.SetLoginStatusText("Missing email or password");
            return false;
        }

        Debug.Log("Verifying User");

        m_cachedEmail = email;
        m_cachedPassword = password;

        rMenu.ShowSpinnerAtPopupPanel();
        HttpClient.Instance.LogIn(email, password, (response) =>
            {
                if (response.status.Equals("OK"))
                {
                    //onHttpClientLoginResult(response.data);
                    Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);
                    LoginLogic.Instance.OnUserDataFromServer(response.data.user_data);
                    UserDataMart.Instance.NotifyDataFromServerValid();

                    ClosePopupPanel?.Invoke();
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

                rMenu.ShowSignupPanelWithPredata(fbUserName, email, (signupUserName, signupEmail, signupPassword) =>
                {
                    Signup(signupUserName, signupEmail, signupPassword, fbProfilePicture);
                });
                rMenu.HideSpinner();
            });


        return true;
    }
    public bool Signup(string userName, string email, string password, string profilepicture = "")
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

        HttpClient.Instance.SignUp(userName, email, password, profilepicture, (response) =>
        {
            if (response.status.Equals("OK"))
            {
                Main.Instance.PreparePlayingDataOnUserDataFromServerReady(response.data.playing_data_uptodate_token);

                LoginLogic.Instance.OnUserDataFromServer(response.data.user_data);

                UserDataMart.Instance.NotifyDataFromServerValid();

                ClosePopupPanel?.Invoke();
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
        rMenu.UiQuestionPackItemPanel.SetActive(true);
        rMenu.UiLoginButtonPanel.SetActive(false);

        if (QuestionDataMart.Instance.packs.Count > 0)
        {
            rMenu.UiQuestionPackScrollList.CreateList<QuestionPackItem>(QuestionDataMart.Instance.packs.Count, (item, index) =>
            {
                item.SetTitle(QuestionDataMart.Instance.packs[index].title);
                item.Index = index;
                item.OnClicked += OpenQuestionPack;
            });
        }
        else
        {

        }
    }
    public void UpdateMenuUiOnQuestionDataLoaded()
    {
        if (QuestionDataMart.Instance.season.from != null)
        {
            var seasonDate = DateTime.ParseExact(QuestionDataMart.Instance.season.from, "yyyy-MM-dd HH:mm:ss", null);
            if (seasonDate != null)
            {
                rMenu.SetSeasonText(QuestionDataMart.Instance.season.name + " - " + seasonDate.ToString("dd/MM/yyyy"));
            }
        }
        rMenu.SetQuoteText(QuestionDataMart.Instance.onluckLocalMetadata.quote);

        UpdateMenuUiWithUserData();
    }

    public void UpdateMenuUiWithUserData()
    {
        if (UserDataMart.Instance.m_isUserDataValid)
        {
            showQuestionPacks();
        }
        else
        {
            rMenu.SetToDefaultUI();
        }
    }

    //public void OnApplicationQuit()
    //{
    //    if (UserDataMart.Instance.userDataChanged)
    //    {
    //        LocalProvider.Instance.SaveUserData(UserDataMart.Instance.m_userData, UserDataMart.Instance.m_isUserDataValid);
    //    }
    //    Debug.Log("MenuPreseneter:OnQuit");
    //}

    public void Logout()
    {
        FBLogin.Instance.Logout();
        UserDataMart.Instance.InvalidateUserData();
        rMenu.SetToDefaultUI();
    }
    public void ToggleSound()
    {
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
            }
            else
            {
                newHeight = newWidth / ratio;
            }
            texture = Utils.Instance.ResizeTexture(texture, (int)newWidth, (int)newHeight);

            Debug.Log(texture.GetPixels().Length);
            rMenu.userInfoBar.SetAvatar(texture);

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
    public void Rename(string newName, Action<bool> callback)
    {
        //rMenu.SetUserName(UserDataMart.Instance.m_userData.name);
        HttpClient.Instance.Rename(UserDataMart.Instance.m_userData.id, UserDataMart.Instance.m_userData.name, (response) =>
        {
            if (response.status.Equals("OK"))
            {
                Debug.Log("Token token: " + response.data.uptodate_token);
                UserDataMart.Instance.m_userData.uptodate_token = response.data.uptodate_token;
                UserDataMart.Instance.m_userData.name = newName;
                UserDataMart.Instance.NotifyNewData();
                callback?.Invoke(true);
            }
            else
                callback?.Invoke(false);
        });
    }

    /*Question Pack number dependency starts from here...*/
    public void OpenQuestionPack(int packIndex)
    {
        var pack = QuestionDataMart.Instance.packs[packIndex];
        var playingPack = PlayingDataMart.Instance.playingPacks[packIndex];

        string startupText = 
            AssetsDataMart.Instance.constantsSO.stringsSO.startup_text_question_pack + " " + pack.title + "\n" +
            AssetsDataMart.Instance.constantsSO.stringsSO.startup_text_question_no + " " + (playingPack.currentIndex + 1);
        string buttonText = AssetsDataMart.Instance.constantsSO.stringsSO.resume;

        if (playingPack.currentIndex == 0)
        {
            startupText = AssetsDataMart.Instance.constantsSO.stringsSO.startup_text_question_pack + " " + pack.title;
            buttonText = AssetsDataMart.Instance.constantsSO.stringsSO.start;
        }
        else if (playingPack.currentIndex == playingPack.playingQuestions.Count)
        {
            startupText = 
                AssetsDataMart.Instance.constantsSO.stringsSO.startup_text_question_pack + " " + pack.title + "\n" + 
                AssetsDataMart.Instance.constantsSO.stringsSO.startup_text_complete;
            buttonText = AssetsDataMart.Instance.constantsSO.stringsSO.open;
        }

        rMenu.OpenStartupPanel(startupText, buttonText, (status) =>
        {
            if (status)
            {
                if (playingPack.playingQuestions.Count > 0)
                {
                    //Assets.Scripts.GameScene.MainGamePresenter.Instance.StartGame(packIndex);
                    Main.Instance.OpenGameScene(packIndex);
                    Debug.Log("Open Question Pack " + packIndex);
                }
                else
                {
                    Debug.Log("No Question in this pack");
                }
            }
        });
    }
    public void DisplayDownloadProgress(string name, string unit, float downloaded, float total)
    {
        string str = "" + name + ((int)downloaded).ToString() + "/" + ((int)total).ToString() + unit;
        rMenu.ShowLogText(str);
    }
}


//public bool ShowScoreboard()
//{
//    if (m_shouldRefreshScoreboard)
//    {
//        m_shouldRefreshScoreboard = false;

//        HttpClient.Instance.LoadScoreboard((response) =>
//        {

//            if (response.status.Equals("OK"))
//            {
//                for (int i = 0; i < response.data.Count; i++)
//                {
//                    var item = response.data[i];

//                    var url = HttpClient.Instance.BaseUrl;

//                    ScoreboardItemLoaded?.Invoke(i, item.name, url + item.profile_picture, item.score);
//                }

//            }

//            rMenu.HideSpinner();
//        });
//        return true;
//    }
//    return false;
//}
