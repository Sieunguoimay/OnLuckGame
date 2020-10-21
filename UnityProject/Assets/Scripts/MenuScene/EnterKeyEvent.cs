using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterKeyEvent : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return)) { 
            eventCallback?.Invoke(); 
        }
    }
    public Action eventCallback = delegate { };
}