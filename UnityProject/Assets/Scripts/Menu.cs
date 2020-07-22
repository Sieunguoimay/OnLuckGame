using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class Menu : MonoBehaviour
{
    public GameObject UiLoginButtonPanel;
    public GameObject UiPlayButton;
    public GameObject UiMenuPanel;
    public GameObject UiPopupPanelBackground;
    public GameObject UiPopupPanel;
    public GameObject UiLogoutPanel;
    public GameObject UiUserInfoBar;

    //the current panel to show in the Login panel
    private int m_currentPanelId;

    public GameObject UiLoginPanel;
    public InputField UiEmailInputField;
    public InputField UiPasswordInputField;


    public GameObject UiSignupPanel;
    public InputField UiSignupNameInputField;
    public InputField UiSignupEmailInputField;
    public InputField UiSignupPasswordInputField;

    public GameObject UiGuidelinePanel;

    public GameObject UiSplashScenePanel;
    public GameObject UiSplashSceneLogoPanel;

    public Image UiAvatarImage;
    public Text UiUserNameText;

    private MenuPresenter m_menuPresenter;

    void Awake()
    {
        Debug.Log("Menu Awake");
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


        m_menuPresenter = MenuPresenter.Instance.Init(this);
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
        m_menuPresenter.LoginWithFB();
    }

    public void OnSignupButtonClicked()
    {
        Debug.Log("onSignupButtonClicked");
        ShowPopupPanel(1);
    }

    public void OnDonateButtonClicked()
    {
        Debug.Log("onDonateButtonClicked");
        ShowPopupPanel(2);
    }
    public void OnSoundButtonClicked()
    {
        Debug.Log("OnSoundButtonClicked");
        m_menuPresenter.ToggleSound();
    }
    public void OnRatingButtonClicked()
    {
        Debug.Log("OnRatingButtonClicked");
    }
    public void OnGuidelineButtonClicked()
    {
        Debug.Log("OnGuidelineButtonClicked");
        ShowPopupPanel(5);
    }
    public void OnScoreboardButtonClicked()
    {
        Debug.Log("OnScoreboardButtonClicked");
        ShowPopupPanel(6);
    }
    public void OnIntroButtonClicked()
    {
        Debug.Log("OnIntroButtonClicked");
        ShowPopupPanel(7);
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
        m_menuPresenter.VerifyUser(UiEmailInputField.text, UiPasswordInputField.text);

        UiEmailInputField.text = "";
        UiPasswordInputField.text = "";
    }
    public void OnSignupSubmitButtonClicked()
    {
        Debug.Log("OnSignupSubmitButtonClicked: " + UiEmailInputField.text + " " + UiPasswordInputField.text);

        //Check for the authorization stuff. and then go back to the main menu with LoginButtonPanel been removed.
        m_menuPresenter.Signup(UiSignupNameInputField.text, UiSignupEmailInputField.text, UiSignupPasswordInputField.text);

        UiSignupNameInputField.text = "";
        UiSignupEmailInputField.text = "";
        UiSignupPasswordInputField.text = "";
    }

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("main_game");
    }
    public void ToggleUiAccordingToLoginState(bool state)
    {
        Debug.Log("ToggleUiAccordingToLoginState " + state);
        UiLoginButtonPanel.SetActive(!state);
        ToggleUserInfoBar(state);
        UiPlayButton.SetActive(state);
    }

    public void ToggleUserInfoBar(bool state)
    {
        if (state)
        {
            UiUserInfoBar.SetActive(state);
            UiUserInfoBar.GetComponent<Animator>()?.SetBool("show", state);
        }else
        {
            StartCoroutine(hideUserInfoBar());
        }
    }
    private IEnumerator hideUserInfoBar()
    {
        Animator animator = UiUserInfoBar.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("show", false);
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
            animator.SetBool("open", true);
        }

    }
    private IEnumerator hidePopupPanel()
    {
        Animator animator = UiPopupPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("open", false);
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
        switch (panelId)
        {
            case 0:
                UiLoginPanel.SetActive(state);
                break;
            case 1:
                UiSignupPanel.SetActive(state);
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                UiGuidelinePanel.SetActive(state);
                break;
            case 6:
                break;
        }

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

    public void OnAvatarButtonClicked()
    {
        UiLogoutPanel.SetActive(!UiLogoutPanel.activeSelf);
    }
    public void OnLogoutButtonClicked()
    {
        m_menuPresenter.Logout();
        UiLogoutPanel.SetActive(false);
    }
    public void ToggleSoundButtonUi(bool state)
    {
        if (state)
        {
            Debug.Log("Set Sound Button to Enabled");
        }else
        {
            Debug.Log("Set Sound Button to Disabled");
        }
    }

}
