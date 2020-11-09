using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public Text UiText;
    public float m_time { private set; get; }
    private bool isRunning = false;

    public delegate void OnTimeoutCallback();
    public OnTimeoutCallback m_timeoutCallback;


    private float m_lastTime;
    private float m_deltaTime;


    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            if (m_time > 0)
            {
                m_deltaTime = Time.time - m_lastTime;
                m_lastTime = Time.time;

                m_time -= m_deltaTime;
                updateTextUi();
            }
            else
            {
                m_time = 0f;
                //trigger the callback onTimeout here
                isRunning = false;
                m_timeoutCallback();
            }
        }
        
    }
    public void Resume()
    {
        isRunning = true;
    }
    public void Run()
    {
        isRunning = true;
        m_lastTime = Time.time;
    }
    public CountdownTimer Reset(int time)
    {
        m_time = (float)time;
        isRunning = false;
        updateTextUi();
        return this;
    }
    public int Pause()
    {
        //I don't think you should be able to pause.
        isRunning = false;
        return (int)m_time;
    }
    private void updateTextUi()
    {
        if (UiText != null)
        {
            int min = ((int)m_time / 60);
            int sec = ((int)m_time % 60);
            UiText.text = (min > 9 ? "" : "0") + min + ":" + (sec > 9 ? "" : "0") + sec;
        }
    }
}
