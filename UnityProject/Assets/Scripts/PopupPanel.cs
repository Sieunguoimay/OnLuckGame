using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour
{
    public Text statusText;
    public Text buttonText;

    public GameObject m_holder;

    private Animator m_animator;

    public Action<bool> OnClose = delegate { };

    private bool flag;

    private void Awake()
    {
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();

            m_animator.SetTrigger("show");
        }
    }

    public void Show(string statusText, string buttonText, Action<bool> onClose, bool resultFlag = true)
    {
        if (!m_holder.activeSelf)
        {
            this.statusText.text = statusText;
            this.buttonText.text = buttonText;

            OnClose = onClose;
            this.flag = resultFlag;

            m_holder.SetActive(true);

            if (m_animator != null)
            {
                m_animator.SetTrigger("show");
            }
            Debug.Log("PopupPanel:Shown");
        }
        else
        {
            Debug.Log("PopupPanel:Bleh");
        }
    }
    public void HideImmediate()
    {
        m_holder.SetActive(false);
    }
    public void OnClosePanelButtonClicked()
    {
        Hide(false);
    }
    public void OnOKButtonClicked()
    {
        Hide(flag);
    }
    public bool Hide(bool value)
    {
        if (m_holder.activeSelf)
        {
            m_animator.SetTrigger("hide");

            StartCoroutine(hide(value));

            return true;
        }
        return false;
    }
    private IEnumerator hide(bool value)
    {
        yield return new WaitForSeconds(m_animator.runtimeAnimatorController.animationClips[0].length);

        m_holder.SetActive(false);

        OnClose?.Invoke(value);
    }

}
