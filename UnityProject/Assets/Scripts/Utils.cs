using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Utils /*: MonoBehaviour*/
{
    private static Utils s_instance = null;
    private MonoBehaviour m_rMonoBehaviour = null;
    public delegate void RequestCallback(string response);
    // Start is called before the first frame update
    public static Utils Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new Utils();
            }
            return s_instance;
        }
    }

    public void Init(MonoBehaviour monoBehaviour)
    {
        //if(m_rMonoBehaviour==null)
        m_rMonoBehaviour = monoBehaviour;
    }

    public void GetRequest(string uri, RequestCallback callback = null)
    {
        m_rMonoBehaviour.StartCoroutine(getRequest(uri,callback));
    }

    IEnumerator getRequest(string uri, RequestCallback callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
                //any call to this function reaching this point will trigger a popup
                //cover the whole screen saying: No internet!!


            }
            else
            {
                //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if(callback!=null)
                    callback(webRequest.downloadHandler.text);
            }
        }
    }
    public void LoadImageIntoImage(string url, Image image)
    {
        m_rMonoBehaviour.StartCoroutine(loadImageIntoTexture(url, image));
    }
    IEnumerator loadImageIntoTexture(string url, Image image)
    {
        if (image != null)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
                Debug.Log("loaded image " + url);
            }
        }
    }
    public void PostRequest(string url, WWWForm formData, RequestCallback callback)
    {
        m_rMonoBehaviour.StartCoroutine(postRequest(url,formData,callback));
    }
    IEnumerator postRequest(string url,WWWForm formData, RequestCallback callback)
    {
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            if (callback != null)
                callback(www.downloadHandler.text);
        }
    }

    public delegate void LoadImageCallback(Texture2D texture);
    public void LoadImage(string url, LoadImageCallback callback)
    {
        //return File.ReadAllBytes(path);
        m_rMonoBehaviour.StartCoroutine(loadImage(url, callback));
    }
    private IEnumerator loadImage(string url, LoadImageCallback callback)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            callback(www.texture);
            Debug.Log("loaded image " + url);
        }
    }

    public Texture2D ResizeTexture(Texture2D texture, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture, rt);
        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        result.Apply();
        return result;
    }
    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    public string convertToUnSign3(string s)
    {
        Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        string temp = s.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
    }
}
