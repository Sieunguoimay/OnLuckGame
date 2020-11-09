using Assets.Scripts.DataMarts;
using System;
using UnityEngine;
using UnityEngine.UI;
public class PopupManager : MonoBehaviour
{
    public GameObject UiHolder;
    public PopupPanel UiAskingOnLeaving;

    public GameObject UiHolder2;
    //public GameObject UiOKButton;
    //public GameObject UiBackToHomeButton;
    //public GameObject UiShareProgressButton;
    //public GameObject UiStatusIcon;
    public Text UiStatusText;
    //public GameObject UiScoreDisplayOnCorrectAnswer;

    public bool m_askingOnLeaving;
    private bool m_askingForLeavingConfirmed = false;

    public Action<bool> m_answerPopupCallback = delegate { };
    private Action<bool> m_askingForLeavingCallback = delegate { };


    // Start is called before the first frame update
    void Start()
    {
        UiHolder.SetActive(false);
        //UiOKButton.SetActive(false);
        //UiBackToHomeButton.SetActive(false);
        //UiShareProgressButton.SetActive(false);
        //UiStatusIcon.SetActive(false);

        //UiAskingOnLeaving.m_closePanelCallback += ()=>Hide();
    }

    public void ShowCorrectAnswer(string buttonText, int score, int tobeAddedScore)
    {
        UiHolder.SetActive(true);
        UiHolder2.SetActive(true);
        //UiStatusIcon.SetActive(true);
        //UiStatusIcon.GetComponent<Image>().sprite = AssetsDataMart.Instance.constantsSO.correctAnswerSprite;
        //UiStatusText.text = AssetsDataMart.Instance.constantsSO.stringsSO.correct;// "Xin Chuc Mung!!\nBan da hoan thanh xong bo cau hoi " + questionPackName;
        UiStatusText.text = "Chúc mừng, câu trả lời chính xác, bạn được +"+score+"UP";// statusText;// "Xin Chuc Mung!!\nBan da hoan thanh xong bo cau hoi " + questionPackName;
        //UiOKButton.SetActive(true);
        //UiOKButton.GetComponentInChildren<Text>().text = buttonText;
        //UiBackToHomeButton.SetActive(true);
        //UiShareProgressButton.SetActive(true);
        //UiAskingOnLeaving.m_holder.SetActive(false);

        //UiScoreDisplayOnCorrectAnswer.SetActive(true);
        //UiScoreDisplayOnCorrectAnswer.GetComponent<ScoreDisplay>().SetUp(tobeAddedScore, score);
    }
    public void ShowWrongAnswer(string buttonText, string statusText, int score)
    {
        UiHolder.SetActive(true);
        UiHolder2.SetActive(true);
        //UiStatusIcon.SetActive(true);
        //UiStatusIcon.GetComponent<Image>().sprite = AssetsDataMart.Instance.constantsSO.wrongAnswerSprite;
        //UiOKButton.SetActive(true);
        //UiOKButton.GetComponentInChildren<Text>().text = buttonText;// (isMCQ ? "CAU TIEP THEO":"THU LAI");
        UiStatusText.text = "Không chính xác!";// statusText;// "Xin Chuc Mung!!\nBan da hoan thanh xong bo cau hoi " + questionPackName;
        //UiBackToHomeButton.SetActive(true);
        //UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);

        //UiScoreDisplayOnCorrectAnswer.SetActive(false);
    }
    public void ShowCongratePopup(string questionPackName, int score)
    {
        UiHolder.SetActive(true);
        UiHolder2.SetActive(true);
        //UiStatusIcon.SetActive(true);
        //UiStatusIcon.GetComponent<Image>().sprite = AssetsDataMart.Instance.constantsSO.congrateCupSprite;
        UiStatusText.text = AssetsDataMart.Instance.constantsSO.stringsSO.congrate+" " + questionPackName;
        //UiOKButton.SetActive(false);
        //UiBackToHomeButton.SetActive(true);
        //UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);

        //if (score != -1)
        //{
        //    UiScoreDisplayOnCorrectAnswer.SetActive(true);
        //    UiScoreDisplayOnCorrectAnswer.GetComponent<ScoreDisplay>().SetUp(0, score).HideTobeAddedScore();
        //}
    }
    //public void SetCorrectAnswer(string answer, bool isTimeout = false)
    //{
    //    UiCorrectAnswer.text = (isTimeout ? "Het Thoi Gian" : "Sai") + answer;
    //}

    //public void ShowAskingOnLeavingPopup(Action<bool> callback)
    //{
    //    m_askingForLeavingCallback = callback;
    //    UiHolder.SetActive(true);
    //    UiHolder2.SetActive(false);
    //    UiAskingOnLeaving.Show();
    //}

    //public delegate void HideCallback();
    //public void Hide(/*HideCallback callback = null*/)
    //{
    //    if (!UiAskingOnLeaving.Hide(()=> { 
    //        UiHolder.SetActive(false);
            
    //        //m_askingForLeavingCallback?.Invoke(m_askingForLeavingConfirmed);

    //        m_askingForLeavingConfirmed = false;
    //    })) {
    //        UiHolder.SetActive(false);
    //        //UiOKButton.SetActive(false);
    //        //UiBackToHomeButton.SetActive(false);
    //        //UiShareProgressButton.SetActive(false);
    //        //UiStatusIcon.SetActive(false);

    //        //callback();
    //        //if(m_answerPopupCallback!=null)
    //        m_answerPopupCallback?.Invoke(false);
    //    }
    //}
    //public void OnPanelClicked()
    //{
    //    Hide();
    //}
    //public void OnPositiveButtonClicked()
    //{
    //    m_answerPopupCallback?.Invoke(true);
    //}
    //public void OnAskingOnLeavingConfirmed()
    //{
    //    m_askingForLeavingConfirmed = true;
    //    Hide();
    //}
}
