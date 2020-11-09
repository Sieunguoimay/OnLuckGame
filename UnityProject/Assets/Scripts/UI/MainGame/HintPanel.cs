using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintPanel : MonoBehaviour
{
    [SerializeField] private Text content;
    private Animator animator;

    private string hint;
    private string answer;

    void Start()
    {
        gameObject.SetActive(false);
        animator = GetComponent<Animator>();
    }

    public void SetHintAndAnswer(List<string> hints, string answer)
    {
        if (hints.Count > 0)
        {
            this.hint = hints[0];
        }
        this.answer = answer;
    }

    public void ShowHint()
    {
        Show();
        content.text = "Gợi ý:\n"+hint;
    }
    public void ShowAnswer()
    {
        Show();
        content.text = "Đáp án:\n" + answer;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("show");
    }
    public void OnCloseButtonClicked()
    {
        animator.SetTrigger("hide");
        new DelayAction(this, () => gameObject.SetActive(false), animator.runtimeAnimatorController.animationClips[0].length);
    }
}
