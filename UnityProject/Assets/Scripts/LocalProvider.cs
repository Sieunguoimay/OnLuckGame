using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.DataMarts;
using System.Drawing;
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
using SFB;
#endif

public class LocalProvider
{
    private static LocalProvider s_instance = null;
    public static LocalProvider Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new LocalProvider();
            }
            return s_instance;
        }
    }
    private LocalProvider()
    {

    }

    List<UserDataMart.UserData> userDataList = new List<UserDataMart.UserData>();
    List<PlayingDataMart.PlayingData> playingDataList = new List<PlayingDataMart.PlayingData>();

    public void AddUserData(UserDataMart.UserData newUserData)
    {
        SaveUserData(newUserData, false);
        //bool isNewUserData = true;
        //for (int i = 0; i < userDataList.Count; i++)
        //{
        //    if (newUserData.id == userDataList[i].id)
        //    {
        //        isNewUserData = false;
        //        //replace the old one
        //        userDataList[i] = newUserData;
        //        if (newUserData.texProfilePicture != null)
        //            Utils.Instance.SaveBytesToFile(newUserData.texProfilePicture.EncodeToPNG(), Path.GetFileName(newUserData.profile_picture));
        //        break;
        //    }
        //}
        //Debug.Log("isNewUserData" + isNewUserData);
        //if (isNewUserData)
        //{
        //    userDataList.Add(newUserData);
        //    Utils.Instance.SaveBytesToFile(newUserData.texProfilePicture.EncodeToPNG(), Path.GetFileName(newUserData.profile_picture));
        //}
    }


    public delegate void BrowseImagePathCallback(string path);
    public void BrowseImagePath(BrowseImagePathCallback callback)
    {

//#if UNITY_EDITOR
//        string path = UnityEditor.EditorUtility.OpenFilePanel("Select Photo", "", "png,jpg,jpeg");
//        Debug.Log("Selected " + path);
//        callback(path);
//#else

        NativeGallery.GetImageFromGallery((path) => {
            Debug.Log("Selected " + path);
            callback(path);
        }, "Select Photo","image/*");
#if UNITY_IOS ||  UNITY_ANDROID
#endif

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        string path = StandaloneFileBrowser.OpenFilePanel("Select Photo", "", "png,jpg,jpeg",false)[0];
        Debug.Log("Selected " + path);
        callback(path);
#endif


    }

    public delegate void SaveImageCallback(bool status);
    public void SaveImage(Texture2D texture, string filename, SaveImageCallback callback = null)
    {

//#if UNITY_EDITOR
//        string path = UnityEditor.EditorUtility.SaveFilePanel("Save texture as PNG", "", filename, "png");
//        if (path.Length != 0)
//        {
//            var pngData = texture.EncodeToPNG();
//            if (pngData != null)
//            {
//                File.WriteAllBytes(path, pngData);
//                callback(true);
//                return;
//            }
//        }
//        callback(false);
//#endif

        var pngData = texture.EncodeToPNG();
        if (pngData != null){
            NativeGallery.SaveImageToGallery(pngData,"OnluckTheGame",filename, (success,path) => {
                Debug.Log("Log: " + success+" "+ path);
                callback(true);
            });
        }
#if UNITY_IOS || UNITY_ANDROID
#endif

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        string path = StandaloneFileBrowser.OpenFilePanel("Select Photo", "", "png,jpg,jpeg",false)[0];
        Debug.Log("Selected " + path);
#endif

    }
    [System.Serializable]
    public class ExtraIntegerWrapper<T>
    {
        public int extra_data;
        public T data;
    }
    public void SavePlayingData(PlayingDataMart.PlayingData playingData)
    {
        bool newPlayingData = true;
        for (int i = 0; i < playingDataList.Count; i++)
            if(playingDataList[i].user_id == playingData.user_id)
            {
                playingDataList[i] = playingData;
                newPlayingData = false;
                break;
            }
        if (newPlayingData)
            playingDataList.Add(playingData);
        Utils.Instance.SaveObjectToFile(playingDataList, "playing_data_list.json");
    }
    public PlayingDataMart.PlayingData GetPlayingDataByUserId(int userId)
    {
        playingDataList = Utils.Instance.LoadFileToObject<List<PlayingDataMart.PlayingData>>("playing_data_list.json");
        if (playingDataList == null)
            playingDataList = new List<PlayingDataMart.PlayingData>();
        foreach (PlayingDataMart.PlayingData playingData in playingDataList)
            if (playingData.user_id == userId)
                return playingData;
        return null;
    }

    public void SaveUserData(UserDataMart.UserData newUserData, bool saveImages = true)
    {
        Debug.Log("userDataList.Count" + userDataList.Count);
        int m_activeUserIndex = -1;
        if (saveImages)
        {
            bool isNewUserData = true;
            for (int i = 0; i < userDataList.Count; i++)
            {
                if (newUserData.id == userDataList[i].id)
                {
                    isNewUserData = false;

                    //replace the old one
                    userDataList[i] = newUserData;

                    if (newUserData.texProfilePicture != null)
                        Utils.Instance.SaveBytesToFile(newUserData.texProfilePicture.EncodeToPNG(), Path.GetFileName(newUserData.profile_picture));
                    m_activeUserIndex = i;
                    break;
                }
            }
            Debug.Log("isNewUserData" + isNewUserData);

            if (isNewUserData)
            {
                userDataList.Add(newUserData);
                m_activeUserIndex = userDataList.Count - 1;
                //Utils.Instance.SaveObjectToFile(userDataList, "last_active_user.onluck");
                Utils.Instance.SaveBytesToFile(newUserData.texProfilePicture.EncodeToPNG(), Path.GetFileName(newUserData.profile_picture));
            }
        }

        Debug.Log("userDataList.Count" + userDataList.Count);
        Utils.Instance.SaveObjectToFile(new ExtraIntegerWrapper<List<UserDataMart.UserData>>() { data = userDataList, extra_data = m_activeUserIndex }, "user_list.json");
    }
    //byte[] textureBytes = Utils.Instance.LoadFileToBytes(Path.GetFileName(userData.profile_picture));
    //if (textureBytes != null)
    //{
    //    userData.texProfilePicture = new Texture2D(2,2);
    //    userData.texProfilePicture.LoadImage(textureBytes);
    //}


    public delegate void LoadLocalDataCallback<T>(T data);
    public UserDataMart.UserData LoadUserData()
    {
        ExtraIntegerWrapper<List<UserDataMart.UserData>> data = Utils.Instance.LoadFileToObject<ExtraIntegerWrapper<List<UserDataMart.UserData>>>("user_list.json");
        if (data == null) {
            userDataList = new List<UserDataMart.UserData>();
            return null;
        }
        else
        {
            userDataList = data.data;
            foreach (UserDataMart.UserData userData in userDataList)
                userData.texProfilePicture = Utils.Instance.LoadFileToTexture(Path.GetFileName(userData.profile_picture));

            if (data.extra_data != -1)
                return userDataList[data.extra_data];
            else
                return null;
        }
    }
    public UserDataMart.UserData GetUserDataByEmail(string email)
    {
        if (userDataList != null)
        {
            foreach (UserDataMart.UserData userData in userDataList)
            {
                if (userData.email.Equals(email))
                {
                    return userData;
                }
            }
        }
        Debug.Log("LocalProvider: Failed to load user data from local");
        return null;
    }
    public void LoadQuestionData(LoadLocalDataCallback<QuestionDataMart.Season> callback)
    {
        QuestionDataMart.Season season = Utils.Instance.LoadFileToObject<QuestionDataMart.Season>(QuestionDataMart.Instance.onluckLocalMetadata.season_name+".json");
        foreach(QuestionDataMart.Pack pack in season.packs)
        {
            Texture2D texture = Utils.Instance.LoadFileToTexture(Path.GetFileName(pack.icon.path));
            pack.icon.sprite = Sprite.Create(texture,new Rect(0,0, texture.width, texture.height),new Vector2(0,0));
            //if (pack.typed_questions.Count > 0) { 
            //    pack.question_type = 0;
            //}
            //else if(pack.mcq_questions.Count>0)
            //    pack.question_type = 1;
            //else
            //    pack.question_type = -1;
            if(pack.question_type == 0)
            {
                foreach (QuestionDataMart.TypedQuestion question in pack.typed_questions)
                {
                    foreach(QuestionDataMart.Image image in question.images)
                    {
                        //Texture2D t = Utils.Instance.LoadFileToTexture(Path.GetFileName(image.path));
                        //image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
                        Debug.Log("Utils.Instance.LoadTextureFileAsync:"+ image.path);
                        Utils.Instance.LoadTextureFileAsync(Path.GetFileName(image.path), (t) =>
                        {
                            Debug.Log("Utils.Instance.LoadTextureFileAsync: Loaded"+ image.path);
                            image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
                        });
                    }
                }
                    //Debug.Log(question.id + " " + question.question + " " + question.images.Count);
            }else if(pack.question_type == 1)
            {
                foreach (QuestionDataMart.MCQQuestion question in pack.mcq_questions)
                {
                    foreach (QuestionDataMart.Image image in question.images)
                    {
                        //Texture2D t = Utils.Instance.LoadFileToTexture(Path.GetFileName(image.path));
                        //image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
                        Utils.Instance.LoadTextureFileAsync(Path.GetFileName(image.path), (t) =>
                        {
                            image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
                        });
                    }
                }
            }
            //Debug.Log("question_type: " 
            //    + pack.question_type +" "+ pack.typed_questions.Count +" "+pack.mcq_questions.Count);
        }
        callback(season);
    }
    public void ClearQuestionData()
    {
        QuestionDataMart.Season season = Utils.Instance.LoadFileToObject<QuestionDataMart.Season>(QuestionDataMart.Instance.onluckLocalMetadata.season_name+".json");
        Debug.Log("Let's clear them all " + QuestionDataMart.Instance.onluckLocalMetadata.season_name + ".json");
        if (season == null)
        {
            Debug.Log("NOOOOOOOOOOO!!!!!!!");
            return;
        }
        foreach (QuestionDataMart.Pack pack in season.packs)
        {
            Utils.Instance.DeleteFile(Path.GetFileName(pack.icon.path));
            if (pack.question_type == 0)
            {
                foreach (QuestionDataMart.TypedQuestion question in pack.typed_questions)
                {
                    foreach (QuestionDataMart.Image image in question.images)
                    {
                        Utils.Instance.DeleteFile(Path.GetFileName(image.path));
                    }
                }
            }
            else if (pack.question_type == 1)
            {
                foreach (QuestionDataMart.MCQQuestion question in pack.mcq_questions)
                {
                    foreach (QuestionDataMart.Image image in question.images)
                    {
                        Utils.Instance.DeleteFile(Path.GetFileName(image.path));
                    }
                }
            }
        }
        Utils.Instance.DeleteFile(season.name+".json");
    }
    public void SaveQuestionData(QuestionDataMart.Season season)
    {
        foreach (QuestionDataMart.Pack pack in season.packs)
        {
            Utils.Instance.SaveBytesToFile(pack.icon.sprite.texture.EncodeToPNG(), Path.GetFileName(pack.icon.path));
            //pack.icon.texture = Utils.Instance.LoadFileToTexture(Path.GetFileName(pack.icon.path));
            if (pack.question_type == 0)
            {
                foreach (QuestionDataMart.TypedQuestion question in pack.typed_questions)
                {
                    foreach (QuestionDataMart.Image image in question.images)
                    {
                        //image.texture = Utils.Instance.LoadFileToTexture(Path.GetFileName(image.path));
                        Utils.Instance.SaveBytesToFile(image.sprite.texture.EncodeToPNG(), Path.GetFileName(image.path));
                    }
                }
            }
            else if (pack.question_type == 1)
            {
                foreach (QuestionDataMart.MCQQuestion question in pack.mcq_questions)
                {
                    foreach (QuestionDataMart.Image image in question.images)
                    {
                        //image.texture = Utils.Instance.LoadFileToTexture(Path.GetFileName(image.path));
                        Utils.Instance.SaveBytesToFile(image.sprite.texture.EncodeToPNG(), Path.GetFileName(image.path));
                    }
                }
            }
        }
        Utils.Instance.SaveObjectToFile(season,season.name+".json");// new ExtraIntegerWrapper<List<UserDataMart.UserData>>() { data = userDataList, extra_data = m_activeUserIndex }, "user_list.json");
    }
    public QuestionDataMart.OnluckLocalMetadata LoadOnluckLocalMetadata()
    {
        return Utils.Instance.LoadFileToObject<QuestionDataMart.OnluckLocalMetadata>("onluck.json");
    }
    public void SaveOnluckLocalMetadata(QuestionDataMart.OnluckLocalMetadata metadata)
    {
        Utils.Instance.SaveObjectToFile<QuestionDataMart.OnluckLocalMetadata>(metadata,"onluck.json");
    }

    public AssetsDataMart.AssetsData LoadAssetsData()
    {
        return JsonUtility.FromJson<AssetsDataMart.AssetsData>(((TextAsset)Resources.Load("Files/assets_data")).text);
        // Utils.Instance.LoadFileToObject<AssetsDataMart.AssetsData>("assets_data.json");
    }
}
