using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Assets.Scripts.GameScene;
using Assets.Scripts;
using Assets.Scripts.DataMarts;
public class MainGame : MonoBehaviour
{
    private MainGamePresenter m_mainGamePresenter;
    public GameObject UiQuestionListPanel;
    public GameObject UiQuestionPanel;


    public Image UiAvatarImage;
    public Text UiUserNameText;
    public Text UiScoreText;


    public Text UiSeasonText;
    public Text UiQuestionPackTitleText;
    public Text UiQuestionNumberText;
    public Text UiQuestionText;
    public Image UiQuestionImage;
    public Button UiNextButton;
    public Button UiPrevButton;


    public GameObject UiEnteredAnswerPanel;
    public Text UiPrevAnswerText;
    public InputField UiAnswerInputField;
    public Button UiSubmitAnswerButton;


    public GameObject UiMQCAnswerPanel;
    public Text UiTimerText;
    public GameObject[] UiMCQAnswerImage;

    public GameObject UiPopupPanel;


    public GameObject UiHintPopupPanel;
    public Text UiHintText;
    public Text UiHintPriceText;
    public Text UiHintPriceText2;
    public Button UiHintButton1;
    public Button UiHintButton2;

    public GameObject UiImageSlideshow;
    public GameObject UiImageItemTemplate;
    public GameObject UiSaveFeedbackText;

