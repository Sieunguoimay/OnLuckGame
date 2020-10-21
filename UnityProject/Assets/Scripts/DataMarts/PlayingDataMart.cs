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


        [Serializable]
        public class ActiveTime
        {
            public string started;
            public string ended;
            public int answered_question_count;
        }

        [Serializable]
        public class PlayingData
        {
            public int id;
            public int user_id;
            public int total_score;
            public int uptodate_token;

            public List<int> current_question_indices;//pack index

            public List<PlayingPack> playing_packs;
        }



        [Serializable]
        public class QuestionPlayingData
        {
            public int id;
            public int question_id;
            public int pack_id;
            public int playing_data_id;

            public char status = 'l';//l-locked, u-unlocked, w-wrongly answered, c-correctly answered
            public string started;
            public string ended;
            public int used_hint_count = 0;
            public int score = 0;

            [NonSerialized]
            public bool dirtyFlag = false;//used in game
        }

        [Serializable]
        public class PlayingPack
        {
            public int id;
            public List<QuestionPlayingData> playing_questions;
        }

        public class Pack
        {
            public int currentIndex;
            public int id;
            public List<QuestionPlayingData> playingQuestions;
            public Dictionary<int, int> questionDictionary;
        }

        public PlayingData playingData = null;
        public List<Pack> playingPacks = new List<Pack>();
        public Dictionary<int, int> packDictionary = new Dictionary<int, int>();


        public delegate void ScoreChangedCallback(bool increased);
        public ScoreChangedCallback m_scoreChangedCallback = null;



        public int m_score
        {
            get
            {
                return ((playingData != null)? playingData.total_score:0);
            }
        }
        public bool LoadLocalRawPlayingData()
        {
            playingData = LocalProvider.Instance.GetPlayingDataByUserId(UserDataMart.Instance.m_userData.id);
            if (playingData != null)
            {
                return true;
            }
            return false;
        }
        public void LoadRawPlayingDataFromServer(Utils.Neuron.Input input)
        {
                HttpClient.Instance.GetPlayingDataFromServer((response) =>
                {
                    if (response.status.Equals("OK"))
                    {
                        playingData = response.data;
                        input.Signal();
                        //LocalProvider.Instance.SavePlayingData(playingData);
                    }
                    else
                    {
                        Debug.Log("Something when wrong with downloading playing data");
                    }
                }, UserDataMart.Instance.m_userData.id);
        }

        public void ParsePlayingData()
        {
            Debug.Log("ParsePlayingData");
            packDictionary.Clear();
            playingPacks.Clear();
            QuestionDataMart.Instance.packs.ForEach((p) =>
            {
                Pack pack = new Pack() { 
                    id = p.id ,
                    currentIndex = 0, 
                    playingQuestions = new List<QuestionPlayingData>(),
                    questionDictionary = new Dictionary<int, int>() 
                };

                p.questions.ForEach((question) => {
                    pack.playingQuestions.Add(new QuestionPlayingData() { question_id = question.id, pack_id = pack.id ,playing_data_id = playingData.id});
                    pack.questionDictionary.Add(question.id, pack.playingQuestions.Count - 1);
                });

                //if (p.question_type == 0)
                //    p.typed_questions.ForEach((question) => {
                //        pack.playingQuestions.Add(new QuestionPlayingData() { question_id = question.id, pack_id=pack.id });
                //        pack.questionDictionary.Add(question.id, pack.playingQuestions.Count - 1);
                //    });
                //else if (p.question_type == 1)
                //    p.mcq_questions.ForEach((question) => {
                //        pack.playingQuestions.Add(new QuestionPlayingData() { question_id = question.id, pack_id = pack.id });
                //        pack.questionDictionary.Add(question.id, pack.playingQuestions.Count - 1);
                //    });
                playingPacks.Add(pack);
                packDictionary.Add(pack.id, playingPacks.Count - 1);
            });
            /*Let's make it real*/
            //playingData.playing_packs.ForEach((playingPack) =>
            //{

            //});
            //Debug.Log("playingData.playing_questions.Count: " + playingData.playing_questions.Count);
            foreach(var p in playingData.playing_packs)
            {
                if (packDictionary.TryGetValue(p.id, out int value))
                {
                    Pack pack = playingPacks[value];

                    foreach (var questionPlayingData in p.playing_questions)
                    {
                        if (pack.questionDictionary.TryGetValue(questionPlayingData.question_id, out value))
                        {
                            // Key was in dictionary; "value" contains corresponding value
                            //Debug.Log("pack.playingQuestions[value] = questionPlayingData: " + questionPlayingData.pack_id + " " + questionPlayingData.question_id);
                            pack.playingQuestions[value] = questionPlayingData;
                        }
                        else
                        {
                            Debug.Log("Question with id " + questionPlayingData.question_id + " not existed in pack" + pack.id);
                            // Key wasn't in dictionary; "value" is now 0
                        }
                    }

                    pack.currentIndex = p.playing_questions.Count;
                }
                else
                {
                    Debug.Log("Pack with id " + p.id + " not existed");
                }
            }

            //for the first time
            for (int i = 0; i < playingPacks.Count; i++)
            {
                Pack pack = playingPacks[i];
                if (pack.playingQuestions.Count>0 && 
                    pack.currentIndex< pack.playingQuestions.Count &&
                    pack.playingQuestions[pack.currentIndex].status == 'l')
                {
                    pack.playingQuestions[pack.currentIndex].status = 'u';
                }
            }

            Debug.Log("Parsed playing data");
        }



        public void SubtractScore(int score)
        {
            if (playingData.total_score > 0)
            {
                playingData.total_score -= score;
                if (m_scoreChangedCallback != null) m_scoreChangedCallback(false);
            }
            else
            {
                playingData.total_score = 0;
            }
        }
        public void AddScore(int score)
        {
            playingData.total_score += score;
            if (m_scoreChangedCallback != null) m_scoreChangedCallback(true);
        }
        public void AddNewPlayingQuestion(QuestionPlayingData playingQuestion)
        {
            foreach (var p in playingData.playing_packs)
            {
                if(p.id == playingQuestion.pack_id)
                {
                    bool newPlayingQuestion = true;
                    for (int i = 0; i < p.playing_questions.Count; i++)
                    {
                        if (p.playing_questions[i].question_id == playingQuestion.question_id)
                        {
                            p.playing_questions[i] = playingQuestion;
                            newPlayingQuestion = false;
                            break;
                        }
                    }
                    if (newPlayingQuestion)
                        p.playing_questions.Add(playingQuestion);
                }
            }
            //        for (int i = 0; i < PlayingDataMart.Instance.playingData.playing_questions.Count; i++)
            //    if (PlayingDataMart.Instance.playingData.playing_questions[i].question_id == playingQuestion.question_id)
            //    {
            //        PlayingDataMart.Instance.playingData.playing_questions[i] = playingQuestion;
            //        newPlayingQuestion = false;
            //        break;
            //    }
            //if (newPlayingQuestion)
            //    PlayingDataMart.Instance.playingData.playing_questions.Add(playingQuestion);

        }
    }
}

//if (i< playingData.current_question_indices.Count)
//    pack.currentIndex = playingData.current_question_indices[i];
//else
//{
//    Debug.Log("playingData.current_question_indices.Add " + playingData.current_question_indices.Count +" "+ playingPacks.Count);

//    playingData.current_question_indices.Add(0);
//    pack.currentIndex = 0;
//}

//int currentQuestionIndex = pack.currentIndex;

//if (pack.playingQuestions.Count > 0)
//    if(currentQuestionIndex < pack.playingQuestions.Count)
//        if (pack.playingQuestions[currentQuestionIndex].status == 'l')
//            pack.playingQuestions[currentQuestionIndex].status = 'u';
