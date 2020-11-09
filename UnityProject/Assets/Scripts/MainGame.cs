using Assets.Scripts.DataMarts;
using Assets.Scripts.GameScene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    private MainGamePresenter m_mainGamePresenter;
    public GameObject UiQuestionListPanel;
    public GameObject UiQuestionPanel;

    public UserInfoBar userInfoBar;

    public Text UiSeasonText;
    public Text UiQuestionPackTitleText;
    public Text UiQuestionNumberText;
    public Text UiQuestionText;
    public Text UiQuestionTextCentre;
    public Image UiQuestionImage;
    public Button UiNextButton;
    public Button UiPrevButton;


    public GameObject UiEnteredAnswerPanel;
    public Text UiPrevSubmittedAnswerText;
    public InputField UiAnswerInputField;
    public Button UiSubmitAnswerButton;


    public GameObject UiMQCAnswerPanel;
    public CountdownTimer UiTimerText;
    public MCQAnswerItem[] mcqChoices;

    public Animator imageSlideShowAnimator;
    public ImageSlideshow UiImageSlideshow;
    //public GameObject UiImageItemTemplate;
    public GameObject UiSaveFeedbackText;

    public Text UiToastText;

    public PopupPanel popup;

    public UtilityButtons utilityButtons;

    public Button homeButton;

    void Awake()
    {
        UiQuestionListPanel.SetActive(false);

        UiSaveFeedbackText.SetActive(false);

        popup.HideImmediate();

        UiTimerText.m_timeoutCallback += OnMQCQuestionTimeout;

        m_mainGamePresenter = GetComponent<MainGamePresenter>();

        AudioController.Instance.PlaySoundOnEnterGameScene();

        Debug.Log("MainGame::Started");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_mainGamePresenter.Terminate();
        }
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
            if (m_mainGamePresenter.IsPlayingInMCQNow)
            {
                AskForLeavingTimingQuestion((success) =>
                {
                    if (success)
                    {
                        m_mainGamePresenter.MCQForceEnding();
                        m_mainGamePresenter.OpenQuestion(questionIndex);

                    }
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
        if (m_mainGamePresenter.IsPlayingInMCQNow)
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
        new NativeShare().SetText(AssetsDataMart.Instance.constantsSO.base_url).Share();
#endif
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

    public void OnAnswerButtonClicked()
    {
        m_mainGamePresenter.VerifyTypedAnswer(UiAnswerInputField.text);
    }
    public void OnMQCAnswerItemButtonClicked()
    {
        int answer = EventSystem.current.currentSelectedGameObject.GetComponent<MCQAnswerItem>().Index;

        m_mainGamePresenter.VerifyMCQAnswer(answer);
    }
    public void OnMQCQuestionTimeout()
    {
        Debug.Log("time's up babe");

        m_mainGamePresenter.MCQTimeout();
    }


    public void OnBackToMenuButtonClicked()
    {
        Debug.Log("End game");
        m_mainGamePresenter.Terminate();
    }

    /*Public functions goes here..*/

    public void ShowQuestionListPanel()
    {
        AudioController.Instance.PlayPanelOpenSound();

        UiQuestionListPanel.SetActive(true);

        var list = UiQuestionListPanel.GetComponent<ScrollList>();

        m_mainGamePresenter.LoadQuestionListIntoUiList(list);

        UiQuestionListPanel.GetComponent<Animator>().SetTrigger("show");
    }

    public void HideQuestionListPanel()
    {
        AudioController.Instance.PlayPanelOpenSound();

        UiQuestionListPanel.GetComponent<Animator>().SetTrigger("hide");

        HideObjectAfterAnimation(UiQuestionListPanel);
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
        UiSeasonText.text = name + "(" + unlocked + "/" + total + ")";
    }
    public void SetQuestionPackTitleText(string title)
    {
        UiQuestionPackTitleText.text = title;
    }
    public void SetQuestionNumber(int number)
    {
        UiQuestionNumberText.text = "CÂU " + number;
    }
    public void ShowQuestion(string question, List<QuestionDataMart.Image> images)
    {
        if (images.Count > 0)
        {
            UiQuestionText.text = question;

            UiImageSlideshow.SetQuestionImages(images);

            UiQuestionTextCentre.gameObject.SetActive(false);
        }
        else
        {
            UiQuestionText.text = "";
            UiQuestionTextCentre.gameObject.SetActive(true);
            UiQuestionTextCentre.text = question;
        }
    }
    public void SetQuestion(string question)
    {
        UiQuestionText.text = question;
    }
    //public void SetQuestionImages(List<QuestionDataMart.Image> images)
    //{
    //    foreach (Transform child in UiImageSlideshow.transform)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    images.ForEach((image) =>
    //    {
    //        var newItem = Instantiate(UiImageItemTemplate) as GameObject;
    //        newItem.GetComponent<Image>().sprite = image.sprite;
    //        newItem.transform.parent = UiImageSlideshow.transform;
    //    });

    //    UiImageSlideshow.scrollbar.value = 0;
    //    UiImageSlideshow.scroll_pos = 0;
    //    UiImageSlideshow.index = 0;
    //}
    public void SetPrevAnswer(string prevAnswer)
    {
        UiPrevSubmittedAnswerText.text = prevAnswer;
    }
    public void SetAnswerMode(MainGamePresenter.Modes mode)
    {
        if (mode == MainGamePresenter.Modes.TYPING_ANSWER)
        {
            UiEnteredAnswerPanel.SetActive(true);
            UiMQCAnswerPanel.SetActive(false);
        }
        else if (mode == MainGamePresenter.Modes.MCQ)
        {
            UiEnteredAnswerPanel.SetActive(false);
            UiMQCAnswerPanel.SetActive(true);
        }
    }

    public void ShowCorrectAnswerPopup(string buttonText, int score, int tobeAddedScore)
    {
        popup.Show(
            string.Format(AssetsDataMart.Instance.constantsSO.stringsSO.correct, tobeAddedScore).Replace("\\n", "\n"),
            buttonText, OnPopupResult, true);
    }

    public void ShowWrongMCQAnswer(string buttonText, int score)
    {
        popup.Show(
            string.Format(AssetsDataMart.Instance.constantsSO.stringsSO.incorrect_subtract, Mathf.Abs(score)).Replace("\\n", "\n"),
            buttonText, OnPopupResult, true);
    }
    public void ShowWrongTypedAnswer()
    {
        popup.Show(
            AssetsDataMart.Instance.constantsSO.stringsSO.incorrect.Replace("\\n", "\n"),
            AssetsDataMart.Instance.constantsSO.stringsSO.ok,
            OnPopupResult, false);
    }
    public void ShowCongratePopup(string questionPackName, int score)
    {
        popup.Show(
            AssetsDataMart.Instance.constantsSO.stringsSO.congrate,
            AssetsDataMart.Instance.constantsSO.stringsSO.ok,
            null);
    }
    public void ShowAnswerPopup(string answer)
    {
        popup.Show(
            string.Format(AssetsDataMart.Instance.constantsSO.stringsSO.the_answer_is, answer),
            AssetsDataMart.Instance.constantsSO.stringsSO.next_question,
            OnPopupResult, true);
    }
    public void OnPopupResult(bool status)
    {
        if (m_mainGamePresenter.IsLastQuestion &&
            m_mainGamePresenter.IsInMCQMode)
        {
            ShowCongratePopup(m_mainGamePresenter.GetCurrentQuestionPackName, PlayingDataMart.Instance.playingData.total_score);
        }
        if (status)
        {
            m_mainGamePresenter.NextQuestionAfterAnswering();
        }
    }


    public void ClearAnswerInputField()
    {
        UiAnswerInputField.text = "";
    }


    public void ShowMCQAnswer(int answer, int correctAnswer)
    {
        for (int i = 0; i < 4; i++)
        {
            var item = mcqChoices[i].GetComponent<MCQAnswerItem>();

            item.DisableButton();

            if (i == correctAnswer)
            {
                item.SetCorrectColor();
            }
            else if (i == answer)
            {
                item.SetWrongColor();
            }
        }
    }


    public void DisableAllUI()
    {
        LockAnswerPanel(false);

        LockNextButton(false);
        LockPrevButton(false);

        LockTopPanelUI(false);

        ClearTextUI();
        ClearMCQChoicesUI(false);
    }

    public void EnableAllUI()
    {
        LockAnswerPanel(true);

        LockNextButton(true);
        LockPrevButton(true);

        LockTopPanelUI(true);
    }

    public void EnableUIOnLoadQuestionFailed()
    {
        LockTopPanelUI(true);
    }

    public void ClearTextUI()
    {
        UiQuestionText.text = "";
        UiQuestionTextCentre.text = "";
        UiQuestionNumberText.text = "";
        UiAnswerInputField.text = "";
        UiPrevSubmittedAnswerText.text = "";
        imageSlideShowAnimator.gameObject.SetActive(false);
    }

    private void LockTopPanelUI(bool state)
    {
        homeButton.interactable = state;
    }

    public void LockAnswerPanel(bool state)
    {
        UiAnswerInputField.interactable = state;
        UiSubmitAnswerButton.interactable = state;

        utilityButtons.LockAllUI(state);
    }

    public void LockNextButton(bool status)
    {
        Debug.Log("SetUiNextButtonInteractable " + status);

        if (status)
        {
            UiNextButton.gameObject.SetActive(true);
            //UiNextButton.GetComponent<Animator>().SetTrigger("show");
        }
        else
        {
            UiNextButton.gameObject.SetActive(false);
            //UiNextButton.GetComponent<Animator>().SetTrigger("hide");
        }
    }

    public void LockPrevButton(bool status)
    {
        if (status)
        {
            UiPrevButton.gameObject.SetActive(true);
            //UiPrevButton.GetComponent<Animator>().SetTrigger("show");
        }
        else
        {
            UiPrevButton.gameObject.SetActive(false);
            //UiPrevButton.GetComponent<Animator>().SetTrigger("hide");
        }
    }


    public void SetMCQChoices(string[] choices)
    {
        for (int i = 0; i < 4; i++)
        {
            mcqChoices[i].SetAnswer(choices[i]);
        }
    }
    public void ClearMCQChoicesUI(bool interactable = true)
    {
        if (m_mainGamePresenter.IsInMCQMode)
        {
            for (int i = 0; i < 4; i++)
            {
                mcqChoices[i].Reset(interactable);
            }
        }
    }
    public void StartTimer(int time)
    {
        UiTimerText.Reset(time).Run();
    }
    public int StopTimer()
    {
        return UiTimerText.Pause();
    }
    public void ResetTimer(int time)
    {
        UiTimerText.Reset(time);
    }

    public void AskForLeavingTimingQuestion(Action<bool> callback)
    {

        popup.Show(
            AssetsDataMart.Instance.constantsSO.stringsSO.end_now,
            AssetsDataMart.Instance.constantsSO.stringsSO.ok,
            (result) => callback?.Invoke(result),true);
    }

    public void ShowSaveFeedback()
    {
        UiSaveFeedbackText.SetActive(true);

        StartCoroutine(saveFeedback(3.0f, () =>
        {

            UiSaveFeedbackText.SetActive(false);

        }));
    }
    public IEnumerator saveFeedback(float time, Action callback)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }
}



//public void ShowHintPanel(string hint)
//{
//    UiHintText.text = hint;
//    UiHintPopupPanel.SetActive(true);
//}

//public void HideHintPanel()
//{
//    UiHintPopupPanel.SetActive(false);
//}
//public void SetHintPriceActive(bool state)
//{
//    UiHintPriceText.transform.parent.gameObject.SetActive(state);
//    UiHintPriceText2.transform.parent.gameObject.SetActive(state);
//}
//public void SetHintPrice(int price)
//{
//    UiHintPriceText.text = "-" + price;
//    UiHintPriceText2.text = "-" + price;
//}