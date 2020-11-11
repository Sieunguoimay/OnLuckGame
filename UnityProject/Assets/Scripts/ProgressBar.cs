using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    private Slider slider;
    private float targetValue;
    private bool done;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < targetValue)
        {
            float speed = (targetValue - slider.value) * 0.05f;
            if (speed < 0.01f) speed = 0.01f;
            slider.value += speed;
        }
        else
        {
            slider.value = targetValue;
            //if (!progressBarLock)
            //{
            //    progressBarLock = true;
            //    if (hideSplashSceneLock)
            //        hideSplashSceneLock = false;
            //    else
            //        StartCoroutine(hideSplashScenePanel());
            //}
        }
        if (!done)
        {
            if (slider.value == 1.0f)
            {
                done = true;
                if(DoneCallback!=null)
                    DoneCallback();
            }
        }
    }
    public void SlideTo(float value)
    {
        targetValue = value;
        if (targetValue > 1.0f)
            targetValue = 1.0f;
    }
    public void Init()
    {
        done = false;
        slider.value = 0;
    }
    public delegate void Done();
    public Done DoneCallback = null;
}
