using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts;
using Assets.Scripts.DataMarts;
using Assets.Scripts.GameScene;
using UnityEditor.PackageManager;
using UnityEditor;

public class Menu : MonoBehaviour
{
    public GameObject UiLoginButtonPanel;
    public GameObject UiLoginButtonContentPanel;
    public GameObject UiQuestionPackItemPanel;
    public GameObject UiMenuPanel;
    public GameObject UiPopupPanelBackground;
    public GameObject UiPopupPanel;

    //the current panel to show in the Login panel
    private int m_currentPanelId;

    public GameObject UiLoginPanel;
    public InputField UiEmailInputField;
    public InputField UiPasswordInputField;
    public Text UiLoginStatusText;


    public GameObject UiSignupPanel;
    public InputField UiSignupNameInputField;
    public InputField UiSignupEmailInputField;
    public InputField UiSignupPasswordInputField;
    public Text UiSignupStatusText;

    public GameObject UiLogoutPanel;
    public Button UiUploadPhotoButton;
    public Button UiRenameButton;
    public Text UiSeasonText;

    public GameObject UiVerificationPanel;


    public GameObject UiSplashScenePanel;
    public GameObject UiSplashSceneSpinnerTargetPanel;
    public GameObject UiSplashSceneLogoPanel;
    public ProgressBar UiProgressBar;
    private bool hideSplashSceneLock;
    private bool progressBarLock;

    public GameObject UiUserInfoBar;
    public Image UiAvatarImage;
    public Text UiUserNameText;
    public Text UiScoreText;
    public GameObject UiRenameInputField;

    public Image UiSoundButton;

    public GameObject UiGuidelinePanel;
    public Text UiGuidelineTitleText;
    public Text UiGuidelineContentText;

    public GameObject UiScoreboardPanel;
    public GameObject UiScoreboardScrollList;

    private MenuPresenter m_menuPresenter;

    public ScrollList UiQuestionPackScrollList;
    public GameObject UiProfileMenuButtonList;
    public GameObject UiProfileMenuGuessText;

    public PopupPanel UiStartupPopupPanel;
    public Text UiStartupText;
    public Text UiStartupButtonText;

    public GameObject UiSpinner;

    public PopupPanel UiAskForPermissionPopupPanel;
    public Button UiAskForPermissionButton;
    public Text UiAskForPermissionText;

    public Text UiQuoteText;

    void Awake()
    {


        Debug.Log("Menu Awake");
        Utils.Instance.Init(this);
        Main.Instance.Init(this);
        UiSpinner.SetActive(false);

        //signal this on returning from MainGameScene
        MainGamePresenter.Instance.outputPlayingDataNeuron.inputs[1].Signal();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Menu Start");
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        /*Default State*/
        //ToggleUiAccordingToLoginState(false);
        UiPopupPanelBackground.SetActive(false);
        UiLoginPanel.SetActive(false);
        UiLogoutPanel.SetActive(false);
        UiSignupPanel.SetActive(false);
        UiGuidelinePanel.SetActive(false);
        UiSplashScenePanel.SetActive(false);
        UiVerificationPanel.SetActive(false);
        UiQuestionPackItemPanel.SetActive(false);
        UiRenameInputField.SetActive(false);
        UiScoreboardPanel.SetActive(false);
        UiProfileMenuButtonList.SetActive(false);
        UiProfileMenuGuessText.SetActive(false);

        UiRenameInputField.GetComponent<EnterKeyEvent>().eventCallback = () => OnRenameInputFieldEndEdit(UiRenameInputField.GetComponentInChildren<Text>().text);
        //UiProgressBar.value = 0;
        hideSplashSceneLock = true;
        progressBarLock = false;


        m_menuPresenter = MenuPresenter.Instance.Init(this);

        gameObject.AddComponent<AudioSource>();
        AssetsDataMart.Instance.rAudioSource = GetComponent<AudioSource>();
        Debug.Log("Menu Started");
    }
    void OnApplicationPause(bool status)
    {
        if (status)
        {
            m_menuPresenter.OnQuit();
        }
        else
        {
            //open up
        }
    }

    public void OnLoginButtonClicked()
    {
        ShowPopupPanel(0);
    }

    public void OnFBLoginButtonClicked()
    {
        m_menuPresenter.SignInWithFB();
    }

