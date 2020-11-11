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
    public interface ILoadQuestionListener
    {
        void OnLoadQuestionFailed();
        void OnLoadQuestionResult(QuestionDataMart.TypedQuestion typedQuestion, QuestionDataMart.MCQQuestion mcqQuestion);
        void OnLoadImageResult(QuestionDataMart.Image image);
    }

    public class QuestionDataMart:MonoBehaviourSingleton<QuestionDataMart>
    {


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
        public class EmptyQuestion
        {
            public int id;
            //public string question;
            //public List<Image> images;
            //public int score;
            //public List<string> hints;
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
            public List<string> answers;

            public bool CheckAnswer(string answer)
            {
                answer = Utils.Instance.convertToUnSign3(answer.ToLower());

                foreach (var a in answers)
                {
                    var _a = Utils.Instance.convertToUnSign3(a.ToLower());

                    if (_a.Equals(answer))
                    {
                        return true;
                    }
                }
                return false;
            }
            public string FirstAnswer
            {
                get
                {
                    if (answers.Count > 0)
                    {
                        return answers[0];
                    }
                    return "";
                }
            }
        }
        [Serializable]
        public class MCQQuestion : Question
        {
            public int answer;
            public int time;
            public int minus_score;
            public string[] choices;
        }

        [Serializable]
        public class Pack
        {
            public int id;
            public string title;
            public string sub_text;
            public Image icon;
            public List<EmptyQuestion> questions;
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

        public Action m_gameDataReadyCallback = delegate { };
        public Action m_gameDataCompletedCallback = delegate { };
        public Action m_askForPermissionCallback = delegate { };
        public Action<string, string, float ,float> progressCallback = null;

        public OnluckLocalMetadata onluckLocalMetadata = null;
        public Action<float> publishProgress = delegate { };
        private int activationCode = 0;

        public ILoadQuestionListener QuestionLoadListener = null;

        public void Init()
        {
        }
        public bool LoadMetadata()
        {
            publishProgress?.Invoke(0.1f);

            var metadata = LocalProvider.Instance.LoadOnluckLocalMetadata();

            if (metadata != null)
            {
                Debug.Log("Loaded local onluck metadata " + metadata.activation_code);
                onluckLocalMetadata = metadata;
                publishProgress?.Invoke(0.3f);

                return true;
            }
            onluckLocalMetadata = new OnluckLocalMetadata();
            return false;
        }

        public void SetFromServerOnluckMetadata(MonoBehaviour context,HttpClient.OnluckMetadata metadata)
        {
            publishProgress?.Invoke(0.6f);
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
                LocalProvider.Instance.LoadQuestionData(context, (ss) =>
                {
                    season = ss;
                    if (season != null)
                    {
                        packs = season.packs;

                        Debug.Log("Loaded Question Data " + packs.Count);
                        publishProgress?.Invoke(1.0f);
                        m_gameDataReadyCallback?.Invoke();
                        m_gameDataCompletedCallback?.Invoke();
                    }
                });
            }
            else
            {
                activationCode = metadata.activation_code;
                Debug.Log("You need to download this new data. otherwise, no game");
                //ask menu presenter for permission
                m_askForPermissionCallback?.Invoke();
            }

        }
        public void OnPermissionGranted()
        {
            Debug.Log("Zeze PermissionGranted");
            m_gameDataCompletedCallback();
            publishProgress?.Invoke(1.0f);

            var gameDataDownloader = new GameObject("GameDataDownloader",typeof(GameDataDownloader));
            gameDataDownloader.transform.parent = transform;

            LocalProvider.Instance.ClearQuestionData();
            gameDataDownloader.GetComponent<GameDataDownloader>().progressCallback = progressCallback;// MenuPresenter.Instance.DisplayDownloadProgress;
            gameDataDownloader.GetComponent<GameDataDownloader>().onDoneCallback = (ss) => {
                onluckLocalMetadata.activation_code = activationCode;
                onluckLocalMetadata.season_name = ss.name;
                season = ss;
                packs = season.packs;
                m_gameDataReadyCallback?.Invoke();
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

        public void LoadQuestion(int id, int type)
        {
            HttpClient.Instance.GetQuestionById(id,type, (questionWrapper) =>
            {
                bool success = false;

                TypedQuestion downloadedTypedQuestion = null;
                MCQQuestion downloadedMCQQuestion = null;

                if (questionWrapper != null)
                {
                    if (questionWrapper.type == 0)
                    {
                        var q = questionWrapper.typed_question;
                        if (q != null)
                        {
                            downloadedTypedQuestion = new QuestionDataMart.TypedQuestion()
                            {
                                id = q.id,
                                question = q.question,
                                answers = ParseStringToList(q.answers),
                                score = q.score,
                                images = new List<QuestionDataMart.Image>(),
                                hints = ParseStringToList(q.hints)
                            };

                            LoadImages(q.images, downloadedTypedQuestion.images);

                            success = true;
                        }
                    }
                    else
                    {
                        var q = questionWrapper.mcq_question;
                        if (q != null)
                        {
                            downloadedMCQQuestion = new QuestionDataMart.MCQQuestion()
                            {
                                id = q.id,
                                question = q.question,
                                choices = Utils.Instance.FromJsonList<List<string>>(q.choices).ToArray(),
                                answer = q.answer,
                                time = q.time,
                                score = q.score,
                                minus_score = q.minus_score,
                                images = new List<QuestionDataMart.Image>(),
                                hints = ParseStringToList(q.hints)
                            };

                            LoadImages(q.images, downloadedMCQQuestion.images);

                            success = true;
                        }
                    }
                }

                if (success)
                {
                    //openQuestion(index);
                    QuestionLoadListener?.OnLoadQuestionResult(downloadedTypedQuestion,downloadedMCQQuestion);
                }
                else
                {
                    //mainGame.EnableUIOnLoadQuestionFailed();
                    QuestionLoadListener?.OnLoadQuestionFailed();
                }
            });
        }

        private void LoadImages(string str, List<Image> imageList)
        {
            if (!str.Equals(""))
            {
                var images = Utils.Instance.FromJsonList<List<string>>(str);

                foreach (string img in images)
                {
                    var image = new QuestionDataMart.Image() { path = img };

                    imageList.Add(image);

                    Utils.Instance.LoadImage(HttpClient.Instance.BaseUrl + img, (texture) =>
                    {
                        Debug.Log("Downloaded " + img);

                        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

                        QuestionLoadListener?.OnLoadImageResult(image);
                        //mainGame.UiImageSlideshow.SetQuestionImages(downloadedTypedQuestion.images);
                    });
                }
            }
            else
            {
                //mainGame.UiImageSlideshow.SetToDefault();
                QuestionLoadListener?.OnLoadImageResult(null);
            }

        }
        private List<string> ParseStringToList(string str)
        {
            
            //return str.Split(';').ToList<string>();

            var hintList = new List<string>();

            if (!str.Equals(""))
            {
                var hints = Utils.Instance.FromJsonList<List<string>>(str);

                foreach (string hint in hints)
                {
                    hintList.Add(hint);
                }
            }
            return hintList;
        }
    }
}
//                            if (!q.images.Equals(""))
//                            {
//                                var images = Utils.Instance.FromJsonList<List<string>>(q.images);

//                                foreach (string img in images)
//                                {
//                                    var image = new QuestionDataMart.Image() { path = img };

//downloadedMCQQuestion.images.Add(image);

//                                    Utils.Instance.LoadImage(HttpClient.Instance.BaseUrl + img, (texture) =>
//                                    {
//                                        Debug.Log("Downloaded " + img);

//                                        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

//                                        mainGame.UiImageSlideshow.SetQuestionImages(downloadedMCQQuestion.images);
//                                    });
//                                }
//                            }
//                            else
//                            {
//                                mainGame.UiImageSlideshow.SetToDefault();
//                            }

