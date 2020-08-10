using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts;
using Assets.Scripts.DataMarts;


public class Menu : MonoBehaviour
{
    public GameObject UiLoginButtonPanel;
    public GameObject UiPlayButton;
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

    public GameObject UiVerificationPanel;

    public GameObject UiGuidelinePanel;
    public Text UiGuidelineTitleText;
    public Text UiGuidelineContentText;

    public GameObject UiSplashScenePanel;
    public GameObject UiSplashSceneLogoPanel;

    public GameObject UiUserInfoBar;
    public Image UiAvatarImage;
    public Text UiUserNameText;
    public Text UiScoreText;
    public GameObject UiRenameInputField;

    public Image UiSoundButton;

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
    void Awake()
    {
        Debug.Log("Menu Awake");
        Utils.Instance.Init(this);
        UiSpinner.SetActive(false);
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
        UiPlayButton.SetActive(false);
        UiRenameInputField.SetActive(false);
        UiScoreboardPanel.SetActive(false);
        UiProfileMenuButtonList.SetActive(false);
        UiProfileMenuGuessText.SetActive(false);

        Debug.Log("Menu Start 1");

        m_menuPresenter = MenuPresenter.Instance.Init(this);
        Debug.Log("Menu Started");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnLoginButtonClicked()
    {
        Debug.Log("onLoginButtonClicked");
        ShowPopupPanel(0);
    }

    public void OnFBLoginButtonClicked()
    {
        Debug.Log("onFBLoginButtonClicked");
        m_menuPresenter.SignInWithFB();
    }

    public void OnSignupButtonClicked()
    {
        Debug.Log("onSignupButtonClicked");
        ShowPopupPanel(1);
    }

    //public void OnDonateButtonClicked()
    //{
    //    Debug.Log("onDonateButtonClicked");
    //    ShowPopupPanel(2);
    //}
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
        ShowGuidelinePanel(AssetsDataMart.Instance.guidelineTitles[0], AssetsDataMart.Instance.guidelines[0]);
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
        ShowGuidelinePanel(AssetsDataMart.Instance.guidelineTitles[1], AssetsDataMart.Instance.guidelines[1]);
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

        //Check for the authorization stuff. and then go back to the main menu with LoginButtonPanel been removed.
        if(m_menuPresenter.Signup(UiSignupNameInputField.text, UiSignupEmailInputField.text, UiSignupPasswordInputField.text))
        {
            UiSignupNameInputField.text = "";
            UiSignupEmailInputField.text = "";
            UiSignupPasswordInputField.text = "";
        }
    }

    public void OnOpenEmailButtonClicked()
    {
        Debug.Log("OnOpenEmailButtonClicked:Openning email");
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

    public void ToggleUiAccordingToLoginState(bool state)
    {
        if (UiPlayButton.activeSelf != state)
        {
            Debug.Log("ToggleUiAccordingToLoginState " + state);

            UiLoginButtonPanel.SetActive(!state);
            ToggleUserInfoBar(state);
            UiPlayButton.SetActive(state);
        }
    }

    public void ToggleUserInfoBar(bool state)
    {
        if (state)
        {
            //UiUserInfoBar.SetActive(state);
            //UiUserInfoBar.GetComponent<Animator>()?.SetTrigger("show");
        }else
        {
            //StartCoroutine(hideUserInfoBar());
            //Load default
            UiAvatarImage.sprite = AssetsDataMart.Instance.defaultProfilePictureSprite;
            UiUserNameText.text = "Guess";
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
    public void ToggleSplashScene(bool state)
    {
        UiSplashScenePanel.SetActive(state);
        if (state)
        {
            StartCoroutine(hideSplashSceneOnAnimationEnd());
        }
    }
    private IEnumerator hideSplashSceneOnAnimationEnd()
    {
        Debug.Log("Animating Splash Scene Logo");
        yield return new WaitForSeconds(UiSplashSceneLogoPanel.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);

        Animator splashAnimator = UiSplashScenePanel.GetComponent<Animator>();
        if (splashAnimator != null)
        {
            splashAnimator.SetBool("hide", true);
            //float duration = splashAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(0.5f);
        }
        UiSplashScenePanel.SetActive(false);
        Debug.Log("Splash scene is gone");
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
        //UiLogoutPanel.SetActive(!UiLogoutPanel.activeSelf);
        //if (UiLogoutPanel.activeSelf)
        //{
        //    UiUploadPhotoButton.interactable = m_menuPresenter.m_IsNormalLogin;
        //    UiRenameButton.interactable = m_menuPresenter.m_IsNormalLogin;
        //}
        UiProfileMenuButtonList.SetActive(!UiProfileMenuButtonList.activeSelf);
        if(UiProfileMenuButtonList.activeSelf)
           SetProfileButtonItemList(m_menuPresenter.m_loginState);
    }

    public void OnRenameInputFieldEndEdit(string text)
    {
        string t = UiRenameInputField.GetComponent<InputField>().text;
        m_menuPresenter.Rename(t);
        UiRenameInputField.SetActive(false);
        Debug.Log("Hey: "+t);
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
        UiSpinner.transform.SetParent(parent.transform);
        UiSpinner.SetActive(true);
    }
    public void HideSpinner()
    {
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
    public void ShowGuidelinePanel(string title, string content)
    {
        ShowPopupPanel(5);
        UiGuidelineTitleText.text = title;
        UiGuidelineContentText.text = content;
    }

    private string[] profileMenuButtonNames = { "Thay anh moi", "Sua ten", "Dang xuat" };
    public void SetProfileButtonItemList(MenuPresenter.LoginStates state)
    {
        UiProfileMenuGuessText.SetActive(false);
        if (state == MenuPresenter.LoginStates.LOGGED_IN)
        {
            UiProfileMenuButtonList.GetComponent<ScrollList>().CreateList<Button>(3, (item, index) =>
            {
                item.GetComponentInChildren<Text>().text = profileMenuButtonNames[index];
            });
        }
        else if(state == MenuPresenter.LoginStates.LOGGED_IN_WITH_FB)
        {
            UiProfileMenuButtonList.GetComponent<ScrollList>().CreateList<Button>(1, (item, index) =>
            {
                item.GetComponentInChildren<Text>().text = profileMenuButtonNames[2];
            });
        }
        else
        {
            UiProfileMenuButtonList.GetComponent<ScrollList>().Clear();
            UiProfileMenuGuessText.SetActive(true);
        }
    }
    public void OnProfileButtonItemClicked()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
        if (buttonName.Equals(profileMenuButtonNames[0]))
        {
            Debug.Log("Upload Photo");
            LocalProvider.Instance.BrowseImagePath((path) => {
                if(path!="")
                    m_menuPresenter.UploadProfilePicture(path);
            });
        }
        else if (buttonName.Equals(profileMenuButtonNames[1]))
        {
            Debug.Log("Rename");
            UiRenameInputField.SetActive(true);
            InputField input = UiRenameInputField.GetComponent<InputField>();
            input.Select();
            input.ActivateInputField();
            UiLogoutPanel.SetActive(false);
        }
        else if (buttonName.Equals(profileMenuButtonNames[2]))
        {
            Debug.Log("Logout");
            m_menuPresenter.Logout();
            UiLogoutPanel.SetActive(false);
        }
        UiProfileMenuButtonList.SetActive(false);
    }
}
