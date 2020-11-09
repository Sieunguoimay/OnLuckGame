using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UtilityButtons : MonoBehaviour
{
    [SerializeField] private GameObject HintAdsIcon;
    [SerializeField] private GameObject SeeAnswerAdsIcon;
    [SerializeField] private Button HintButton;
    [SerializeField] private Button SeeAnswerButton;
    
    public HintPanel hintPanel;

    private bool hasAds;

    public bool ShouldShowHintButton { set; get; } = true;

    public Action SeeAnswerButtonClicked = delegate { };

    private void Awake()
    {
        SeeAnswerButton.gameObject.SetActive(false);

        if (ShouldShowHintButton)
        {
            HintButton.gameObject.SetActive(true);
        }
    }

    public void LockAllUI(bool state)
    {
        SeeAnswerButton.interactable = state;
        HintButton.interactable = state;
    }

    public void ResetAll(bool hasAds)
    {
        this.hasAds = hasAds;

        if (hasAds)
        {
            HintAdsIcon.SetActive(true);
            SeeAnswerAdsIcon.SetActive(true);
            SeeAnswerButton.gameObject.SetActive(false);
        }
        else
        {
            HintAdsIcon.SetActive(false);
            SeeAnswerAdsIcon.SetActive(false);
            SeeAnswerButton.gameObject.SetActive(true);
        }

        if (ShouldShowHintButton)
        {
            HintButton.gameObject.SetActive(true);
        }
        else
        {
            HintButton.gameObject.SetActive(false);
            SeeAnswerButton.gameObject.SetActive(true);
        }
    }

    public void OnShareButtonClicked()
    {
        Share();
    }

    public void OnHintButtonClicked()
    {
        if (hasAds)
        {
            AdsManagement.Instance.ShowRewardedAds(success =>
            {
                if (success)
                {
                    HintAdsIcon.SetActive(false);
                    SeeAnswerButton.gameObject.SetActive(true);

                    hintPanel.ShowHint();
                }
                else
                {

                }
            });
        }
        else
        {
            hintPanel.ShowHint();
        }
    }
    public void OnSeeAnswerButtonClicked()
    {
        if (hasAds)
        {
            AdsManagement.Instance.ShowRewardedAds(success =>
            {
                if (success)
                {
                    SeeAnswerAdsIcon.SetActive(false);
               
                    SeeAnswerButtonClicked?.Invoke();
                    //hintPanel.ShowAnswer();
                }
                else
                {

                }
            });
        }
        else
        {
            SeeAnswerButtonClicked?.Invoke();
            //hintPanel.ShowAnswer();
        }
    }

    public void Share()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Collor Roll Game").SetText("Check out for the game!")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}
