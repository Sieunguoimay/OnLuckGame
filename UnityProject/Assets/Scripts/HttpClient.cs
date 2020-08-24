using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataMarts;
using Assets.Scripts;
public class HttpClient 
{
    [System.Serializable]
    public class LogInResponseData
    {
        public int user_id;
        public int uptodate_token;
        public string user_name;
        public string email;
        public string active_vendor;
        public string profile_picture;
    }
    [System.Serializable]
    public class TwoStrings
    {
        public string name;
        public string profile_picture;
    }

    //[System.Serializable]
    //public class SignInResponseData
    //{
    //    public int user_id;
    //    //public string active_vendor;
    //    //public string profile_picture;
    //}
    //[System.Serializable]
    //public class UploadPhotoResponseData
    //{
    //    public string profile_picture;
    //}
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
    public delegate void HttpClientResponseDataCallback<T>(T data);

#if UNITY_EDITOR
    public string BaseUrl = "http://onluck-the-game.herokuapp.com";
    public string BaseApiUrl = "http://onluck-the-game.herokuapp.com/api/onluck";
#else 
    public string BaseUrl = "http://onluck-the-game.herokuapp.com";
    public string BaseApiUrl = "http://onluck-the-game.herokuapp.com/api/onluck";
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

    public void LogIn(string email, string password, HttpClientResponseCallback<UserDataAndPlayingDataUptodateToken> callback,
        HttpClientResponseCallback<TwoStrings> callback2)
    {
        Utils.Instance.GetRequest(BaseApiUrl+"/login?email="+email+"&password="+password, 
            (response) => { 
                Debug.Log("LogIn.GetRequest.response: "+response); 
                
                //callback(JsonUtility.FromJson<Response<UserDataMart.UserData>>(response));
                 Response<UserDataAndPlayingDataUptodateToken> res = JsonUtility.FromJson<Response<UserDataAndPlayingDataUptodateToken>>(response);
                if (res.status.Equals("not_signed_up_but_already_signed_in"))
                {
                    Response<TwoStrings> res2 = JsonUtility.FromJson<Response<TwoStrings>>(response);
                    callback2(res2);
                }
                else
                {
                    callback(res);
                }

            });
    }
    [Serializable]
    public class UserDataAndPlayingDataUptodateToken
    {
        public UserDataMart.UserData user_data;
        public int playing_data_uptodate_token;
    }
    public void SignUp(string userName, string email, string password, string profilepicture,
        HttpClientResponseCallback<UserDataAndPlayingDataUptodateToken> callback)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", email);
        formData.AddField("password", password);
        formData.AddField("name", userName);
        if(!profilepicture.Equals(""))
            formData.AddField("profile_picture", profilepicture);
        Utils.Instance.PostRequest(BaseApiUrl + "/signup", formData, (response) =>
             {
                 Debug.Log(response);
                 Response<UserDataAndPlayingDataUptodateToken> res = JsonUtility.FromJson<Response<UserDataAndPlayingDataUptodateToken>>(response);
                 callback(res);
             });
    }
    [Serializable]
    public class SignInData
    {
        public int user_id;
        public int playing_data_uptodate_token;
        public int uptodate_token;
        public string profile_picture;
    }
    public void SignIn(string userName, string email, byte[] pixels,
        HttpClientResponseCallback<SignInData> callback1,
        HttpClientResponseCallback<UploadPhotoResponse> callback2 = null
        )
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", email);
        formData.AddField("name", userName);
        formData.AddField("vendor", "facebook");
        formData.AddBinaryData("profile_picture", pixels, "profile_picture.png", "image/png");
        Utils.Instance.PostRequest(BaseApiUrl + "/signin", formData,(response) => {
            Response<SignInData> r = JsonUtility.FromJson<Response<SignInData>>(response);
            callback1(r);

        });
    }
    [System.Serializable]
    public class UploadPhotoResponse
    {
        public int uptodate_token;
        public string profile_picture;
    }
    public void UploadPhoto(int id, byte[] pixels, HttpClientResponseCallback<UploadPhotoResponse> callback,bool dontGenerateUptodateToken = false)
    {
        WWWForm formProfilePicture = new WWWForm();
        formProfilePicture.AddField("id", id);
        formProfilePicture.AddBinaryData("profile_picture", pixels, "profile_picture.png", "image/png");
        if(dontGenerateUptodateToken)
            formProfilePicture.AddField("dont_generate_uptodate_token", 1);
        Utils.Instance.PostRequest(BaseApiUrl + "/uploadphoto", formProfilePicture, (response) =>
        {
            Debug.Log("uploadphoto:" + response);
            Response<UploadPhotoResponse> res = JsonUtility.FromJson<Response<UploadPhotoResponse>>(response);
            callback(res);
        });
    }
    [System.Serializable]
    public class UptodateTokenResponse
    {
        public int uptodate_token;
    }
    public void Rename(int id, string newName, HttpClientResponseCallback<UptodateTokenResponse> callback)
    {
        WWWForm renameForm = new WWWForm();
        renameForm.AddField("id", id);
        renameForm.AddField("new_name", newName);
        Utils.Instance.PostRequest(BaseApiUrl + "/rename", renameForm, (response) => {
            Debug.Log("rename:"+response);
            Response<UptodateTokenResponse>res =  JsonUtility.FromJson<Response<UptodateTokenResponse>>(response);
            callback(res);
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

    public delegate void LoadDataFromServerCallback(UserDataMart.UserData newUserData);
    public void LoadUserDataFromServer(int id,LoadDataFromServerCallback callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl
            + "/getuser?id=" + id,
            (response) => {
                Response<UserDataMart.UserData> res = JsonUtility.FromJson<Response<UserDataMart.UserData>>(response);
                if (res.status.Equals("OK"))
                {
                    callback(res.data);
                }
                else
                    callback(null);
            });
    }

    public void GetOnluckMetadata(HttpClientResponseCallback<OnluckMetadata> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl+ "/getonluckmetadata",
        (response) => {
            Debug.Log(response);
            Response<OnluckMetadata> res = JsonUtility.FromJson<Response<OnluckMetadata>>(response);
            callback(res);
        });
    }

    [Serializable]
    public class OnluckMetadata
    {
        public int active_season;
        public int activation_code;
        public int season_uptodate_token;
        public string quote;
        public int uptodate_token;
        public string intro_content;
        public string guideline_content;
    }
    [Serializable]
    public class GameStartData
    {
        public OnluckMetadata metadata;
        public UserDataMart.UserData user_data;
        public int playing_data_uptodate_token;
    }
    public void NotifyServerOnGameStart(UserDataMart.UserData userData, HttpClientResponseCallback<GameStartData> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/notifyserverongamestart?user_id=" + userData.id 
            +"&vendor_name=" + userData.active_vendor,
            //+ "&playing_data_uptodate_token="+ playingData.uptodata_token,
        (response) => {
            Debug.Log(response);
            Response<GameStartData> res = JsonUtility.FromJson<Response<GameStartData>>(response);
            callback(res);
        });
    }
    public void CheckForUptodateUserDataFromServer(UserDataMart.UserData userData, HttpClientResponseCallback<UserDataMart.UserData> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl
            + "/checkuptodateuserdata?id=" + userData.id + "&uptodate_token=" + userData.uptodate_token,
            (response) => {
                Response<UserDataMart.UserData> res = JsonUtility.FromJson<Response<UserDataMart.UserData>>(response);
                callback(res);
            });
    }
    [System.Serializable]
    public class McqQuestion
    {
        public int id;
        public int pack_id;
        public string question;
        public string choices;
        public int answer;
        public int time;
        public int score;
        public string images;
        public string hints;
    }
    [System.Serializable]
    public class TypedQuestion
    {
        public int id;
        public int pack_id;
        public string question;
        public string answer;
        public int score;
        public string images;
        public string hints;
    }
    [System.Serializable]
    public class Pack
    {
        public int id;
        public int season_id;
        public string title;
        public string sub_text;
        public string icon;
        public string question_type;
        public List<TypedQuestion> typed_questions;
        public List<McqQuestion> mcq_questions;
    }
    [System.Serializable]
    public class Season
    {
        public int id;
        public string name;
        public string from;
        public string to;
        public List<Pack> packs;
    }
    private int imagesToDownloadCount = 0;
    private void signalImageDownloaded()
    {
        Debug.Log("signalImageDownloaded " + imagesToDownloadCount);
        if (imagesToDownloadCount > 1)
            imagesToDownloadCount--;
        else 
        {
            if (downloadedCallback != null)
            {
                Debug.Log("FIRE " + imagesToDownloadCount);
                downloadedCallback();
                downloadedCallback = null;
            }
        }
    }
    public delegate void DownloadedCallback();
    private DownloadedCallback downloadedCallback = null;
    public void DownloadGameData(HttpClientResponseDataCallback<QuestionDataMart.Season> callback, DownloadedCallback callback2)
    {
        downloadedCallback = callback2;
        Utils.Instance.GetRequest(BaseApiUrl + "/downloadgamedata", (response) =>
        {
            Debug.Log(response);
            imagesToDownloadCount = 0;
            Response<Season> res = JsonUtility.FromJson<Response<Season>>(response);
            if (res.status.Equals("OK"))
            {
                QuestionDataMart.Season season = new QuestionDataMart.Season();
                HttpClient.Season ss = res.data;
                season.id = ss.id;
                season.name = ss.name;
                season.from = ss.from;
                season.to = ss.to;
                season.packs = new List<QuestionDataMart.Pack>();
                foreach (HttpClient.Pack p in ss.packs)
                {
                    QuestionDataMart.Pack pack = new QuestionDataMart.Pack();
                    pack.id = p.id;
                    pack.title = p.title;
                    pack.sub_text = p.sub_text;
                    pack.icon = new QuestionDataMart.Image() { path = p.icon };
                    imagesToDownloadCount += 1;
                    Utils.Instance.LoadImage(BaseUrl + pack.icon.path,(texture)=> { 
                        Debug.Log("Downloaded "+ pack.icon.path); 
                        pack.icon.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,0)); 
                        signalImageDownloaded(); });
                    pack.question_type = Int32.Parse(p.question_type);

                    if (pack.question_type == 0)
                    {
                        pack.typed_questions = new List<QuestionDataMart.TypedQuestion>();
                        foreach (HttpClient.TypedQuestion q in p.typed_questions)
                        {
                            QuestionDataMart.TypedQuestion question = new QuestionDataMart.TypedQuestion();
                            question.id = q.id;
                            question.question = q.question;
                            question.answer = q.answer;
                            question.score = q.score;
                            question.images = new List<QuestionDataMart.Image>();
                            List<string> images = Utils.Instance.FromJsonList<List<string>>(q.images);
                            imagesToDownloadCount += images.Count;
                            foreach (string img in images)
                            {
                                QuestionDataMart.Image image = new QuestionDataMart.Image() { path = img };
                                question.images.Add(image);
                                Utils.Instance.LoadImage(BaseUrl + img,(texture)=> { Debug.Log("Downloaded "+img);
                                    image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,0));
                                    /*image.texture = texture;*/ signalImageDownloaded(); });
                            }
                            question.hints = new List<string>();
                            List<string> hints = Utils.Instance.FromJsonList<List<string>>(q.hints);
                            foreach (string hint in hints)
                                question.hints.Add(hint);
                            pack.typed_questions.Add(question);
                        }
                    }
                    else if (pack.question_type == 1)
                    {
                        pack.mcq_questions = new List<QuestionDataMart.MCQQuestion>();
                        foreach (HttpClient.McqQuestion q in p.mcq_questions)
                        {
                            QuestionDataMart.MCQQuestion question = new QuestionDataMart.MCQQuestion();
                            question.id = q.id;
                            question.question = q.question;
                            question.choices = Utils.Instance.FromJsonList<List<string>>(q.choices).ToArray();
                            question.answer = q.answer;
                            question.time = q.time;
                            question.score = q.score;
                            question.images = new List<QuestionDataMart.Image>();
                            List<string> images = Utils.Instance.FromJsonList<List<string>>(q.images);
                            imagesToDownloadCount += images.Count;
                            foreach (string img in images)
                            {
                                QuestionDataMart.Image image = new QuestionDataMart.Image() { path = img };
                                question.images.Add(image);
                                Utils.Instance.LoadImage(BaseUrl + img, (texture) => { Debug.Log("Downloaded " + img); 
                                    image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,0));
                                    signalImageDownloaded(); });
                            }
                            question.hints = new List<string>();
                            question.hints = Utils.Instance.FromJsonList<List<string>>(q.hints);
                            pack.mcq_questions.Add(question);
                        }
                    }

                    season.packs.Add(pack);
                }

                callback(season);
            }
            if (imagesToDownloadCount == 0)
                signalImageDownloaded();

        });
    }

    public void GetPlayingDataFromServer(HttpClientResponseCallback<PlayingDataMart.PlayingData> callback,int userId)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/getplayingdata?user_id="+userId, (response) =>
        {
            Debug.Log(response);
            Response<PlayingDataMart.PlayingData> res = JsonUtility.FromJson<Response<PlayingDataMart.PlayingData>>(response);
            callback(res);
        });
    }
    public void SendQuestionPlayingDataToServer(GameLogic.PlayingData playingData,HttpClientResponseCallback<int> callback)
    {
        string json = JsonUtility.ToJson(playingData);
        Debug.Log("SendQuestionPlayingDataToServer: " + json);
        Utils.Instance.PostRequest(BaseApiUrl + "/storeplayingdata", json, (response) => {
            Debug.Log("SendQuestionPlayingDataToServer:" + response);
            Response<int> res = JsonUtility.FromJson<Response<int>>(response);
            callback(res);
        });
    }
}
