using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpClient 
{
    [System.Serializable]
    public class LogInResponseData
    {
        public int user_id;
        public string user_name;
        public string active_vendor;
        public string profile_picture;
        public int score;
    }
    [System.Serializable]
    public class SignInResponseData
    {
        public int user_id;
        public int score;
        public string active_vendor;
        public bool has_profile_picture;
        public string profile_picture;
    }
    [System.Serializable]
    public class UploadPhotoResponseData
    {
        public string profile_picture;
    }
    [System.Serializable]
    public class UserScoreResponseData
    {
        public string id;
        public int score;
        public string profile_picture;
        public string name;
    }

    [System.Serializable]
    public class Response<T>
    {
        public string status;
        public T data;
    }
    private static HttpClient s_instance = null;
    public delegate void HttpClientBoolCallback(bool result);
    public delegate void HttpClientResponseCallback<T>(Response<T> response);

#if UNITY_EDITOR
    public string BaseUrl = "localhost:8000";
    public string BaseApiUrl = "localhost:8000/api/onluck";
#else 
    public string BaseUrl = "http://926f60481683.ngrok.io";
    public string BaseApiUrl = "http://926f60481683.ngrok.io/api/onluck";
#endif

    private HttpClient()
    {

    }

    public static HttpClient Instance
    {
        get
        {
            if(s_instance == null)
            {
                s_instance = new HttpClient();
            }
            return s_instance;
        }
    }
    public void Init()
    {
        Debug.Log("Initialized Http Client");
    }

    public void LogIn(string email, string password, HttpClientResponseCallback<LogInResponseData> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl+"/login?email="+email+"&password="+password, 
            (response) => { Debug.Log(response); callback(JsonUtility.FromJson<Response<LogInResponseData>>(response)); });
    }
    public void SignUp(string userName, string email, string password, HttpClientResponseCallback<LogInResponseData> callback)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", email);
        formData.AddField("password", password);
        formData.AddField("name", userName);
        Utils.Instance.PostRequest(BaseApiUrl + "/signup",formData,
            (response) => callback(JsonUtility.FromJson<Response<LogInResponseData>>(response)));
    }
    public void SignIn(string userName, string email, byte[] pixels,
        HttpClientResponseCallback<SignInResponseData> callbackForScore,
        HttpClientResponseCallback<UploadPhotoResponseData> callbackForProfilePicture)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", email);
        formData.AddField("name", userName);
        formData.AddField("vendor", "facebook");
        Utils.Instance.PostRequest(BaseApiUrl + "/signin", formData,(response) => {
            Response<SignInResponseData> r = JsonUtility.FromJson<Response<SignInResponseData>>(response);
            callbackForScore(r);
            if(!r.data.has_profile_picture)
            {
                WWWForm formProfilePicture = new WWWForm();
                formProfilePicture.AddField("id", r.data.user_id);
                formProfilePicture.AddBinaryData("profile_picture", pixels, "profile_picture.png", "image/png");
                Utils.Instance.PostRequest(BaseApiUrl + "/uploadphoto", formProfilePicture, (response2) =>
                {
                    Debug.Log("uploadphoto:facebook:" + response2);
                    callbackForProfilePicture(JsonUtility.FromJson<Response<UploadPhotoResponseData>>(response2));
                });
            }
        });
    }
    public void UploadPhoto(int id, byte[] pixels, HttpClientResponseCallback<UploadPhotoResponseData> callback)
    {
        WWWForm formProfilePicture = new WWWForm();
        formProfilePicture.AddField("id", id);
        formProfilePicture.AddBinaryData("profile_picture", pixels, "profile_picture.png", "image/png");
        Utils.Instance.PostRequest(BaseApiUrl + "/uploadphoto", formProfilePicture, (response) =>
        {
            Debug.Log("uploadphoto:" + response);
            callback(JsonUtility.FromJson<Response<UploadPhotoResponseData>>(response));
        });
    }

    public void Rename(int id, string newName)
    {
        WWWForm renameForm = new WWWForm();
        renameForm.AddField("id", id);
        renameForm.AddField("new_name", newName);
        Utils.Instance.PostRequest(BaseApiUrl + "/rename", renameForm, (response) => {
            Debug.Log("rename:"+response);
        });

    }

    public void LoadScoreboard(HttpClientResponseCallback<List<UserScoreResponseData>> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/getscoreboard", (response) =>
        {
            Debug.Log(response);
            callback(JsonUtility.FromJson<Response<List<UserScoreResponseData>>>(response));
        });
    }

}
