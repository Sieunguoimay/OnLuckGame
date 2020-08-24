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


        //Raw Playing data from server
        //it points to the user_playing_slots table by id with many-to-one relationships
        //it also points to the questions table by id with many-to-one relationships
        //it is the link between the user playing slot and the question of a given user

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


            public List<QuestionPlayingData> playing_questions;//get from active_questions table
            public List<ActiveTime> active_times;
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


        public class Pack
        {
            public int currentIndex;
            public int id;
            public List<QuestionPlayingData> playingQuestions;
            public Dictionary<int, int> questionDictionary = new Dictionary<int, int>();
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
                        LocalProvider.Instance.SavePlayingData(playingData);
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
                Pack pack = new Pack() { id = p.id };
                pack.playingQuestions = new List<QuestionPlayingData>();
                pack.questionDictionary.Clear();
                if (p.question_type == 0)
                    p.typed_questions.ForEach((question) => {
                        pack.playingQuestions.Add(new QuestionPlayingData() { question_id = question.id, pack_id=pack.id });
                        pack.questionDictionary.Add(question.id, pack.playingQuestions.Count - 1);
                    });
                else if (p.question_type == 1)
                    p.mcq_questions.ForEach((question) => {
                        pack.playingQuestions.Add(new QuestionPlayingData() { question_id = question.id, pack_id = pack.id });
                        pack.questionDictionary.Add(question.id, pack.playingQuestions.Count - 1);
                    });
                playingPacks.Add(pack);
                packDictionary.Add(pack.id, playingPacks.Count - 1);
            });
            /*Let's make it real*/
            Debug.Log("playingData.playing_questions.Count: " + playingData.playing_questions.Count);
            playingData.playing_questions.ForEach((questionPlayingData) =>
            {
                int value;
                if (packDictionary.TryGetValue(questionPlayingData.pack_id, out value))
                {
                    Pack pack = playingPacks[value];
                    if (pack.questionDictionary.TryGetValue(questionPlayingData.question_id, out value))
                    {
                        // Key was in dictionary; "value" contains corresponding value
                        Debug.Log("pack.playingQuestions[value] = questionPlayingData: " + questionPlayingData.pack_id + " " + questionPlayingData.question_id);
                        pack.playingQuestions[value] = questionPlayingData;
                    }
                    else
                    {
                        Debug.Log("Question with id " + questionPlayingData.question_id + " not existed in pack" + pack.id);
                        // Key wasn't in dictionary; "value" is now 0
                    }
                }
                else
                {
                    Debug.Log("Pack with id " + questionPlayingData.pack_id + " not existed");
                }
            });

            //for the first time
            for (int i = 0; i < playingPacks.Count; i++)
            {
                Pack pack = playingPacks[i];
                if (i< playingData.current_question_indices.Count)
                    pack.currentIndex = playingData.current_question_indices[i];
                else
                {
                    Debug.Log("playingData.current_question_indices.Add " + playingData.current_question_indices.Count +" "+ playingPacks.Count);

                    playingData.current_question_indices.Add(0);
                    pack.currentIndex = 0;
                }

                int currentQuestionIndex = pack.currentIndex;

                if (pack.playingQuestions.Count > 0)
                    if(currentQuestionIndex < pack.playingQuestions.Count)
                        if (pack.playingQuestions[currentQuestionIndex].status == 'l')
                            pack.playingQuestions[currentQuestionIndex].status = 'u';

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
    }
}
