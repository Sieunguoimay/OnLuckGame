using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toast
{
    private static Toast s_instance = null;
    public static Toast Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new Toast();
            return s_instance;
        }
    }
    private Toast() { }

    

    public delegate void ToastCallback();
    public void Show(GameObject parent,string text, float time, ToastCallback callback)
    {
        Utils.Instance.context.StartCoroutine(show(parent,text, time,callback));
    }
    private IEnumerator show(GameObject parent,string text, float time, ToastCallback callback)
    {
        GameObject toastPanel = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UiToastPanel",typeof(GameObject)),parent.transform) as GameObject;
        toastPanel.GetComponentInChildren<Text>().text = text;
        toastPanel.transform.parent = parent.transform;

        yield return new WaitForSeconds(time);
        MonoBehaviour.Destroy(toastPanel);
        callback();
    }
}
