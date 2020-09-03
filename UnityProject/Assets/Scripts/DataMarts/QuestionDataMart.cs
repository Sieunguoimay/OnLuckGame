using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts;
namespace Assets.Scripts.DataMarts
{
    public class QuestionDataMart
    {

        /*This class is a Singleton*/
        private static QuestionDataMart s_instance = null;
        public static QuestionDataMart Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new QuestionDataMart();
                return s_instance;
            }
        }
        private QuestionDataMart() { }
        /*End of Singleton Declaration*/

        [System.Serializable]
        public class Season
        {
            public int id;
            public string name;
            public string from;
            public string to;
            public List<Pack> packs;
        }

        [Serializable]
        public class Image {
            public string path;
            //[NonSerialized]
            //public Texture2D texture;
            [NonSerialized]
            public Sprite sprite;
        }
        [Serializable]
        public class Question
        {
            public int id;
            public string question;
            public List<Image> images;
            public int score;
            public List<string> hints;
        }

        [Serializable]
        public class TypedQuestion : Question
        {
            public string answer;
        }
        [Serializable]
        public class MCQQuestion : Question
        {
            public int answer;
            public int time;
            public string[] choices;
        }
        [Serializable]
        public class Pack
        {
            public int id;
            public string title;
            public string sub_text;
            public Image icon;
            public List<TypedQuestion> typed_questions;
            public List<MCQQuestion> mcq_questions;
            public int question_type;
        }

 
        [Serializable]
        public class OnluckLocalMetadata
        {
            public int activation_code = -1;
            public int uptodate_token = -1;
            public string season_name;
            public string quote;
            public string intro_content;
            public string guideline_content;
        }

        public List<Pack> packs = new List<Pack>();

        public Season season { get; private set; } = new Season();

        public delegate void GameDataReadyCallback();
        public delegate void AskForPermissionCallback();
        public GameDataReadyCallback m_gameDataReadyCallback = null;
        public GameDataReadyCallback m_gameDataCompletedCallback = null;
        public AskForPermissionCallback m_askForPermissionCallback = null;

        public OnluckLocalMetadata onluckLocalMetadata = null;
        public ProgressBar.ProgressPublisher progressPublisher;
        private int activationCode = 0;

        public void Init()
        {
            m_gameDataReadyCallback = null;
            m_gameDataCompletedCallback = null;
            m_askForPermissionCallback = null;
            progressPublisher = new ProgressBar.ProgressPublisher();
        }
        public bool LoadMetadata()
        {
            progressPublisher.publishProgress(0.1f);
            OnluckLocalMetadata metadata = LocalProvider.Instance.LoadOnluckLocalMetadata();
            if (metadata != null)
            {
                Debug.Log("Loaded local onluck metadata " + metadata.activation_code);
                onluckLocalMetadata = metadata;
                progressPublisher.publishProgress(0.3f);

                return true;
            }
            onluckLocalMetadata = new OnluckLocalMetadata();
            return false;
            //LocalProvider.Instance.LoadQuestionData((loadedPacks) =>
            //{
            //    packs = loadedPacks;

            //    Debug.Log("Loaded Question Data " + packs.Count);
            //    m_gameDataReadyCallback();
            //});
        }

        public void SetFromServerOnluckMetadata(HttpClient.OnluckMetadata metadata)
        {
            progressPublisher.publishProgress(0.6f);
            if (onluckLocalMetadata.uptodate_token != metadata.uptodate_token)
            {
                onluckLocalMetadata.uptodate_token = metadata.uptodate_token;
                onluckLocalMetadata.quote = metadata.quote;
                onluckLocalMetadata.intro_content = metadata.intro_content;
                onluckLocalMetadata.guideline_content = metadata.guideline_content;
                LocalProvider.Instance.SaveOnluckLocalMetadata(onluckLocalMetadata);
            }
            if (onluckLocalMetadata.activation_code == metadata.activation_code)
            {
                Debug.Log("You're good to go. let's go to load local question data ");
                season =  LocalProvider.Instance.LoadQuestionData();
                if(season!=null) {
                    packs = season.packs;

                    Debug.Log("Loaded Question Data " + packs.Count);
                    progressPublisher.publishProgress(1.0f);
                    m_gameDataReadyCallback();
                    m_gameDataCompletedCallback();
                }
            }
            else
            {
                activationCode = metadata.activation_code;
                Debug.Log("You need to download this new data. otherwise, no game");
                //ask menu presenter for permission
                m_askForPermissionCallback();
            }

        }
        public void OnPermissionGranted()
        {
            Debug.Log("Zeze PermissionGranted");
            m_gameDataCompletedCallback();
            progressPublisher.publishProgress(1.0f);

            GameObject gameDataDownloader = new GameObject("GameDataDownloader",typeof(GameDataDownloader));

            LocalProvider.Instance.ClearQuestionData();
            gameDataDownloader.GetComponent<GameDataDownloader>().progressCallback = MenuPresenter.Instance.DisplayDownloadProgress;
            gameDataDownloader.GetComponent<GameDataDownloader>().onDoneCallback = (ss) => {
                onluckLocalMetadata.activation_code = activationCode;
                onluckLocalMetadata.season_name = ss.name;
                season = ss;
                packs = season.packs;
                m_gameDataReadyCallback();
                //LocalProvider.Instance.SaveQuestionData(season);
                LocalProvider.Instance.SaveOnluckLocalMetadata(onluckLocalMetadata);
                GameObject.Destroy(gameDataDownloader);
                
            };

            //HttpClient.Instance.DownloadGameData((ss) => {
            //    LocalProvider.Instance.ClearQuestionData();

            //    onluckLocalMetadata.activation_code = activationCode;
            //    onluckLocalMetadata.season_name = ss.name;
            //    season = ss;
            //    packs = season.packs;
            //    m_gameDataReadyCallback();
            //},()=> {
            //    Debug.Log("Save the downloaded data");
            //    progressPublisher.publishProgress(1.0f);
            //    m_gameDataCompletedCallback();
            //    LocalProvider.Instance.SaveQuestionData(season);
            //    LocalProvider.Instance.SaveOnluckLocalMetadata(onluckLocalMetadata);
            //});
        }
    }
}
