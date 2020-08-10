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

    void Awake()
    {
        Utils.Instance.Init(this);
        MainGamePresenter.Instance.Init(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        UiQuestionListPanel.SetActive(false);
        UiPopupPanel.SetActive(false);
        UiHintPopupPanel.SetActive(false);
        UiPopupPanel.GetComponent<PopupManager>().m_answerPopupCallback = OnPopupPanelClosed;

        UiHintPriceText.transform.parent.gameObject.SetActive(false);
        UiHintPriceText2.transform.parent.gameObject.SetActive(false);

        //ShowQuestionList();
        m_mainGamePresenter = MainGamePresenter.Instance;


        try
        {
            Debug.Log(UserDataMart.Instance.m_userData.name);

            Texture2D texture = UserDataMart.Instance.m_userData.texProfilePicture;
            UiUserNameText.text = UserDataMart.Instance.m_userData.name;
            UiScoreText.text = UserDataMart.Instance.m_userData.score.ToString();
            UiAvatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
        catch (Exception e)
        {
            //do nothing babe
        }


        UiTimerText.GetComponent<CountdownTimer>().Reset(10).m_timeoutCallback += OnMQCQuestionTimeout;

        //m_mainGamePresenter.InitWithTypingMode(QuestionDataMart.Instance.m_80QuestionPack, PlayingDataMart.Instance.m_questionImage80, PlayingDataMart.Instance.m_currentQuestionIndex80);
        m_mainGamePresenter.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        //UiTimerText.GetComponent<CountdownTimer>().Pause();



        if (m_mainGamePresenter.m_isPlayingInMCQNow)
        {
            AskForLeavingTimingQuestion((success) =>
            {
                if (success)
                {
                    Debug.Log("End game");
                    m_mainGamePresenter.Terminate();
                    SceneManager.LoadScene("menu");
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
            SceneManager.LoadScene("menu");
        }
    }
    public void OnShareProgressButtonClicked()
    {
        Debug.Log("OnShareProgressButtonClicked");
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
        m_mainGamePresenter.BuyHint();
    }
    public void OnSaveImageButtonClicked()
    {
        m_mainGamePresenter.SaveImageOfTheCurrentQuestion();
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
            ShowCongratePopup(m_mainGamePresenter.GetCurrentQuestionPackName());
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
        SceneManager.LoadScene("menu");
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
        UiQuestionListPanel.SetActive(true);
        ScrollList list = UiQuestionListPanel.GetComponent<ScrollList>();
        m_mainGamePresenter.LoadQuestionListIntoUiList(list);

        HideQuestionPanel();
    }
    public void HideQuestionListPanel()
    {
        UiQuestionListPanel.SetActive(false);
        ShowQuestionPanel();
    }

    public void ShowQuestionPanel()
    {
        UiQuestionPanel.SetActive(true);
    }
    public void HideQuestionPanel()
    {
        UiQuestionPanel.SetActive(false);
    }

    public void SetSeasonText(string text)
    {
        UiSeasonText.text = text;
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
    public void SetQuestionImage(Texture2D texture)
    {
        if(texture!=null)
            UiQuestionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
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
    //public void ShowAnswerResultPopupPanel(int state)
    //{
    //    answerResultState = state;
    //    //UiPopupPanel.SetActive(true);
    //    if (state == 0)
    //    {
    //        UiPopupPanel.GetComponent<PopupManager>().ShowCorrectAnswer();
    //    }
    //    else if(state == 1)
    //    {
    //        UiPopupPanel.GetComponent<PopupManager>().ShowWrongAnswer(m_mainGamePresenter.m_mode==MainGamePresenter.Modes.MCQ);
    //    }
    //    else if (state == 2)
    //    {
    //        //Show Congrate scene.

    //    }
    //}
    private int answerResultState = 0;
    public void ShowCorrectAnswer(string buttonText)
    {
        UiPopupPanel.GetComponent<PopupManager>().ShowCorrectAnswer(buttonText);
        answerResultState = 0;
    }
    public void ShowWrongAnswer(string buttonText, string statusText)
    {
        UiPopupPanel.GetComponent<PopupManager>().ShowWrongAnswer(buttonText, statusText);
        answerResultState = 1;
    }
    public void ShowCongratePopup(string questionPackName)
    {
        UiPopupPanel.GetComponent<PopupManager>().ShowCongratePopup(questionPackName);
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
    public void SetHintPrice(int price)
    {
        UiHintPriceText.transform.parent.gameObject.SetActive(true);
        UiHintPriceText2.transform.parent.gameObject.SetActive(true);
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
    public void SetHintUtInteractable(bool state)
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
        UiNextButton.interactable = status;
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
    public delegate void AskForLeavingTimingQuestionCallback(bool success);
    public void AskForLeavingTimingQuestion(AskForLeavingTimingQuestionCallback callback)
    {
        PopupManager popupManager = UiPopupPanel.GetComponent<PopupManager>();
        popupManager.m_askingOnLeaving = true;
        popupManager.ShowAskingOnLeavingPopup((success)=>callback(success));
    }
}
