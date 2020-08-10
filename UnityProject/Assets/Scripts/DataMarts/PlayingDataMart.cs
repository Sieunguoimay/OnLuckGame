using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.DataMarts
{
    public class PlayingDataMart
    {
        //public class AnsweredQuestion
        //{
        //    public int m_questionId;
        //    public DateTime m_started;
        //    public DateTime m_ended;
        //    public int m_usedHintCount;
        //    public int m_score;
        //}
        //public class ActiveTime
        //{
        //    public DateTime m_started;
        //    public DateTime m_ended;
        //}
        //public class UnlockedQuestion
        //{
        //    public int pack;
        //    public int question;
        //}
        //public class PlayingData
        //{

        //    public List<AnsweredQuestion> m_answeredQuestions = new List<AnsweredQuestion>();
        //    public List<ActiveTime> m_activeTimes = new List<ActiveTime>();
        //    public int m_score;
        //    public int m_unlockedQuestions;
        //}


        /*This class is a Singleton*/
        private static PlayingDataMart s_instance = null;
        public static PlayingDataMart Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new PlayingDataMart();
                return s_instance;
            }
        }
        private PlayingDataMart() { }
        /*End of Singleton Declaration*/




        //public PlayingData m_playingData { get; private set; }
        public class QuestionPlayingData
        {
            public char m_status = 'l';//l-locked, u-unlocked, w-wrongly answered, c-correctly answered
            public int m_questionId = -1;
            public DateTime m_started;
            public DateTime m_ended;
            public int m_usedHintCount = 0;
            public int m_score = 0;
        }

        public List<QuestionPlayingData> m_questionImage80 = new List<QuestionPlayingData>();
        public List<QuestionPlayingData> m_questionImage1000 = new List<QuestionPlayingData>();
        public List<QuestionPlayingData> m_questionImage30 = new List<QuestionPlayingData>();

        public int m_currentQuestionIndex80 { get; private set; } = 0;
        public int m_currentQuestionIndex1000 { get; private set; } = 0;
        public int m_currentQuestionIndex30 { get; private set; } = 0;

        public delegate void ScoreChangedCallback(bool increased);
        public ScoreChangedCallback m_scoreChangedCallback = null;
        private int score;
        public int m_score
        {
            set
            {
                if (score != value)
                {
                    bool increased = (value > score);
                    score = value;
                    if(m_scoreChangedCallback!=null)
                        m_scoreChangedCallback(increased);
                }
            }
            get
            {
                return score;
            }
        }
        //private char[] getQuestionImage(GameScene.MainGamePresenter.Packs pack)
        //{
        //    switch (pack)
        //    {
        //        case GameScene.MainGamePresenter.Packs.PACK_80:
        //            return m_questionImage80;
        //        case GameScene.MainGamePresenter.Packs.PACK_1000:
        //            return m_questionImage1000;
        //        case GameScene.MainGamePresenter.Packs.PACK_30:
        //            return m_questionImage30;
        //    }
        //    return null;
        //}


        public void LoadPlayingData()
        {
            /*For now it's a fake loading function*/
            QuestionDataMart.Instance.m_80QuestionPack.ForEach((question) =>
            {
                CreateNewQuestionPlayingData(0, question.id);
            });
            QuestionDataMart.Instance.m_1000QuestionPack.ForEach((question) =>
            {
                CreateNewQuestionPlayingData(1, question.id);
            });
            QuestionDataMart.Instance.m_30QuestionPack.ForEach((question) =>
            {
                CreateNewQuestionPlayingData(2, question.id);
            });

            Debug.Log("Loaded playing data "+ m_questionImage80.Count+" "+ m_questionImage1000.Count+ " "+m_questionImage30.Count);
        }

        public void CreateNewQuestionPlayingData(int pack, int questionId)
        {
            QuestionPlayingData questionPlayingData = new QuestionPlayingData();
            questionPlayingData.m_questionId = questionId;

            switch (pack)
            {
                case 0:
                    if (m_questionImage80.Count == 0)
                        questionPlayingData.m_status = 'u';
                    m_questionImage80.Add(questionPlayingData);
                    break;
                case 1:
                    if (m_questionImage1000.Count == 0)
                        questionPlayingData.m_status = 'u';
                    m_questionImage1000.Add(questionPlayingData);
                    break;
                case 2:
                    if (m_questionImage30.Count == 0)
                        questionPlayingData.m_status = 'u';
                    m_questionImage30.Add(questionPlayingData);
                    break;
            }
        }
        public void OnPlayingDataOutputted(int pack, int currentIndex)
        {
            Debug.Log("Output to play data mart " + currentIndex);
            switch (pack)
            {
                case 0:
                    m_currentQuestionIndex80 = currentIndex;
                    break;
                case 1:
                    m_currentQuestionIndex1000 = currentIndex;
                    break;
                case 2:
                    m_currentQuestionIndex30 = currentIndex;
                    break;
            }

        }
        //public void AddNewAnsweredQuestion(AnsweredQuestion answeredQuestion, GameScene.MainGamePresenter.Packs pack, int index) 
        //{
        //    m_playingData.m_score += answeredQuestion.m_score;
        //    m_playingData.m_answeredQuestions.Add(answeredQuestion);
        //    getQuestionMap(pack)[index] = m_playingData.m_answeredQuestions.Count - 1;
        //}
        //public AnsweredQuestion GetAnsweredQuestionByQuestionIndex(GameScene.MainGamePresenter.Packs pack,int index)
        //{
        //    int answeredQuestionIndex = getQuestionMap(pack)[index];
        //    if (answeredQuestionIndex >= 0)
        //    {
        //        return m_playingData.m_answeredQuestions[answeredQuestionIndex];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public void SubtractScore(int score)
        {
            if (m_score > 0)
            {
                m_score -= score;
            }
            else
            {
                m_score = 0;
            }
        }
        public void AddScore(int score)
        {
            m_score += score;
        }
    }
}
