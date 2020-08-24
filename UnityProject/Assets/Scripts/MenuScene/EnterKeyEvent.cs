using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterKeyEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return)) { if(eventCallback!=null) eventCallback(); }
    }
    public delegate void EventCallback();
    public EventCallback eventCallback = null;
}