    public void OnSignupButtonClicked()
    {
        ShowPopupPanel(1);
        IsNormalSignup = true;
    }
    private bool IsNormalSignup = true;
    public delegate void ShowSignupPanelWithPredataCallback(string userName,string email, string password);
    private ShowSignupPanelWithPredataCallback showSignupPanelWithPredataCallback = null;
    public void ShowSignupPanelWithPredata(string name,string email, ShowSignupPanelWithPredataCallback callback)
    {
        IsNormalSignup = false;
        showSignupPanelWithPredataCallback = callback;
        SetSignupStatusText(AssetsDataMart.Instance.assetsData.strings.enter_new_password);
        UiSignupNameInputField.text = name;
        UiSignupEmailInputField.text = email;
        ShowPopupPanel(1);
    }

    public void OnSoundButtonClicked()
    {
        Debug.Log("OnSoundButtonClicked");
        m_menuPresenter.ToggleSound();
    }
    public void OnRatingButtonClicked()
    {
        Debug.Log("OnRatingButtonClicked");
        Application.OpenURL(HttpClient.Instance.BaseUrl);
    }
    public void OnGuidelineButtonClicked()
    {
        Debug.Log("OnGuidelineButtonClicked");
        ShowGuidelinePanel(AssetsDataMart.Instance.assetsData.strings.guideline, QuestionDataMart.Instance.onluckLocalMetadata.guideline_content);
        //ShowPopupPanel(5);
    }
    public void OnScoreboardButtonClicked()
    {
        Debug.Log("OnScoreboardButtonClicked");
        ShowPopupPanel(6);
        if(m_menuPresenter.ShowScoreboard())
            SetSpinnerTo(getPanelById(6));
    }
    public void OnIntroButtonClicked()
    {
        Debug.Log("OnIntroButtonClicked");
        //ShowPopupPanel(7);
        ShowGuidelinePanel(AssetsDataMart.Instance.assetsData.strings.intro, QuestionDataMart.Instance.onluckLocalMetadata.intro_content);
    }

    public void OnClosePupupButtonClicked()
    {
        Debug.Log("OnClosePupupButtonClicked");
        HidePopupPanel();
    }
    public void OnLoginSubmitButtonClicked()
    {
        Debug.Log("OnLoginSubmitButtonClicked: "+UiEmailInputField.text+" "+UiPasswordInputField.text);

        //Check for the authorization stuff. and then go back to the main menu with LoginButtonPanel been removed.
        if(m_menuPresenter.LogIn(UiEmailInputField.text, UiPasswordInputField.text))
        {
            UiEmailInputField.text = "";
            UiPasswordInputField.text = "";
        }
    }
    public void OnSignupSubmitButtonClicked()
    {
        Debug.Log("OnSignupSubmitButtonClicked: " + UiEmailInputField.text + " " + UiPasswordInputField.text);
        if (!IsNormalSignup)
        {
            showSignupPanelWithPredataCallback(UiSignupNameInputField.text, UiSignupEmailInputField.text, UiSignupPasswordInputField.text);
        }
        else
        {
            //Check for the authorization stuff. and then go back to the main menu with LoginButtonPanel been removed.
            if (m_menuPresenter.Signup(UiSignupNameInputField.text, UiSignupEmailInputField.text, UiSignupPasswordInputField.text))
            {
                UiSignupNameInputField.text = "";
                UiSignupEmailInputField.text = "";
                UiSignupPasswordInputField.text = "";
            }
        }
    }

    public void OnOpenEmailButtonClicked()
    {
        Debug.Log("OnOpenEmailButtonClicked:Openning email");

        Application.OpenURL("https://www.google.com/search?query=email");

    }
    public void OnVerificationOkButtonClicked()
    {
        Debug.Log("OnVerificationOkButtonClicked: Try to login once");
        m_menuPresenter.LogInWithLastEnteredAccount();
    }

