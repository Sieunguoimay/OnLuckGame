using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public UserProfile UserProfile;
    [SerializeField] private Scoreboard Scoreboard;
    [SerializeField] private GameObject Guideline;
    [SerializeField] private Text TextGuideline;
    [SerializeField] private Text TextTitle;
    [SerializeField] private Animator animator;

    private int hashCode_Show;
    private int hashCode_Hide;

    void Start()
    {
        hashCode_Show = Animator.StringToHash("show");
        hashCode_Hide = Animator.StringToHash("hide");

        UserProfile.LogoutButtonClicked += Hide;

    }

    public void ShowUserProfile()
    {
        gameObject.SetActive(true);
        animator.SetTrigger(hashCode_Show);

        UserProfile.gameObject.SetActive(true);
        Scoreboard.gameObject.SetActive(false);
        Guideline.SetActive(false);

        UserProfile.Refresh();
    }

    public void ShowGuideline(string title,string content)
    {
        TextTitle.text = title;
        TextGuideline.text = content;

        gameObject.SetActive(true);
        animator.SetTrigger(hashCode_Show);

        UserProfile.gameObject.SetActive(false);
        Scoreboard.gameObject.SetActive(false);
        Guideline.SetActive(true);
    }

    public void ShowScoreboard()
    {
        gameObject.SetActive(true);
        animator.SetTrigger(hashCode_Show);

        UserProfile.gameObject.SetActive(false);
        Scoreboard.gameObject.SetActive(true);
        Guideline.SetActive(false);

        Scoreboard.Show();
    }

    public void Hide()
    {
        animator.SetTrigger(hashCode_Hide);

        float animDuration = animator.runtimeAnimatorController.animationClips[0].length;

        new DelayAction(this, () => {

            gameObject.SetActive(false);

        }, animDuration);
    }

    public void HandleClosePupupButtonClicked()
    {
        Hide();
    }



}
