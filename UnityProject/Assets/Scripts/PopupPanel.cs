using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPanel : MonoBehaviour
{
    private Animator m_animator;
    public GameObject m_holder;
    public delegate void ClosePanelCallback();
    public ClosePanelCallback m_closePanelCallback;
    // Start is called before the first frame update
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_holder.SetActive(false);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        if (!m_holder.activeSelf)
        {
            m_holder.SetActive(true);
            m_animator.SetTrigger("show");
        }
        else
        {
            Debug.Log("Bleh");
        }
    }
    public delegate void HideCallback();
    public bool Hide(HideCallback callback = null)
    {
        if (m_holder.activeSelf)
        {
            m_animator.SetTrigger("hide");
            StartCoroutine(hide(callback));
            return true;
        }
        return false;
    }
    private IEnumerator hide(HideCallback callback = null)
    {
        yield return new WaitForSeconds(m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        if(callback!=null)
            callback();
        m_holder.SetActive(false);
    }
    public void OnClosePanelButtonClicked()
    {
        m_closePanelCallback();
    }
}
