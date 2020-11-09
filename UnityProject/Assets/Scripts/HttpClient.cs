using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataMarts;
using Assets.Scripts;

public class HttpClient : MonoBehaviourSingleton<HttpClient>
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
    [Serializable]
    public class Paginate<T>
    {
        public List<T> data;

        public string prev_page_url;
        public string next_page_url;

        public int current_page;
        public int from;
        public int to;
        public int per_page;
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

#if UNITY_EDITOR
    [NonSerialized] public string BaseUrl = "localhost:8000";
    [NonSerialized] public string BaseApiUrl = "localhost:8000/api/onluck";
#else 
    [NonSerialized] public string BaseUrl = "http://onluck-the-game.herokuapp.com";
    [NonSerialized] public string BaseApiUrl = "http://onluck-the-game.herokuapp.com/api/onluck";
#endif

    private void Awake()
    {
        if (!IsAwakened)
        {
            Debug.Log("Initialized Http Client");

            BaseUrl = AssetsDataMart.Instance.constantsSO.base_url;
            BaseApiUrl = AssetsDataMart.Instance.constantsSO.base_api_url;
        }
    }



    public void LogIn(string email, string password,
        Action<Response<UserDataAndPlayingDataUptodateToken>> callback,
        Action<Response<TwoStrings>> callback2)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/login?email=" + email + "&password=" + password,
            (response) =>
            {

                Debug.Log("LogIn.GetRequest.response: " + response);

                var res = JsonUtility.FromJson<Response<UserDataAndPlayingDataUptodateToken>>(response);
                if (res.status.Equals("not_signed_up_but_already_signed_in"))
                {
                    Response<TwoStrings> res2 = JsonUtility.FromJson<Response<TwoStrings>>(response);

                    callback2?.Invoke(res2);
                }
                else
                {
                    callback?.Invoke(res);
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
        Action<Response<UserDataAndPlayingDataUptodateToken>> callback)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", email);
        formData.AddField("password", password);
        formData.AddField("name", userName);

        if (!profilepicture.Equals(""))
        {
            formData.AddField("profile_picture", profilepicture);
        }

        Utils.Instance.PostRequest(BaseApiUrl + "/signup", formData, (response) =>
             {
                 Response<UserDataAndPlayingDataUptodateToken> res = JsonUtility.FromJson<Response<UserDataAndPlayingDataUptodateToken>>(response);

                 callback?.Invoke(res);
             });
    }
    [Serializable]
    public class SignInData
    {
        public int user_id;
        public int playing_data_uptodate_token;
        public int uptodate_token;
        public string name;
        public string phone;
        public string address;
    }
    public void SignIn(string userName, string email, string fbid, /*byte[] pixels,*/
        Action<Response<SignInData>> callback1,
        Action<Response<UploadPhotoResponse>> callback2 = null
        )
    {
        WWWForm formData = new WWWForm();

        formData.AddField("email", email);
        formData.AddField("name", userName);
        formData.AddField("fbid", fbid);
        formData.AddField("vendor", "facebook");
        //formData.AddBinaryData("profile_picture", pixels, "profile_picture.png", "image/png");

        Utils.Instance.PostRequest(BaseApiUrl + "/signin", formData, (response) =>
        {
            var r = JsonUtility.FromJson<Response<SignInData>>(response);

            callback1?.Invoke(r);

        });
    }

    [System.Serializable]
    public class UploadPhotoResponse
    {
        public int uptodate_token;
        public string profile_picture;
    }

    public void UploadPhoto(int id, byte[] pixels, Action<Response<UploadPhotoResponse>> callback, bool dontGenerateUptodateToken = false)
    {
        WWWForm formProfilePicture = new WWWForm();

        formProfilePicture.AddField("id", id);
        formProfilePicture.AddBinaryData("profile_picture", pixels, "profile_picture.png", "image/png");

        if (dontGenerateUptodateToken)
        {
            formProfilePicture.AddField("dont_generate_uptodate_token", 1);
        }

        Utils.Instance.PostRequest(BaseApiUrl + "/uploadphoto", formProfilePicture, (response) =>
        {
            Response<UploadPhotoResponse> res = JsonUtility.FromJson<Response<UploadPhotoResponse>>(response);

            callback?.Invoke(res);
        });
    }

    [System.Serializable]
    public class UptodateTokenResponse
    {
        public int uptodate_token;
    }

    public void Rename(int id, string newName, Action<Response<UptodateTokenResponse>> callback)
    {
        WWWForm renameForm = new WWWForm();

        renameForm.AddField("id", id);
        renameForm.AddField("new_name", newName);

        Utils.Instance.PostRequest(BaseApiUrl + "/rename", renameForm, (response) =>
        {

            Response<UptodateTokenResponse> res = JsonUtility.FromJson<Response<UptodateTokenResponse>>(response);

            callback?.Invoke(res);
        });
    }

    public void UpdateProfile(UserDataMart.UserData userData, Action<bool, int> result)
    {

        Utils.Instance.PostRequest(BaseApiUrl + "/updateuser", JsonUtility.ToJson(userData), (response) =>
        {

            var res = JsonUtility.FromJson<Response<UptodateTokenResponse>>(response);

            if (res.status.Equals("OK"))
            {
                result?.Invoke(true, res.data.uptodate_token);
            }
            else
            {
                result?.Invoke(false, -1);
            }
        });
    }











    public void LoadScoreboard(string paginateUrl, int mode, Action<Paginate<UserScoreResponseData>> callback)
    {
        string url = paginateUrl != null ? paginateUrl : BaseApiUrl + "/getscoreboard?";
        url += "&mode=" + mode;

        Utils.Instance.GetRequest(url, (response) =>
          {
              Debug.Log(response);
              var res = JsonUtility.FromJson<Response<Paginate<UserScoreResponseData>>>(response);
              if (res.status.Equals("OK"))
              {
                  callback?.Invoke(res.data);
              }
              else
              {
                  callback?.Invoke(null);
              }
          });
    }

    public void LoadUserDataFromServer(int id, Action<UserDataMart.UserData> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/getuser?id=" + id, (response) =>
        {

            var res = JsonUtility.FromJson<Response<UserDataMart.UserData>>(response);

            if (res.status.Equals("OK"))
            {
                callback?.Invoke(res.data);
            }
            else
            {
                callback?.Invoke(null);
            }

        });
    }

    public void GetOnluckMetadata(Action<Response<OnluckMetadata>> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/getonluckmetadata",
        (response) =>
        {

            Debug.Log(response);

            var res = JsonUtility.FromJson<Response<OnluckMetadata>>(response);

            callback?.Invoke(res);
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
    public void NotifyServerOnGameStart(UserDataMart.UserData userData, Action<Response<GameStartData>> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/notifyserverongamestart?user_id=" + userData.id
            + "&vendor_name=" + userData.active_vendor,
        (response) =>
        {
            Debug.Log(response);

            var res = JsonUtility.FromJson<Response<GameStartData>>(response);

            callback?.Invoke(res);
        });
    }
    public void CheckForUptodateUserDataFromServer(UserDataMart.UserData userData, Action<Response<UserDataMart.UserData>> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl
            + "/checkuptodateuserdata?id=" + userData.id + "&uptodate_token=" + userData.uptodate_token,
            (response) =>
            {

                var res = JsonUtility.FromJson<Response<UserDataMart.UserData>>(response);

                callback?.Invoke(res);

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
        public int minus_score;
        public string images;
        public string hints;
    }
    [System.Serializable]
    public class TypedQuestion
    {
        public int id;
        public int pack_id;
        public string question;
        public string answers;
        public int score;
        public string images;
        public string hints;
    }
    [System.Serializable]
    public class QuestionWrapper
    {
        public int type;
        public TypedQuestion typed_question;
        public McqQuestion mcq_question;
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

    public void GetPlayingDataFromServer(Action<Response<PlayingDataMart.PlayingData>> callback, int userId)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/getplayingdata?user_id=" + userId, (response) =>
          {
              Debug.Log("GetPlayingDataFromServer:" + response);

              var res = JsonUtility.FromJson<Response<PlayingDataMart.PlayingData>>(response);

              callback?.Invoke(res);
          });
    }
    public void SendQuestionPlayingDataToServer(GameLogic.PlayingData playingData, Action<Response<int>> callback)
    {
        string json = JsonUtility.ToJson(playingData);

        Debug.Log("SendQuestionPlayingDataToServer: " + json);

        Utils.Instance.PostRequest(BaseApiUrl + "/storeplayingdata", json, (response) =>
        {

            Debug.Log("SendQuestionPlayingDataToServer:" + response);

            var res = JsonUtility.FromJson<Response<int>>(response);

            callback?.Invoke(res);
        });
    }

    public void GetQuestionById(int questionId, int type, Action<QuestionWrapper> callback)
    {
        Utils.Instance.GetRequest(BaseApiUrl + "/getquestionbyid?type=" + type + "&id=" + questionId, (response) =>
        {

            Debug.Log(response);

            var res = JsonUtility.FromJson<Response<QuestionWrapper>>(response);

            if (res.status.Equals("OK"))
            {
                callback(res.data);
            }
            else
            {
                callback(null);
            }
        });
    }

    [Serializable]
    public class SubmittingResponse
    {
        public int playing_data_uptodate_token;
        public int score;
    }

    public void SubmitQuestionPlayingData(PlayingDataMart.QuestionPlayingData questionPlayingData, Action<SubmittingResponse> callback)
    {
        string json = "{\"question_playing_data\":" + JsonUtility.ToJson(questionPlayingData) + "}";

        Debug.Log("submitquestionplayingdata: " + json);

        Utils.Instance.PostRequest(BaseApiUrl + "/submitquestionplayingdata", json, (response) =>
        {

            Debug.Log(response);

            var res = JsonUtility.FromJson<Response<SubmittingResponse>>(response);

            if (res.status.Equals("OK"))
            {
                callback?.Invoke(res.data);
            }
            else
            {
                callback?.Invoke(null);
            }
        });
    }
}