    void Awake()
    {
        Utils.Instance.Init(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        UiImageItemTemplate.transform.parent = null;
        UiQuestionListPanel.SetActive(false);
        UiPopupPanel.SetActive(false);
        UiHintPopupPanel.SetActive(false);
        UiHintPriceText.transform.parent.gameObject.SetActive(false);
        UiHintPriceText2.transform.parent.gameObject.SetActive(false);
        UiTimerText.GetComponent<CountdownTimer>().m_timeoutCallback += OnMQCQuestionTimeout;
        UiPopupPanel.GetComponent<PopupManager>().m_answerPopupCallback = OnPopupPanelClosed;
 
        m_mainGamePresenter = MainGamePresenter.Instance.Ready(this);

        gameObject.AddComponent<AudioSource>().PlayOneShot(AssetsDataMart.Instance.panelOpenAudioClip);
        AssetsDataMart.Instance.rAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnApplicationPause()
    {
        MenuPresenter.Instance.OnQuit();
        MainGamePresenter.Instance.OnQuit();
    }

    /*Event list goes here..*/

    public void OnQuestionListPanelBackButtonClicked()
    {
        HideQuestionListPanel();
    }

    public void OnQuestionItemClicked()
    {
        HideQuestionListPanel();


        int questionIndex = EventSystem.current.currentSelectedGameObject.GetComponent<QuestionItem>().Index;
        if (!m_mainGamePresenter.CheckCurrentQuestionIndex(questionIndex))
        {
            if (m_mainGamePresenter.m_isPlayingInMCQNow)
            {
                AskForLeavingTimingQuestion((success) => {
                    if (success)
                        m_mainGamePresenter.OpenQuestion(questionIndex);
                });
            }
            else
            {
                m_mainGamePresenter.OpenQuestion(questionIndex);
            }
        }
        Debug.Log(questionIndex);
    }
    public void OnMainSceneBackButtonClicked()
    {
        if (m_mainGamePresenter.m_isPlayingInMCQNow)
        {
            AskForLeavingTimingQuestion((success) =>
            {
                if (success)
                {
                    Debug.Log("End game");
                    m_mainGamePresenter.Terminate();
                }
                else
                {
                    //UiTimerText.GetComponent<CountdownTimer>().Resume();
                }
            });
        }
        else
        {
            Debug.Log("End game");
            m_mainGamePresenter.Terminate();
        }
    }
    public void OnShareProgressButtonClicked()
    {
        Debug.Log("OnShareProgressButtonClicked");

#if UNITY_ANDROID || UNITY_IOS
        new NativeShare().SetText(AssetsDataMart.Instance.assetsData.base_url).Share();
#endif
    }
    public void PlayAudio(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
    public void OnShowQuestionListClicked()
    {
        ShowQuestionListPanel();
    }
    public void OnNextButtonClicked()
    {
        m_mainGamePresenter.NextQuestion();
    }
    public void OnPrevButtonClicked()
    {
        m_mainGamePresenter.PrevQuestion();
    }
    public void OnHintButtonClicked()
    {
        if (!UiHintPopupPanel.activeSelf)
            m_mainGamePresenter.BuyHint();
        else
            HideHintPanel();
    }
    public void OnSaveImageButtonClicked()
    {
        m_mainGamePresenter.SaveImageOfTheCurrentQuestion(UiImageSlideshow.GetComponent<ImageSlideshow>().currentIndex);
    }
    public void OnAnswerButtonClicked()
    {
        string answer = UiAnswerInputField.text;
        m_mainGamePresenter.VerifyTypedAnswer(answer);
        //Debug.Log(answer);
    }
    public void OnMQCAnswerItemButtonClicked()
    {
        int answer = EventSystem.current.currentSelectedGameObject.GetComponent<MCQAnswerItem>().Index;
        m_mainGamePresenter.VerifyMCQAnswer(answer);
        //Debug.Log(answer);
    }
    public void OnMQCQuestionTimeout()
    {
        Debug.Log("time's up babe");
        m_mainGamePresenter.MCQTimeout();
    }

    //public void OnPopupPanelClicked()
    //{
    //    UiPopupPanel.GetComponent<PopupManager>().Hide();
    //}
    public void OnPopupPanelClosed(bool status)
    {
        if (m_mainGamePresenter.IsLastQuestion() && m_mainGamePresenter.IsInMCQMode())
        {
            ShowCongratePopup(m_mainGamePresenter.GetCurrentQuestionPackName(),PlayingDataMart.Instance.playingData.total_score);
        }
        if (status)
        {
            m_mainGamePresenter.NextQuestionAfterAnswering();
        }
        else
        {

        }
    }
    //public void OnOkButtonClicked()
    //{
    //    m_mainGamePresenter.NextQuestionAfterAnswering();
    //}
    public void OnBackToMenuButtonClicked()
    {
        Debug.Log("End game");
        m_mainGamePresenter.Terminate();
    }
    public void OnCloseHintPanelButtonClicked()
    {
        UiHintPopupPanel.SetActive(false);
    }
    public void OnAvatarButtonClicked()
    {
        //m_mainGamePresenter.OpenQuestion(0);
    }



    /*Public functions goes here..*/

    public void ShowQuestionListPanel()
    {
        PlayAudio(AssetsDataMart.Instance.panelOpenAudioClip);

        UiQuestionListPanel.SetActive(true);
        ScrollList list = UiQuestionListPanel.GetComponent<ScrollList>();
        m_mainGamePresenter.LoadQuestionListIntoUiList(list);

        UiQuestionListPanel.GetComponent<Animator>().SetTrigger("show");
        //HideQuestionPanel();
    }
    public void HideQuestionListPanel()
    {
        PlayAudio(AssetsDataMart.Instance.panelOpenAudioClip) ;
        UiQuestionListPanel.GetComponent<Animator>().SetTrigger("hide");
        HideObjectAfterAnimation(UiQuestionListPanel);
        //UiQuestionListPanel.SetActive(false);
        //ShowQuestionPanel();
    }

    private void HideObjectAfterAnimation(GameObject gameObject)
    {
        StartCoroutine(hideObjectAfterAnimation(gameObject));
    }
    private IEnumerator hideObjectAfterAnimation(GameObject gameObject)
    {
        yield return new WaitForSeconds(gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        gameObject.SetActive(false);
    }
    public void ShowQuestionPanel()
    {
        UiQuestionPanel.SetActive(true);
    }
    public void HideQuestionPanel()
    {
        UiQuestionPanel.SetActive(false);
    }

    public void SetSeasonText(string name, int unlocked, int total)
    {
        UiSeasonText.text = name +"(" + unlocked + "/" + total + ")";
    }
    public void SetQuestionPackTitleText(string title)
    {
        UiQuestionPackTitleText.text = title;
    }
    public void SetQuestionNumber(int number)
    {
        UiQuestionNumberText.text = "CAU " + number;
    }
    public void SetQuestion(string question)
    {
        UiQuestionText.text = question;
    }
    public void SetQuestionImages(List<QuestionDataMart.Image>images)
    {
        foreach (Transform child in UiImageSlideshow.transform)
            Destroy(child.gameObject);


        images.ForEach((image) =>
        {
            //Texture2D texture = image.sprite.texture;
            GameObject newItem = Instantiate(UiImageItemTemplate) as GameObject;
            newItem.GetComponent<Image>().sprite = image.sprite;// Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            newItem.transform.SetParent(UiImageSlideshow.transform);
        });
        UiImageSlideshow.GetComponent<ImageSlideshow>().scrollbar.GetComponent<Scrollbar>().value = 0;
        UiImageSlideshow.GetComponent<ImageSlideshow>().scroll_pos = 0;
    }
    public void SetPrevAnswer(string prevAnswer)
    {
        UiPrevAnswerText.text = prevAnswer;
    }
    public void SetAnswerMode(MainGamePresenter.Modes mode)
    {
        if (mode == MainGamePresenter.Modes.TYPING_ANSWER)
        {
            UiEnteredAnswerPanel.SetActive(true);
            UiMQCAnswerPanel.SetActive(false);
        }
        else if(mode == MainGamePresenter.Modes.MCQ)
        {
            UiEnteredAnswerPanel.SetActive(false);
            UiMQCAnswerPanel.SetActive(true);
        }
    }
    public void HideAnswerResultPopupPanel()
    {
        UiPopupPanel.GetComponent<PopupManager>().Hide();
        //UiPopupPanel.SetActive(false);
    }
  
    private int answerResultState = 0;
    public void ShowCorrectAnswer(string buttonText, int score, int tobeAddedScore)
    {
        UiPopupPanel.GetComponent<PopupManager>().ShowCorrectAnswer(buttonText,score, tobeAddedScore);
        answerResultState = 0;
    }
    public void ShowWrongAnswer(string buttonText, string statusText, int score )
    {
        UiPopupPanel.GetComponent<PopupManager>().ShowWrongAnswer(buttonText, statusText,score);
        answerResultState = 1;
    }
    public void ShowCongratePopup(string questionPackName, int score)
    {
        UiPopupPanel.GetComponent<PopupManager>().ShowCongratePopup(questionPackName, score);
        answerResultState = 2;
    }

    public void ClearAnswerInputField()
    {
        UiAnswerInputField.text = "";
    }

    public void ShowHintPanel(string hint)
    {
        UiHintText.text = hint;
        UiHintPopupPanel.SetActive(true);
    }

    public void HideHintPanel()
    {
        UiHintPopupPanel.SetActive(false);
    }
    public void SetHintPriceActive(bool state)
    {
        UiHintPriceText.transform.parent.gameObject.SetActive(state);
        UiHintPriceText2.transform.parent.gameObject.SetActive(state);
    }
    public void SetHintPrice(int price)
    {
        UiHintPriceText.text = "-" + price;
        UiHintPriceText2.text = "-" + price;
    }
    public void ShowMCQAnswer(int answer,int correctAnswer)
    {
        for (int i = 0; i < 4; i++)
        {
            MCQAnswerItem item = UiMCQAnswerImage[i].GetComponent<MCQAnswerItem>();
            item.DisableButton();
            if(i == correctAnswer)
                item.SetCorrectColor();
            else if (i == answer)
                item.SetWrongColor();
        }
    }
    public void SetHintUiInteractable(bool state)
    {
        UiHintButton1.interactable = state;
        UiHintButton2.interactable = state;
        UiHintPriceText.transform.parent.gameObject.SetActive(false);
        UiHintPriceText2.transform.parent.gameObject.SetActive(false);
        //SetUiNextButtonInteractable(!state);
    }
    public void SetUiInteractableInTypingMode(bool state)
    {
        UiAnswerInputField.interactable = state;
        UiSubmitAnswerButton.interactable = state;
    }
    public void SetUiNextButtonInteractable(bool status)
    {
        //UiNextButton.interactable = status;
        Debug.Log("SetUiNextButtonInteractable " + status);
        if (status)
            UiNextButton.GetComponent<Animator>().SetTrigger("show");
        else
            UiNextButton.GetComponent<Animator>().SetTrigger("hide");
    }
    public void SetUiPrevButtonInteractable(bool status)
    {
        //UiNextButton.interactable = status;
        if (status)
            UiPrevButton.GetComponent<Animator>().SetTrigger("show");
        else
            UiPrevButton.GetComponent<Animator>().SetTrigger("hide");
    }
    public void SetMCQChoices(string[] choices)
    {
        for(int i = 0; i<4; i++)
        {
            UiMCQAnswerImage[i].GetComponent<MCQAnswerItem>().SetAnswer(choices[i]);
        }
    }
    public void ClearMCQChoicesColor()
    {
        for (int i = 0; i < 4; i++)
        {
            UiMCQAnswerImage[i].GetComponent<MCQAnswerItem>().Reset();
        }
    }
    public void StartTimer(int time)
    {
        UiTimerText.GetComponent<CountdownTimer>().Reset(time).Run();
    }
    public int StopTimer()
    {
        return UiTimerText.GetComponent<CountdownTimer>().Pause();
    }
    public void ResetTimer(int time)
    {
        UiTimerText.GetComponent<CountdownTimer>().Reset(time);
    }
    public void SetScore(int score)
    {
        UiScoreText.text = score.ToString();
    }
    public void SetAvatar(Texture2D texture)
    {
        UiAvatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    }
    public void SetUserName(string name)
    {
        UiUserNameText.text = name;
    }
    public delegate void AskForLeavingTimingQuestionCallback(bool success);
    public void AskForLeavingTimingQuestion(AskForLeavingTimingQuestionCallback callback)
    {
        PopupManager popupManager = UiPopupPanel.GetComponent<PopupManager>();
        popupManager.m_askingOnLeaving = true;
        popupManager.ShowAskingOnLeavingPopup((success)=>callback(success));
    }

    public void ShowSaveFeedback()
    {
        UiSaveFeedbackText.SetActive(true);
        UiSaveFeedbackText.GetComponent<Animator>().SetTrigger("show");
        StartCoroutine(saveFeedback(UiSaveFeedbackText.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length,()=> {
            UiSaveFeedbackText.GetComponent<Animator>().SetTrigger("hide");
            StartCoroutine(saveFeedback(UiSaveFeedbackText.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length, () => {
                UiSaveFeedbackText.SetActive(false);
            }));
        }));
    }
    public delegate void showSaveFeedbackCallback();
    public IEnumerator saveFeedback(float time,showSaveFeedbackCallback callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
