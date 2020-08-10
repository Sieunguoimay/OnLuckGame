using Assets.Scripts.DataMarts;
using UnityEngine;
using UnityEngine.UI;
public class PopupManager : MonoBehaviour
{
    public GameObject UiHolder;

    public GameObject UiOKButton;
    public GameObject UiBackToHomeButton;
    public GameObject UiShareProgressButton;
    public GameObject UiCorrectImage;
    public GameObject UiWrongImage;
    public Text UiStatusText;
    public PopupPanel UiAskingOnLeaving;


    public bool m_askingOnLeaving;
    private bool m_askingForLeavingConfirmed = false;


    // Start is called before the first frame update
    void Start()
    {
        UiHolder.SetActive(false);
        UiOKButton.SetActive(false);
        UiBackToHomeButton.SetActive(false);
        UiShareProgressButton.SetActive(false);
        //UiCorrectImage.SetActive(false);
        UiWrongImage.SetActive(false);

        UiAskingOnLeaving.m_closePanelCallback += ()=>Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCorrectAnswer(string buttonText)
    {
        UiHolder.SetActive(true);
        //UiCorrectImage.SetActive(true);
        UiCorrectImage.GetComponent<Image>().sprite = AssetsDataMart.Instance.correctAnswerSprite;
        UiStatusText.text = "CHINH XAC";// "Xin Chuc Mung!!\nBan da hoan thanh xong bo cau hoi " + questionPackName;
        UiOKButton.SetActive(true);
        UiOKButton.GetComponentInChildren<Text>().text = buttonText;
        UiBackToHomeButton.SetActive(true);
        UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);
    }
    public void ShowWrongAnswer(string buttonText, string statusText)
    {
        UiHolder.SetActive(true);
        //UiWrongImage.SetActive(true);
        UiCorrectImage.GetComponent<Image>().sprite = AssetsDataMart.Instance.wrongAnswerSprite;
        UiOKButton.SetActive(true);
        UiOKButton.GetComponentInChildren<Text>().text = buttonText;// (isMCQ ? "CAU TIEP THEO":"THU LAI");
        UiStatusText.text = statusText;// "Xin Chuc Mung!!\nBan da hoan thanh xong bo cau hoi " + questionPackName;
        UiBackToHomeButton.SetActive(true);
        UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);
    }
    public void ShowCongratePopup(string questionPackName)
    {
        UiHolder.SetActive(true);
        UiCorrectImage.GetComponent<Image>().sprite = AssetsDataMart.Instance.congrateCupSprite;
        UiStatusText.text = "Xin Chuc Mung!!\nBan da hoan thanh xong goi cau hoi "+ questionPackName;
        UiOKButton.SetActive(false);
        UiBackToHomeButton.SetActive(true);
        UiShareProgressButton.SetActive(true);
        UiAskingOnLeaving.m_holder.SetActive(false);
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
            //UiCorrectImage.SetActive(false);
            UiWrongImage.SetActive(false);
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