//public delegate void DownloadedCallback();
//private DownloadedCallback downloadedCallback = null;
//public void DownloadGameData(HttpClientResponseDataCallback<QuestionDataMart.Season> callback, DownloadedCallback callback2)
//{
//    downloadedCallback = callback2;
//    Utils.Instance.GetRequest(BaseApiUrl + "/downloadgamedata", (response) =>
//    {
//        Debug.Log(response);
//        imagesToDownloadCount = 0;
//        Response<Season> res = JsonUtility.FromJson<Response<Season>>(response);
//        if (res.status.Equals("OK"))
//        {
//            HttpClient.Season ss = res.data;
//            QuestionDataMart.Season season = new QuestionDataMart.Season
//            {
//                id = ss.id,
//                name = ss.name,
//                from = ss.from,
//                to = ss.to,
//                packs = new List<QuestionDataMart.Pack>()
//            };
//            foreach (HttpClient.Pack p in ss.packs)
//            {
//                QuestionDataMart.Pack pack = new QuestionDataMart.Pack
//                {
//                    id = p.id,
//                    title = p.title,
//                    sub_text = p.sub_text,
//                    icon = new QuestionDataMart.Image() { path = p.icon }
//                };
//                imagesToDownloadCount += 1;
//                Utils.Instance.LoadImage(BaseUrl + pack.icon.path,(texture)=> { 
//                    Debug.Log("Downloaded "+ pack.icon.path); 
//                    pack.icon.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,0)); 
//                    signalImageDownloaded(); });
//                pack.question_type = Int32.Parse(p.question_type);

