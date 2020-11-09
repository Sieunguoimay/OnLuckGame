using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts;
using Assets.Scripts.DataMarts;
using Assets.Scripts.GameScene;
using System;

public class Menu : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject UiLoginButtonPanel;
    public GameObject UiLoginButtonContentPanel;
    public GameObject UiQuestionPackItemPanel;
    public GameObject UiMenuPanel;

    public GameObject UiPopupPanelBackground;
    //public GameObject UiPopupPanel;

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

    public UserInfoBar userInfoBar;

    public Image UiSoundButton;

    public GameObject UiGuidelinePanel;
    public Text UiGuidelineTitleText;
    public Text UiGuidelineContentText;

    public GameObject UiScoreboardPanel;

    private MenuPresenter m_menuPresenter;

    public ScrollList UiQuestionPackScrollList;

    public Spinner UiSpinner;

    public Text UiQuoteText;
    public Text UiLogText;

    private bool pressOneMoreTimeToExit;

    private bool IsNormalSignup = true;
    private Action<string, string, string> showSignupPanelWithPredataCallback;

    [SerializeField] private PopupController PopupController;
    [SerializeField] private PopupPanel simplePopup;

    private void Awake()
    {
        UiLogText.gameObject.SetActive(false);
        UiPopupPanelBackground.SetActive(false);
        UiLoginPanel.SetActive(false);
        UiLogoutPanel.SetActive(false);
        UiSignupPanel.SetActive(false);
        UiGuidelinePanel.SetActive(false);
        UiVerificationPanel.SetActive(false);
        UiQuestionPackItemPanel.SetActive(false);
        UiScoreboardPanel.SetActive(false);
        pressOneMoreTimeToExit = false;
        ToggleSoundButtonUi(Main.Instance.soundEnabled);
    }

    void Start()
    {
        Debug.Log("Menu Start");

        m_menuPresenter = GetComponent<MenuPresenter>();

        m_menuPresenter.ClosePopupPanel += PopupController.Hide;
        userInfoBar.AvatarClicked += OnAvatarClicked;
        PopupController.UserProfile.LogoutButtonClicked += m_menuPresenter.Logout;

        Debug.Log("Menu Started");
    }
    private void OnEnable()
    {
        QuestionDataMart.Instance.m_gameDataReadyCallback += HideLogText;
        LoginLogic.Instance.fbLoginCallback += ToggleSpinnerAtLoginButtonPanel;
        LoginLogic.Instance.fbLoginDoneCallback += HideSpinnerAtLoginButtonPanel;
    }
    private void OnDisable()
    {
        if (QuestionDataMart.Instance != null)
        {
            QuestionDataMart.Instance.m_gameDataReadyCallback -= HideLogText;
        }
        if (LoginLogic.Instance!=null)
        {
            LoginLogic.Instance.fbLoginCallback -= ToggleSpinnerAtLoginButtonPanel;
            LoginLogic.Instance.fbLoginDoneCallback -= HideSpinnerAtLoginButtonPanel;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pressOneMoreTimeToExit)
            {
                pressOneMoreTimeToExit = true;
                Toast.Instance.Show(Canvas, AssetsDataMart.Instance.constantsSO.stringsSO.press_one_more_time, 3, () =>
                {
                    pressOneMoreTimeToExit = false;

                });
            }
            else
            {
                Application.Quit();
            }
        }
    }
    public void OnAvatarClicked()
    {
        if (UserDataMart.Instance.m_isUserDataValid)
        {
            PopupController.ShowUserProfile();
        }
        else
        {
            simplePopup.Show(AssetsDataMart.Instance.constantsSO.stringsSO.pls_login, AssetsDataMart.Instance.constantsSO.stringsSO.ok, null);
        }
    }
    public void OnLoginButtonClicked()
    {
    }

    public void OnFBLoginButtonClicked()
    {
        m_menuPresenter.SignInWithFB();
    }

    public void OnSignupButtonClicked()
    {
        IsNormalSignup = true;
    }

    public void ShowSignupPanelWithPredata(string name, string email, Action<string, string, string> callback)
    {
        IsNormalSignup = false;
        showSignupPanelWithPredataCallback = callback;
        SetSignupStatusText(AssetsDataMart.Instance.constantsSO.stringsSO.enter_new_password);
        UiSignupNameInputField.text = name;
        UiSignupEmailInputField.text = email;
    }

    public void OnSoundButtonClicked()
    {
        m_menuPresenter.ToggleSound();
    }
    public void OnRatingButtonClicked()
    {
        Application.OpenURL(HttpClient.Instance.BaseUrl);
    }



    public void OnGuidelineButtonClicked()
    {
        PopupController.ShowGuideline(
            AssetsDataMart.Instance.constantsSO.stringsSO.guideline,
            QuestionDataMart.Instance.onluckLocalMetadata.guideline_content);
    }

    public void OnScoreboardButtonClicked()
    {
        PopupController.ShowScoreboard();
    }

    public void OnIntroButtonClicked()
    {
        PopupController.ShowGuideline(
            AssetsDataMart.Instance.constantsSO.stringsSO.intro,
            QuestionDataMart.Instance.onluckLocalMetadata.intro_content);
    }


    public void OnLoginSubmitButtonClicked()
    {
        Debug.Log("OnLoginSubmitButtonClicked: " + UiEmailInputField.text + " " + UiPasswordInputField.text);

        //Check for the authorization stuff. and then go back to the main menu with LoginButtonPanel been removed.
        if (m_menuPresenter.LogIn(UiEmailInputField.text, UiPasswordInputField.text))
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
            showSignupPanelWithPredataCallback?.Invoke(UiSignupNameInputField.text, UiSignupEmailInputField.text, UiSignupPasswordInputField.text);
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

    public void OpenStartupPanel(string startupText, string buttonText, Action<bool> callback)
    {
        simplePopup.Show(startupText, buttonText, callback);
    }
    //public void OnStartupOKButtonClicked()
    //{
    //    UiStartupPopupPanel.Hide(() => {
    //        m_openStartupPanelCallback(true);
    //    });
    //}

    public void OnQuotePanelClicked()
    {
        //ShowGuidelinePanel(AssetsDataMart.Instance.constantsSO.stringsSO.qoute_popup_title, UiQuoteText.text);
        PopupController.ShowGuideline(
            AssetsDataMart.Instance.constantsSO.stringsSO.qoute_popup_title,
            QuestionDataMart.Instance.onluckLocalMetadata.quote);
    }


    public void SetToDefaultUI()
    {
        UiQuestionPackItemPanel.SetActive(false);
        UiLoginButtonPanel.SetActive(true);
    }

    //private IEnumerator hideUserInfoBar()
    //{
    //    Animator animator = UiUserInfoBar.GetComponent<Animator>();
    //    if (animator != null)
    //    {
    //        animator.SetTrigger("hide");
    //    }
    //    yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    //    UiUserInfoBar.SetActive(false);
    //}

    //public void HidePopupPanel()
    //{
    //    StartCoroutine(hidePopupPanel());
    //}
    //public void ShowPopupPanel(int panelId)
    //{
    //    togglePopupPanel(true, panelId);

    //    UiPopupPanel.GetComponent<Animator>()?.SetTrigger("show");
    //}
    //private IEnumerator hidePopupPanel()
    //{
    //    Animator animator = UiPopupPanel.GetComponent<Animator>();

    //    if (animator != null)
    //    {
    //        animator.SetTrigger("hide");

    //        float animDuration = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

    //        yield return new WaitForSeconds(animDuration);
    //    }
    //    togglePopupPanel(false, m_currentPanelId);
    //}
    //private void togglePopupPanel(bool state, int panelId)
    //{
    //    if (state)
    //    {
    //        m_currentPanelId = panelId;
    //    }


    //    UiPopupPanelBackground.SetActive(state);
    //    getPanelById(panelId)?.SetActive(state);


    //}
    //private GameObject getPanelById(int panelId)
    //{
    //    switch (panelId)
    //    {
    //        case 0:
    //            return UiLoginPanel;
    //        case 1:
    //            return UiSignupPanel;
    //        case 2:
    //            break;
    //        case 3:
    //            break;
    //        case 4:
    //            break;
    //        case 5:
    //            return UiGuidelinePanel;
    //        case 6:
    //            return UiScoreboardPanel;
    //        case 7:
    //            break;
    //        case 8:
    //            return UiVerificationPanel;
    //    }
    //    return null;
    //}


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
        //getPanelById(m_currentPanelId).SetActive(false);
        //UiVerificationPanel.SetActive(true);
        //m_currentPanelId = 8;
    }


    public void ToggleSoundButtonUi(bool state)
    {
        if (state)
        {
            Debug.Log("Set Sound Button to Enabled");
            UiSoundButton.sprite = AssetsDataMart.Instance.constantsSO.soundIconSprite;
        }
        else
        {
            Debug.Log("Set Sound Button to Disabled");
            UiSoundButton.sprite = AssetsDataMart.Instance.constantsSO.soundOffIconSprite;
        }
    }
 

    private int spinnerIndex = -1;
    public void HideSpinner()
    {
        //foreach (Transform child in UiSpinner.transform.parent.transform)
        //    child.gameObject.SetActive(true);
        if (spinnerIndex == 0 || spinnerIndex == 1 || spinnerIndex == 6)
        {
            UiSpinner.Hide();
            //getPanelById(m_currentPanelId).SetActive(true);
        }
    }
    public void ShowSpinnerAtPopupPanel()
    {
        spinnerIndex = 0;
        //SetSpinnerTo(UiPopupPanel);
        //getPanelById(m_currentPanelId).SetActive(false);
    }
    public void ShowSpinnerOverTheScene()
    {
        spinnerIndex = 1;
        UiSpinner.Show(transform);
    }
    public void ToggleSpinnerAtLoginButtonPanel(bool state)
    {
        UiLoginButtonContentPanel.SetActive(false);
        UiSpinner.Show(UiLoginButtonContentPanel.transform.parent);
    }
    public void HideSpinnerAtLoginButtonPanel()
    {
        UiLoginButtonContentPanel.SetActive(true);
        UiSpinner.Hide();
    }
    //public void ShowGuidelinePanel(string title, string content)
    //{
    //    //ShowPopupPanel(5);
    //    UiGuidelineTitleText.text = title;
    //    UiGuidelineContentText.text = content;
    //}

    //public void SetProfileButtonItemList(bool state)
    //{
    //    string[] profileMenuButtonNames = { 
    //        AssetsDataMart.Instance.constantsSO.stringsSO.upload_photo,
    //        AssetsDataMart.Instance.constantsSO.stringsSO.rename,
    //        AssetsDataMart.Instance.constantsSO.stringsSO.logout};
    //    UiProfileMenuGuessText.SetActive(false);
    //    if (state)
    //    {
    //        if (!UserDataMart.Instance.m_userData.active_vendor.Equals("sieunguoimay"))
    //        {
    //            UiProfileMenuButtonList.GetComponentInChildren<ScrollList>().CreateList(1, (item, index) =>
    //            {
    //                IntegerCarrier integerCarrier =  item.AddComponent(typeof(IntegerCarrier)) as IntegerCarrier;
    //                integerCarrier.integer = 2;
    //                item.GetComponentInChildren<Text>().text = profileMenuButtonNames[2];
    //            });
    //        }
    //        else
    //        {
    //            UiProfileMenuButtonList.GetComponentInChildren<ScrollList>().CreateList(3, (item, index) =>
    //            {
    //                IntegerCarrier integerCarrier = item.AddComponent(typeof(IntegerCarrier)) as IntegerCarrier;
    //                integerCarrier.integer = index;
    //                item.GetComponentInChildren<Text>().text = profileMenuButtonNames[index];
    //            });
    //        }
    //    }
    //    else
    //    {
    //        UiProfileMenuButtonList.GetComponentInChildren<ScrollList>().Clear();
    //        UiProfileMenuGuessText.SetActive(true);
    //    }
    //}
    //public void OnProfileButtonItemClicked()
    //{
    //    int index = EventSystem.current.currentSelectedGameObject.GetComponent<IntegerCarrier>().integer;
    //    if (index == 0)
    //    {
    //        Debug.Log("Upload Photo");
    //        LocalProvider.Instance.BrowseImagePath((path) => {
    //            if(path!="")
    //                m_menuPresenter.UploadProfilePicture(path);
    //        });
    //    }
    //    else if (index == 1)
    //    {
    //        Debug.Log("Rename");
    //        UiRenameInputField.SetActive(true);
    //        InputField input = UiRenameInputField.GetComponent<InputField>();
    //        input.Select();
    //        input.ActivateInputField();
    //        UiLogoutPanel.SetActive(false);
    //    }
    //    else if (index == 2)
    //    {
    //        Debug.Log("Logout");
    //        m_menuPresenter.Logout();
    //        UiLogoutPanel.SetActive(false);
    //    }
    //    UiProfileMenuButtonList.SetActive(false);
    //}




    //private int permissinPopupMode = -1;

    //public void SetUpAskForPermissionPopup(Action callback)
    //{
    //    askForPermissionCallback = callback;

    //    UiAskForPermissionButton.onClick.AddListener(() =>
    //    {
    //        if (permissinPopupMode == 0)
    //        {
    //            askForPermissionCallback?.Invoke();
    //        }
    //        else if (permissinPopupMode == 1)
    //        {
    //            checkYourNetworkCallback?.Invoke();
    //        }

    //        UiAskForPermissionPopupPanel.Hide();
    //    });
    //}



    public void SetQuoteText(string quote)
    {
        UiQuoteText.text = quote;
    }
    public void ShowLogText(string text)
    {
        if (!UiLogText.gameObject.activeSelf)
        {
            UiLogText.gameObject.SetActive(true);
        }
        UiLogText.text = text;
    }
    public void HideLogText()
    {
        UiLogText.gameObject.SetActive(false);
    }
}
