using Assets.Scripts.DataMarts;
using UnityEngine;
using UnityEngine.UI;
public class PopupManager : MonoBehaviour
{
    public GameObject UiHolder;
    public GameObject UiHolder2;

    public GameObject UiOKButton;
    public GameObject UiBackToHomeButton;
    public GameObject UiShareProgressButton;
    public GameObject UiStatusIcon;
    public Text UiStatusText;
    public PopupPanel UiAskingOnLeaving;

    public GameObject UiScoreDisplayOnCorrectAnswer;

    public bool m_askingOnLeaving;
    private bool m_askingForLeavingConfirmed = false;


    // Start is called before the first frame update
    void Start()
    {
        UiHolder.SetActive(false);
        UiOKButton.SetActive(false);
        UiBackToHomeButton.SetActive(false);
        UiShareProgressButton.SetActive(false);
        UiStatusIcon.SetActive(false);

        UiAskingOnLeaving.m_closePanelCallback += ()=>Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCorrectAnswer(string buttonText, int score, int tobeAddedScore)
    {
        UiHolder.SetActive(true);
        UiHolder2.SetActive(true);
        UiStatusIcon.SetActive(true);
        UiStatusIcon.GetComponent<Image>().sprite = AssetsDataMart.Instance.constantsSO.correctAnswerSprite;
        UiStatusText.text = AssetsDataMart.Instance.constantsSO.stringsSO.correct;// "Xin Chuc Mung!!\nBan da hoan thanh xong bo cau hoi " + questionPackName;
        UiOKButton.SetActive(true);
        UiOKButton.GetComponentInChildren<Text>().text = buttonText;
        UiBackToHomeButton.SetActive(true);
        UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);

        UiScoreDisplayOnCorrectAnswer.SetActive(true);
        UiScoreDisplayOnCorrectAnswer.GetComponent<ScoreDisplay>().SetUp(tobeAddedScore, score);
    }
    public void ShowWrongAnswer(string buttonText, string statusText, int score)
    {
        UiHolder.SetActive(true);
        UiHolder2.SetActive(true);
        UiStatusIcon.SetActive(true);
        UiStatusIcon.GetComponent<Image>().sprite = AssetsDataMart.Instance.constantsSO.wrongAnswerSprite;
        UiOKButton.SetActive(true);
        UiOKButton.GetComponentInChildren<Text>().text = buttonText;// (isMCQ ? "CAU TIEP THEO":"THU LAI");
        UiStatusText.text = statusText;// "Xin Chuc Mung!!\nBan da hoan thanh xong bo cau hoi " + questionPackName;
        UiBackToHomeButton.SetActive(true);
        UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);

        UiScoreDisplayOnCorrectAnswer.SetActive(false);
    }
    public void ShowCongratePopup(string questionPackName, int score)
    {
        UiHolder.SetActive(true);
        UiHolder2.SetActive(true);
        UiStatusIcon.SetActive(true);
        UiStatusIcon.GetComponent<Image>().sprite = AssetsDataMart.Instance.constantsSO.congrateCupSprite;
        UiStatusText.text = AssetsDataMart.Instance.constantsSO.stringsSO.congrate+" " + questionPackName;
        UiOKButton.SetActive(false);
        UiBackToHomeButton.SetActive(true);
        UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);

        if (score != -1)
        {
            UiScoreDisplayOnCorrectAnswer.SetActive(true);
            UiScoreDisplayOnCorrectAnswer.GetComponent<ScoreDisplay>().SetUp(0, score).HideTobeAddedScore();
        }
    }
    //public void SetCorrectAnswer(string answer, bool isTimeout = false)
    //{
    //    UiCorrectAnswer.text = (isTimeout ? "Het Thoi Gian" : "Sai") + answer;
    //}

    public delegate void AskingForLeavingCallback(bool status);
    AskingForLeavingCallback m_askingForLeavingCallback;
    public void ShowAskingOnLeavingPopup(AskingForLeavingCallback callback)
    {
        m_askingForLeavingCallback = callback;
        UiHolder.SetActive(true);
        UiHolder2.SetActive(false);
        UiAskingOnLeaving.Show();
    }

    //public delegate void HideCallback();
    public void Hide(/*HideCallback callback = null*/)
    {
        if (!UiAskingOnLeaving.Hide(()=> { 
            UiHolder.SetActive(false);
            if (m_askingForLeavingCallback != null)
                m_askingForLeavingCallback(m_askingForLeavingConfirmed);
            m_askingForLeavingConfirmed = false;
            //callback();
        })) {
            UiHolder.SetActive(false);
            UiOKButton.SetActive(false);
            UiBackToHomeButton.SetActive(false);
            UiShareProgressButton.SetActive(false);
            UiStatusIcon.SetActive(false);

            //callback();
            if(m_answerPopupCallback!=null)
                m_answerPopupCallback(false);
        }
    }
    public delegate void AnswerPopupCallback(bool status);
    public AnswerPopupCallback m_answerPopupCallback = null;
    public void OnPanelClicked()
    {
        Hide();
    }
    public void OnPositiveButtonClicked()
    {
        m_answerPopupCallback(true);
    }
    public void OnAskingOnLeavingConfirmed()
    {
        m_askingForLeavingConfirmed = true;
        Hide();
    }
}
