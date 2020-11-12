using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataMarts;
using System;

public class SplashUI : MonoBehaviour
{
    [SerializeField] private PopupPanel simplePopup;
    [SerializeField] private Spinner UiSpinner;

    public GameObject UiSplashScenePanel;
    public GameObject UiSplashSceneSpinnerTargetPanel;
    public GameObject UiSplashSceneLogoPanel;
    public ProgressBar UiProgressBar;
    private bool hideSplashSceneLock;

    private Utils.Neuron hideSplashSceneNeuron= new Utils.Neuron(2);

    void Awake()
    {

        DontDestroyOnLoad(this);

        hideSplashSceneLock = true;

        hideSplashSceneNeuron.output = HideSplashScene;

        QuestionDataMart.Instance.publishProgress += SetProgressBar;

        QuestionDataMart.Instance.m_gameDataCompletedCallback += hideSplashSceneNeuron.inputs[0].Signal;

        UiProgressBar.DoneCallback = hideSplashSceneNeuron.inputs[1].Signal;

        QuestionDataMart.Instance.m_askForPermissionCallback += () =>
            ShowPopupToAskForPermission(QuestionDataMart.Instance.OnPermissionGranted);

        Utils.Instance.networkErrorCallback += () =>ShowPopupCheckYourNetwork(Main.Instance.Init);

        UiSpinner.Show();
    }

    //public void ShowAutoHideSplashScene()
    //{
    //    UiSplashScenePanel.SetActive(true);
    //    StartCoroutine(hideSplashSceneOnAnimationEnd());
    //}

    //private IEnumerator hideSplashSceneOnAnimationEnd()
    //{
    //    Debug.Log("Animating Splash Scene Logo");
    //    yield return new WaitForSeconds(UiSplashSceneLogoPanel.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
    //    if (hideSplashSceneLock)
    //    {
    //        hideSplashSceneLock = false;
    //    }
    //    else
    //    {
    //        StartCoroutine(hideSplashScenePanel());
    //    }
    //}

    private IEnumerator hideSplashScenePanel()
    {

        Main.Instance.ActivateAsyncScene();

        var splashAnimator = UiSplashScenePanel.GetComponent<Animator>();
        if (splashAnimator != null)
        {
            splashAnimator.SetBool("hide", true);
            yield return new WaitForSeconds(splashAnimator.runtimeAnimatorController.animationClips[0].length);
        }


        UiSplashScenePanel.SetActive(false);
        UiSpinner.Hide();
        Debug.Log("Splash scene is gone");

        Destroy(gameObject);
    }

    public void HideSplashScene()
    {
        StartCoroutine(hideSplashScenePanel());
    }
    public void SetProgressBar(float value)
    {
        UiProgressBar.SlideTo(value);
    }

    public void ShowPopupToAskForPermission(Action callback)
    {
        //"Da co du lieu moi cho game. Vui long tai xuong!";
        simplePopup.Show(
            AssetsDataMart.Instance.constantsSO.stringsSO.new_game_data_available,
            AssetsDataMart.Instance.constantsSO.stringsSO.download,
            result =>
            {
                if (result)
                {
                    callback?.Invoke();
                }
            });
    }

    public void ShowPopupCheckYourNetwork(Action callback)
    {
        //"Khong co internet!";
        simplePopup.Show(
            AssetsDataMart.Instance.constantsSO.stringsSO.no_internet,
            AssetsDataMart.Instance.constantsSO.stringsSO.try_again,
            _ => callback?.Invoke());
    }

}