    public void OnQuestionPackItemClicked()
    {
        int index = EventSystem.current.currentSelectedGameObject.GetComponent<QuestionPackItem>().Index;
        m_menuPresenter.OpenQuestionPack(index);
    }
    public void OnQuestionPackItemSubButtonClicked()
    {
        int index = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<QuestionPackItem>().Index;
        ShowPopupPanel(5);
    }
    public delegate void OpenStartupPanelCallback(bool status);
    private OpenStartupPanelCallback m_openStartupPanelCallback = null;
    public void OpenStartupPanel(string startupText,string buttonText, OpenStartupPanelCallback callback)
    {
        m_openStartupPanelCallback = callback;
        UiStartupText.text = startupText;
        UiStartupButtonText.text = buttonText;
        UiStartupPopupPanel.Show();
        UiStartupPopupPanel.m_closePanelCallback = ()=> {
            m_openStartupPanelCallback(false);
            UiStartupPopupPanel.Hide();
        };
    }
    public void OnStartupOKButtonClicked()
    {
        UiStartupPopupPanel.Hide(() => {
            m_openStartupPanelCallback(true);
        });
    }

    public void OnQuotePanelClicked()
    {
        ShowGuidelinePanel(AssetsDataMart.Instance.assetsData.strings.qoute_popup_title, UiQuoteText.text);
    }

    public void ToggleUiAccordingToLoginState(bool state)
    {
        if (UiQuestionPackItemPanel.activeSelf != state)
        {
            Debug.Log("ToggleUiAccordingToLoginState " + state);

            UiLoginButtonPanel.SetActive(!state);
            ToggleUserInfoBar(state);
            UiQuestionPackItemPanel.SetActive(state);
        }
    }