//                if (pack.question_type == 0)
//                {
//                    pack.typed_questions = new List<QuestionDataMart.TypedQuestion>();
//                    foreach (HttpClient.TypedQuestion q in p.typed_questions)
//                    {
//                        QuestionDataMart.TypedQuestion question = new QuestionDataMart.TypedQuestion
//                        {
//                            id = q.id,
//                            question = q.question,
//                            answer = q.answer,
//                            score = q.score,
//                            images = new List<QuestionDataMart.Image>()
//                        };
//                        List<string> images = Utils.Instance.FromJsonList<List<string>>(q.images);
//                        imagesToDownloadCount += images.Count;
//                        foreach (string img in images)
//                        {
//                            QuestionDataMart.Image image = new QuestionDataMart.Image() { path = img };
//                            question.images.Add(image);
//                            Utils.Instance.LoadImage(BaseUrl + img,(texture)=> { Debug.Log("Downloaded "+img);
//                                image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,0));
//                                /*image.texture = texture;*/ signalImageDownloaded(); });
//                        }
//                        question.hints = new List<string>();
//                        List<string> hints = Utils.Instance.FromJsonList<List<string>>(q.hints);
//                        foreach (string hint in hints)
//                            question.hints.Add(hint);
//                        pack.typed_questions.Add(question);
//                    }
//                }
//                else if (pack.question_type == 1)
//                {
//                    pack.mcq_questions = new List<QuestionDataMart.MCQQuestion>();
//                    foreach (HttpClient.McqQuestion q in p.mcq_questions)
//                    {
//                        QuestionDataMart.MCQQuestion question = new QuestionDataMart.MCQQuestion
//                        {
//                            id = q.id,
//                            question = q.question,
//                            choices = Utils.Instance.FromJsonList<List<string>>(q.choices).ToArray(),
//                            answer = q.answer,
//                            time = q.time,
//                            score = q.score,
//                            images = new List<QuestionDataMart.Image>()
//                        };
//                        List<string> images = Utils.Instance.FromJsonList<List<string>>(q.images);
//                        imagesToDownloadCount += images.Count;
//                        foreach (string img in images)
//                        {
//                            QuestionDataMart.Image image = new QuestionDataMart.Image() { path = img };
//                            question.images.Add(image);
//                            Utils.Instance.LoadImage(BaseUrl + img, (texture) => { Debug.Log("Downloaded " + img); 
//                                image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,0));
//                                signalImageDownloaded(); });
//                        }
//                        //question.hints = new List<string>();
//                        question.hints = Utils.Instance.FromJsonList<List<string>>(q.hints);
//                        pack.mcq_questions.Add(question);
//                    }
//                }

//                season.packs.Add(pack);
//            }

//            callback(season);
//        }
//        if (imagesToDownloadCount == 0)
//            signalImageDownloaded();

//    });
//}
