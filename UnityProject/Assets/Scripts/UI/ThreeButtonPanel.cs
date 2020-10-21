using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;

public class ThreeButtonPanel : MonoBehaviour
{
    [SerializeField] Text introButtonText;
    [SerializeField] Text scoreboardButtonText;
    [SerializeField] Text guidelineButtonText;

    // Start is called before the first frame update
    void Start()
    {
        introButtonText.text = AssetsDataMart.Instance.constantsSO.stringsSO.intro;
        scoreboardButtonText.text = AssetsDataMart.Instance.constantsSO.stringsSO.scoreboard;
        guidelineButtonText.text = AssetsDataMart.Instance.constantsSO.stringsSO.guideline;
    }

    //public void OnRatingButtonClicked()
    //{
    //    Debug.Log("OnRatingButtonClicked");
    //    Application.OpenURL(HttpClient.Instance.BaseUrl);
    //}
    //public void OnGuidelineButtonClicked()
    //{
    //    Debug.Log("OnGuidelineButtonClicked");
    //    ShowGuidelinePanel(AssetsDataMart.Instance.constantsSO.stringsSO.guideline, QuestionDataMart.Instance.onluckLocalMetadata.guideline_content);
    //    //ShowPopupPanel(5);
    //}
    //public void OnScoreboardButtonClicked()
    //{
    //    Debug.Log("OnScoreboardButtonClicked");
    //    ShowPopupPanel(6);
    //    if (m_menuPresenter.ShowScoreboard())
    //    {
    //        spinnerIndex = 6;
    //        SetSpinnerTo(getPanelById(6));
    //    }
    //}
}