    public void ToggleUserInfoBar(bool state)
    {
        if (state)
        {
        }else
        {
            UiAvatarImage.sprite = AssetsDataMart.Instance.defaultProfilePictureSprite;
            UiUserNameText.text = AssetsDataMart.Instance.assetsData.strings.user_name;
            UiScoreText.text = 0.ToString();
        }
    }
    private IEnumerator hideUserInfoBar()
    {
        Animator animator = UiUserInfoBar.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("hide");
        }
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        UiUserInfoBar.SetActive(false);
    }

    public void HidePopupPanel()
    {
        StartCoroutine(hidePopupPanel());
    }
    public void ShowPopupPanel(int panelId)
    {
        togglePopupPanel(true, panelId);
        Animator animator = UiPopupPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("show");
        }

    }
    private IEnumerator hidePopupPanel()
    {
        Animator animator = UiPopupPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("hide");
            float animDuration = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(animDuration);
        }
        togglePopupPanel(false, m_currentPanelId);
    }
    private void togglePopupPanel(bool state, int panelId)
    {
        if (state)
            m_currentPanelId = panelId;


        UiPopupPanelBackground.SetActive(state);
        getPanelById(panelId)?.SetActive(state);


    }
    private GameObject getPanelById(int panelId)
    {
        switch (panelId)
        {
            case 0:
                return UiLoginPanel;
            case 1:
                return UiSignupPanel;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                return UiGuidelinePanel;
            case 6:
                return UiScoreboardPanel;
            case 7:
                break;
            case 8:
                return UiVerificationPanel;
        }
        return null;
    }
    public void ShowAutoHideSplashScene()
    {
        UiSplashScenePanel.SetActive(true);
        StartCoroutine(hideSplashSceneOnAnimationEnd());
    }
    private IEnumerator hideSplashSceneOnAnimationEnd()
    {
        Debug.Log("Animating Splash Scene Logo");
        yield return new WaitForSeconds(UiSplashSceneLogoPanel.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        if (hideSplashSceneLock)
            hideSplashSceneLock = false;
        else
            StartCoroutine(hideSplashScenePanel());
    }
    private IEnumerator hideSplashScenePanel()
    {
        Animator splashAnimator = UiSplashScenePanel.GetComponent<Animator>();
        if (splashAnimator != null)
        {
            splashAnimator.SetBool("hide", true);
            yield return new WaitForSeconds(0.5f);
        }
        UiSplashScenePanel.SetActive(false);
        UiSpinner.transform.SetParent(transform);
        UiSpinner.SetActive(false);
        Debug.Log("Splash scene is gone");

    }
    public void ShowSplashScene()
    {
        UiSplashScenePanel.SetActive(true);
        SetSpinnerTo(UiSplashSceneSpinnerTargetPanel);
    }
    public void HideSplashScene()
    {
        StartCoroutine(hideSplashScenePanel());
    }

    public void SetUserName(string name)
    {
        UiUserNameText.text = name;
    }
    public void SetAvatar(Sprite avatar)
    {
        UiAvatarImage.sprite = avatar;
    }
    public void SetAvatar(Texture2D avatar)
    {
        UiAvatarImage.sprite = Sprite.Create(avatar,new Rect(0, 0, avatar.width, avatar.height),new Vector2(0, 0));
    }
    public void SetAvatar(string url)
    {
        Utils.Instance.LoadImageIntoImage(url,UiAvatarImage);
    }
    public void SetScore(int score)
    {
        UiScoreText.text = score.ToString();
    }
    public void SetLoginStatusText(string status)
    {
        UiLoginStatusText.text = status;
    }
    public void SetSeasonText(string season)
    {
        UiSeasonText.text = season;
    }
    public void SetSignupStatusText(string status)
    {
        UiSignupStatusText.text = status;
    }
    public void ShowVerificationPanel()
    {
        getPanelById(m_currentPanelId).SetActive(false);
        UiVerificationPanel.SetActive(true);
        m_currentPanelId = 8;
    }
    public void OnAvatarButtonClicked()
    {

        UiProfileMenuButtonList.SetActive(!UiProfileMenuButtonList.activeSelf);
        if(UiProfileMenuButtonList.activeSelf)
           SetProfileButtonItemList(m_menuPresenter.m_loginState);
    }

    public void OnRenameInputFieldEndEdit(string text)
    {
        string t = UiRenameInputField.GetComponent<InputField>().text;
        m_menuPresenter.Rename(t,(status)=>
        {
            UiRenameInputField.GetComponent<InputField>().Select();
            UiRenameInputField.GetComponent<InputField>().text = "";
            UiRenameInputField.SetActive(false);
        });
    }
    public void OnCancelRenameButtonClicked()
    {
        UiRenameInputField.SetActive(false);
        UiRenameInputField.GetComponent<InputField>().Select();
        UiRenameInputField.GetComponent<InputField>().text = "";
    }
    public void ToggleSoundButtonUi(bool state)
    {
        if (state)
        {
            Debug.Log("Set Sound Button to Enabled");
            UiSoundButton.sprite = AssetsDataMart.Instance.soundIconSprite;
        }
        else
        {
            Debug.Log("Set Sound Button to Disabled");
            UiSoundButton.sprite = AssetsDataMart.Instance.soundOffIconSprite;
        }
    }

    public delegate void SetScoreboardCallback(ScoreboardItem item, int index);
    public void SetScoreboard(int size, SetScoreboardCallback callback)
    {
        UiScoreboardScrollList
            .GetComponent<ScrollList>()
            .CreateList<ScoreboardItem>(size, (item, index)=>callback(item,index));
    }
    public delegate void SetQuestionPacksCallback(QuestionPackItem item, int index);
    public void SetQuestionPacks(int size, SetQuestionPacksCallback callback)
    {
        UiQuestionPackScrollList
            .CreateList<QuestionPackItem>(size, (item, index) => callback(item, index));
    }
    public void SetSpinnerTo(GameObject parent)
    {
        //foreach (Transform child in parent.transform)
        //    child.gameObject.SetActive(false);
        UiSpinner.transform.SetParent(parent.transform);
        UiSpinner.transform.localPosition = Vector2.zero;
        UiSpinner.SetActive(true);
    }
    public void HideSpinner()
    {
        //foreach (Transform child in UiSpinner.transform.parent.transform)
        //    child.gameObject.SetActive(true);

        UiSpinner.transform.SetParent(transform);
        UiSpinner.SetActive(false);
        getPanelById(m_currentPanelId).SetActive(true);
    }
    public void ShowSpinnerAtPopupPanel()
    {
        SetSpinnerTo(UiPopupPanel);
        getPanelById(m_currentPanelId).SetActive(false);
    }
    public void ShowSpinnerOverTheScene()
    {
        UiSpinner.SetActive(true);
    }
    public void ToggleSpinnerAtLoginButtonPanel(bool state)
    {
        Debug.Log("ToggleSpinnerAtLoginButtonPanel " + state);
        if (state)
        {
            SetSpinnerTo(UiLoginButtonContentPanel.transform.parent.gameObject);
            UiLoginButtonContentPanel.SetActive(false);
        }
        else
        {
            UiLoginButtonContentPanel.SetActive(true);
            UiSpinner.transform.SetParent(transform);
            UiSpinner.SetActive(false);
        }
    }
    public void ShowGuidelinePanel(string title, string content)
    {
        ShowPopupPanel(5);
        UiGuidelineTitleText.text = title;
        UiGuidelineContentText.text = content;
    }

    public void SetProfileButtonItemList(bool state)
    {
        string[] profileMenuButtonNames = { 
            AssetsDataMart.Instance.assetsData.strings.upload_photo,
            AssetsDataMart.Instance.assetsData.strings.rename,
            AssetsDataMart.Instance.assetsData.strings.logout};
        UiProfileMenuGuessText.SetActive(false);
        if (state)
        {
            if (!UserDataMart.Instance.m_userData.active_vendor.Equals("sieunguoimay"))
            {
                UiProfileMenuButtonList.GetComponentInChildren<ScrollList>().CreateList(1, (item, index) =>
                {
                    IntegerCarrier integerCarrier =  item.AddComponent(typeof(IntegerCarrier)) as IntegerCarrier;
                    integerCarrier.integer = 2;
                    item.GetComponentInChildren<Text>().text = profileMenuButtonNames[2];
                });
            }
            else
            {
                UiProfileMenuButtonList.GetComponentInChildren<ScrollList>().CreateList(3, (item, index) =>
                {
                    IntegerCarrier integerCarrier = item.AddComponent(typeof(IntegerCarrier)) as IntegerCarrier;
                    integerCarrier.integer = index;
                    item.GetComponentInChildren<Text>().text = profileMenuButtonNames[index];
                });
            }
        }
        else
        {
            UiProfileMenuButtonList.GetComponentInChildren<ScrollList>().Clear();
            UiProfileMenuGuessText.SetActive(true);
        }
    }
    public void OnProfileButtonItemClicked()
    {
        int index = EventSystem.current.currentSelectedGameObject.GetComponent<IntegerCarrier>().integer;
        if (index == 0)
        {
            Debug.Log("Upload Photo");
            LocalProvider.Instance.BrowseImagePath((path) => {
                if(path!="")
                    m_menuPresenter.UploadProfilePicture(path);
            });
        }
        else if (index == 1)
        {
            Debug.Log("Rename");
            UiRenameInputField.SetActive(true);
            InputField input = UiRenameInputField.GetComponent<InputField>();
            input.Select();
            input.ActivateInputField();
            UiLogoutPanel.SetActive(false);
        }
        else if (index == 2)
        {
            Debug.Log("Logout");
            m_menuPresenter.Logout();
            UiLogoutPanel.SetActive(false);
        }
        UiProfileMenuButtonList.SetActive(false);
    }


    public delegate void AskForPermissionCallback();
    public AskForPermissionCallback askForPermissionCallback = null;
    public AskForPermissionCallback checkYourNetworkCallback = null;
    private int permissinPopupMode = -1;
    public void SetUpAskForPermissionPopup(AskForPermissionCallback callback)
    {
        askForPermissionCallback = callback;
        UiAskForPermissionButton.onClick.AddListener(() =>
        {
            if (permissinPopupMode == 0) 
                askForPermissionCallback();
            else if (permissinPopupMode == 1)
                checkYourNetworkCallback();
            UiAskForPermissionPopupPanel.Hide();
        });
    }
    public void SetUpCheckYourNetwork(AskForPermissionCallback callback)
    {
        checkYourNetworkCallback = callback;
    }
    public void ShowPopupToAskForPermission()
    {
        permissinPopupMode = 0;
        UiAskForPermissionButton.GetComponentInChildren<Text>().text = AssetsDataMart.Instance.assetsData.strings.download;
        //"TAI XUONG";
        UiAskForPermissionText.text = AssetsDataMart.Instance.assetsData.strings.new_game_data_available;
        //"Da co du lieu moi cho game. Vui long tai xuong!";
        UiAskForPermissionPopupPanel.Show();// m_holder.SetActive(true);
    }
    public void ShowPopupCheckYourNetwork()
    {
        permissinPopupMode = 1;
        UiAskForPermissionButton.GetComponentInChildren<Text>().text = AssetsDataMart.Instance.assetsData.strings.try_again;
        //"THU LAI";
        UiAskForPermissionText.text = AssetsDataMart.Instance.assetsData.strings.no_internet;
        //"Khong co internet!";
        UiAskForPermissionPopupPanel.Show();// m_holder.SetActive(true);
    }

    public void SetProgressBar(float value)
    {
        UiProgressBar.SlideTo(value);
    }
    public void SetQuoteText(string quote)
    {
        UiQuoteText.text = quote;
    }
}
