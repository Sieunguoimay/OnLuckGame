using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
public class Utils /*: MonoBehaviour*/
{
    public class Neuron
    {
        public class Input
        {
            private Neuron neuron;
            public bool state;
            public int index;
            public Input(Neuron neuron,int index)
            {
                this.neuron = neuron;
                this.index = index;
                Reset();
            }
            public void Signal()
            {
                state = true;
                neuron.Signal(this);
            }
            public void Reset()
            {
                state = false;
            }
        }
        public List<Input> inputs;
        public Neuron(int n)
        {
            inputs = new List<Input>();
            for(int i = 0; i<n; i++)
            {
                CreateNewInput();
            }
        }
        public void Signal(Input input)
        {
            foreach(Input i in inputs)
                if (!i.state)
                    return;
            if (output != null)
                output();
        }
        public Input CreateNewInput()
        {
            Input input = new Input(this,inputs.Count);
            inputs.Add(input);
            return input;
        }
        public delegate void Output();
        public Output output = null;
    }


    private static Utils s_instance = null;
    private MonoBehaviour m_rMonoBehaviour = null;
    private string root = "/";
    public delegate void RequestCallback(string response);
    public delegate void NoInternetCallback();
    public NoInternetCallback networkErrorCallback = null;
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
    private Utils()
    {
        root = Application.persistentDataPath;
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
                if(networkErrorCallback!=null)
                    networkErrorCallback();

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
    public void PostRequest(string url, string str, RequestCallback callback)
    {
        Debug.Log("PostRequest" + url);
        m_rMonoBehaviour.StartCoroutine(postRequest(url, str, callback));
    }
    IEnumerator postRequest(string url, string str, RequestCallback callback)
    {
        Debug.Log("postRequest" + url);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(str);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("postRequest2" + url);
        yield return request.SendWebRequest();
        Debug.Log("postRequest3" + url);

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            if (networkErrorCallback != null)
                networkErrorCallback();
        }
        else
        {
            Debug.Log("Form upload complete!");
            if (callback != null)
                callback(request.downloadHandler.text);
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

        UnityWebRequest request = UnityWebRequest.Post(url, formData);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            if (networkErrorCallback != null)
                networkErrorCallback();
        }
        else
        {
            Debug.Log("Form upload complete!");
            if (callback != null)
                callback(request.downloadHandler.text);
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
    //public void SaveJsonToFile(string json, string fileName)
    //{
    //    File.WriteAllText(fileName, json);
    //}
    [Serializable]
    public class JsonWrapper<T>
    {
        public T data;
    }
    public void SaveObjectToFile<T>(T data,string fileName)
    {
        //BinaryFormatter bf = new BinaryFormatter();
        ////FileStream file = File.Create(Application.persistentDataPath +"/"+ fileName);
        //Stream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Create);
        //bf.Serialize(file, data);
        //file.Close();
        JsonWrapper<T> jsonWrappingClass = new JsonWrapper<T>() { data = data };
        
        string jsonData = JsonUtility.ToJson(jsonWrappingClass);
        Debug.Log("Saved json: " + jsonData);
        File.WriteAllText(root + "/" + fileName, jsonData);


        //serialize
        //using (Stream stream = File.Open(serializationFile, FileMode.Create))
        //{
        //    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        //    bformatter.Serialize(stream, salesmanList);
        //}

    }
    public T LoadFileToObject<T>(string fileName)
    {
        if (File.Exists(root + "/"+fileName))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
            //obj= bf.Deserialize(file);
            //file.Close();
            string jsonData = File.ReadAllText(root + "/" + fileName);
            Debug.Log("Loaded json: " + jsonData);
            return JsonUtility.FromJson<JsonWrapper<T>>(jsonData).data;
        }
        return default(T);
    }
    public void SaveBytesToFile(byte[]bytes,string fileName)
    {
        Debug.Log("Saved file to: " + root+ "/" + fileName);
        File.WriteAllBytes(root +"/"+fileName, bytes);
    }
    public void DeleteFile(string fileName)
    {
        Debug.Log("Deleted file: " + root+ "/" + fileName);
        File.Delete(root + "/" + fileName);
    }
    public Texture2D LoadFileToTexture(string fileName)
    {
        byte[] textureBytes = Utils.Instance.LoadFileToBytes(fileName);
        if (textureBytes != null)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(textureBytes);
            return texture;
        }
        return null;
    }
    public byte[] LoadFileToBytes(string fileName)
    {
        byte[] obj = null;
        if (File.Exists(root + "/" + fileName))
        {
            obj = File.ReadAllBytes(root + "/" + fileName);
        }
        else
        {
            Debug.Log("Not Existed" + root + "/" + fileName);
        }
        return obj;
    }
    public void SetAndStretchToParentSize(RectTransform _mRect, RectTransform _parent)
    {
        _mRect.anchoredPosition = _parent.position;
        _mRect.anchorMin = new Vector2(1, 0);
        _mRect.anchorMax = new Vector2(0, 1);
        _mRect.pivot = new Vector2(0.5f, 0.5f);
        _mRect.sizeDelta = _parent.rect.size;
        _mRect.transform.SetParent(_parent);
    }
    public T FromJsonList<T>(string json)
    {
        return JsonUtility.FromJson<JsonWrapper<T>>("{\"data\":" + json + "}").data;
    }
    public string GetCurrentDateTime()
    {
        DateTime utcDate = DateTime.UtcNow;
        return utcDate.ToString(new CultureInfo("en-GB"));
    }

    //public delegate void ShowListCallback(GameObject item, int index);
    //public void CreateList<T>(int number,GameObject ItemTemplate, ShowListCallback callback)
    //{

    //    for (int i = 0; i < number; i++)
    //    {
    //        GameObject newItem = MonoBehaviour.Instantiate(ItemTemplate) as GameObject;
    //        newItem.SetActive(true);
    //        newItem.transform.SetParent(ItemTemplate.transform.parent, false);

    //        callback(newItem, i);
    //    }
    //}



}
