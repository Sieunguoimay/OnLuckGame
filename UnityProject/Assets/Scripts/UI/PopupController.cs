using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject UserProfile;
    [SerializeField] private GameObject Scoreboard;
    [SerializeField] private GameObject Guideline;
    [SerializeField] private Text TextGuideline;
    [SerializeField] private ScrollList ScoreboardScrollList;
    [SerializeField] private Animator animator;

    private int hashCode_Show;
    private int hashCode_Hide;

    void Start()
    {
        hashCode_Show = Animator.StringToHash("show");
        hashCode_Hide = Animator.StringToHash("hide");
    }

    public void ShowGuideline(string content)
    {
        TextGuideline.text = content;

        gameObject.SetActive(true);
        UserProfile.SetActive(false);
        Scoreboard.SetActive(false);
        Guideline.SetActive(true);
    }

    public void ShowScoreboard()
    {
        gameObject.SetActive(true);
        UserProfile.SetActive(false);
        Scoreboard.SetActive(true);
        Guideline.SetActive(false);
    }


    public void AddScoreboardItem(int index, string name, string profilePic, int score)
    {
        var item = ScoreboardScrollList.CreateItem<ScoreboardItem>();

        item.SetRank(index);
        item.SetUserName(name);
        item.SetScore(score);
        item.SetProfilePicture(profilePic);
    }

    public void HandleClosePupupButtonClicked()
    {
        animator.SetTrigger(hashCode_Hide);

        float animDuration = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        new DelayAction(this, () => { 

            gameObject.SetActive(false);

        }, animDuration);
    }
}
