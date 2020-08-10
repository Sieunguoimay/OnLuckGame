using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataMarts
{
    public class QuestionDataMart
    {
        public class Question
        {
            public int id;
            public string question;

            public string image;
            public Texture2D texImage;

            public int pack;
            public int type;
            public int number;
            public int score;

            //public AnswerStatuses answerStatus;
            public List<string> hints;
            //public int usedHintCount;
        }

        public class TypedQuestion : Question
        {
            public string answer;
        }
        public class MCQQuestion : Question
        {
            public int answer;
            public int time;
            public string[] choices;
        }
        public class QuestionPackMetadata
        {
            public string title;
            public string subText;
            public string icon;
            public Texture2D texIcon;
            public int id;
        }
        public delegate void GameDataReadyCallback();

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



        public List<TypedQuestion> m_1000QuestionPack = new List<TypedQuestion>();
        public List<TypedQuestion> m_80QuestionPack = new List<TypedQuestion>();
        public List<MCQQuestion> m_30QuestionPack = new List<MCQQuestion>();

        public List<QuestionPackMetadata> m_packMetadata = new List<QuestionPackMetadata>();

        public GameDataReadyCallback m_gameDataReadyCallback = null;

        public void LoadGameData()
        {
            {
                QuestionPackMetadata packMetadata = new QuestionPackMetadata();
                packMetadata.id = 0;
                packMetadata.title = "THU THACH";
                packMetadata.subText = "HUONG DAN";
                packMetadata.icon = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                Utils.Instance.LoadImage(packMetadata.icon, (texture) => {
                    packMetadata.texIcon = texture;
                    Debug.Log("loaded pack icon");
                });
                m_packMetadata.Add(packMetadata);
            }
            {
                QuestionPackMetadata packMetadata = new QuestionPackMetadata();
                packMetadata.id = 1;
                packMetadata.title = "KIEN THUC";
                packMetadata.subText = "TRAC NGHIEM";
                packMetadata.icon = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                Utils.Instance.LoadImage(packMetadata.icon, (texture) => {
                    packMetadata.texIcon = texture;
                    Debug.Log("loaded pack icon");
                });
                m_packMetadata.Add(packMetadata);
            }
            {
                QuestionPackMetadata packMetadata = new QuestionPackMetadata();
                packMetadata.id = 2;
                packMetadata.title = "THUONG HIEU";
                packMetadata.subText = "TRAC NGHIEM";
                packMetadata.icon = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                Utils.Instance.LoadImage(packMetadata.icon, (texture) => {
                    packMetadata.texIcon = texture;
                    Debug.Log("loaded pack icon");
                });
                m_packMetadata.Add(packMetadata);
            }









            int id = 0;
            int number = 0;
            //when do we load the game data?
            //ofcourse from the very beginning
            {

                TypedQuestion question = new TypedQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Bac Ho ten that la gi?";
                question.type = 0;
                question.answer = "Nguyen Sinh Cung";
                question.pack = 0;
                question.score = 50;
                //question.time = 0;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");
                //});
                question.texImage = AssetsDataMart.Instance.hoChiMinhSprite;

                question.hints = new List<string>();
                question.hints.Add("Co ho Nguyen");
                question.hints.Add("Ten dem la Van");
                question.hints.Add("Ten goi bat dau bang chu B");
                //question.usedHintCount = 0;

                m_80QuestionPack.Add(question);

            }
            //when do we load the game data?
            //ofcourse from the very beginning
            {
                TypedQuestion question = new TypedQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Thu do cua Viet Nam la gi?";
                question.type = 0;
                question.answer = "Ha Noi";
                question.pack = 0;
                question.score = 50;
                //question.time = 0;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");

                //});
                question.texImage = AssetsDataMart.Instance.noImageSprite;
                question.hints = new List<string>();
                question.hints.Add("Bat dau bang chu H");
                //question.usedHintCount = 0;

                m_80QuestionPack.Add(question);

            }            //when do we load the game data?
            //ofcourse from the very beginning
            {
                TypedQuestion question = new TypedQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Thu do cua Iran la gi?";
                question.type = 0;
                question.answer = "Tehran";
                question.pack = 0;
                question.score = 50;
                //question.time = 0;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");

                //});
                question.texImage = AssetsDataMart.Instance.noImageSprite;
                question.hints = new List<string>();
                question.hints.Add("T");
                question.hints.Add("Te");
                question.hints.Add("Teh");
                //question.usedHintCount = 0;

                m_80QuestionPack.Add(question);

            }            //when do we load the game data?
            //ofcourse from the very beginning
            {
                TypedQuestion question = new TypedQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Thu do cua Qatar?";
                question.type = 0;
                question.answer = "Doha";
                question.pack = 0;
                question.score = 50;
                //question.time = 0;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");

                //});
                question.texImage = AssetsDataMart.Instance.noImageSprite;
                question.hints = new List<string>();
                question.hints.Add("D");
                question.hints.Add("Do");
                question.hints.Add("Doh");
                question.hints.Add("Doha");
                //question.usedHintCount = 0;

                m_80QuestionPack.Add(question);

            }            //when do we load the game data?
            //ofcourse from the very beginning
            {
                TypedQuestion question = new TypedQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Thu do cua Uzbekistan?";
                question.type = 0;
                question.answer = "Tashkent";
                question.pack = 0;
                question.score = 50;
                //question.time = 0;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");

                //});
                question.texImage = AssetsDataMart.Instance.noImageSprite;
                question.hints = new List<string>();
                question.hints.Add("Tashkent");
                //question.usedHintCount = 0;

                m_80QuestionPack.Add(question);

            }



            number = 0;

            //ofcourse from the very beginning
            {
                TypedQuestion question = new TypedQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Ngon ngu lap trinh pho bien nhat tren the gioi la gi?";
                question.type = 0;
                question.answer = "Python";
                question.pack = 0;
                question.score = 50;
                //question.time = 0;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");

                //});
                question.texImage = AssetsDataMart.Instance.noImageSprite;
                question.hints = new List<string>();
                question.hints.Add("Py");
                question.hints.Add("Pyth");
                question.hints.Add("Python");
                //question.usedHintCount = 0;

                m_1000QuestionPack.Add(question);

            }













            number = 0;
            {
                MCQQuestion question = new MCQQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Day la dat nuoc nao?";
                question.answer = 2;
                question.choices = new string[] { "America", "India", "Turkey", "Japan" };
                question.pack = 0;
                question.type = 0;
                question.time = 10;
                question.score = 50;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");
                //});
                question.texImage = AssetsDataMart.Instance.turkeySprite;
                question.hints = new List<string>();
                question.hints.Add("Chau A");
                question.hints.Add("Bien den");
                //question.usedHintCount = 0;

                m_30QuestionPack.Add(question);
            }
            {
                MCQQuestion question = new MCQQuestion();
                question.id = id++;
                question.number = number++;
                question.question = "Dinh Averest nam o nuoc nao?";
                question.answer = 3;
                question.choices = new string[] { "Trung Quoc", "An Do", "Buhtan", "Nepal" };
                question.pack = 0;
                question.type = 0;
                question.time = 10;
                question.score = 50;
                //question.answerStatus = AnswerStatuses.NOT_ANSWERED;
                question.image = HttpClient.Instance.BaseUrl + "/assets/icons/default_profile_picture.png";
                //Utils.Instance.LoadImage(question.image, (texture) => {
                //    question.texImage = texture;
                //    Debug.Log("loaded image");
                //});
                question.texImage = AssetsDataMart.Instance.noImageSprite;
                question.hints = new List<string>();
                question.hints.Add("This is a hint");
                question.hints.Add("This is another hint");
                question.hints.Add("This is the final hint");
                //question.usedHintCount = 0;

                m_30QuestionPack.Add(question);
            }

            Debug.Log("Loaded Question Data " + m_80QuestionPack.Count + " " + m_1000QuestionPack.Count + " " + m_30QuestionPack.Count);
            m_gameDataReadyCallback();
        }
        public List<QuestionPackMetadata> GetQuestionPacksMetadata()
        {
            return m_packMetadata;
        }
        public int GetSeasonNumber()
        {
            return 1;
        }
    }
}
