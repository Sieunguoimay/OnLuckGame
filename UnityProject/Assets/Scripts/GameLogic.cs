using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.DataMarts;
namespace Assets.Scripts
{
    public class GameLogic
    {
        /*This class is a Singleton*/
        private static GameLogic s_instance = null;
        public static GameLogic Instance
            {get{if (s_instance == null)s_instance = new GameLogic();return s_instance;}}
        private GameLogic() { }
        /*End of Singleton Declaration*/

        [Serializable]
        public class PlayingData
        {
            public int playing_data_id;
            public int total_score;
            public int pack_id;
            public int index;
            public List<PlayingDataMart.QuestionPlayingData> modified_questions;
        }

        public class HintManager
        {
            public int remainingScore;
            public int GetHintPrice(int hintCount)
            {
                return (hintCount + 1) * 10;
            }
            public void SubtractRemainingScoreOfQuestion(int score)
            {
                remainingScore -= score;
            }
        }
        public class TrackKeeper
        {
            public bool dirtyFlag { get; private set; }
            public void Reset()
            {
                dirtyFlag = false;
            }
            public void ActivateQuestionPlayingData(PlayingDataMart.QuestionPlayingData playingQuestion)
            {
                if (playingQuestion.status == 'u')
                {
                    playingQuestion.started = Utils.Instance.GetCurrentDateTime();// (new System.DateTime()).ToString();//fix this please...
                    playingQuestion.status = 'a';
                    playingQuestion.dirtyFlag = true;
                    dirtyFlag = true;
                    Debug.Log("playingQuestion.started " + playingQuestion.started);
                }
            }
            public void TerminateQuestionData(PlayingDataMart.QuestionPlayingData playingQuestion,char status, bool isTypingQuestion = false)
            {
                if (playingQuestion.status == 'a')
                {
                    playingQuestion.ended = Utils.Instance.GetCurrentDateTime(); //(new System.DateTime()).ToString();
                    playingQuestion.status = status;
                    playingQuestion.score = Instance.hintManager.remainingScore;
                    playingQuestion.dirtyFlag = true;
                    dirtyFlag = true;
                    Debug.Log("playingQuestion.ended " + playingQuestion.ended);

                    if(status == 'c'|| !isTypingQuestion)
                        HttpClient.Instance.SubmitQuestionPlayingData(playingQuestion, (response) =>
                        {
                            PlayingDataMart.Instance.playingData.uptodate_token = response.playing_data_uptodate_token;
                            PlayingDataMart.Instance.playingData.total_score = response.score;
                        });
                }
            }
            public bool UnlockQuestionData(PlayingDataMart.QuestionPlayingData playingQuestion)
            {
                if (playingQuestion.status == 'l')
                {
                    playingQuestion.status = 'u';
                    playingQuestion.dirtyFlag = true;
                    dirtyFlag = true;
                    return true;
                }
                return false;
            }
            public void ChangeUsedHintCount(PlayingDataMart.QuestionPlayingData playingQuestion, int newHintCount)
            {
                playingQuestion.used_hint_count = newHintCount;
                playingQuestion.dirtyFlag = true;
                dirtyFlag = true;
            }
        }
        public TrackKeeper trackKeeper = new TrackKeeper();
        public HintManager hintManager = new HintManager();


        public void OnPlayingDataOutputted(int packIndex, int questionIndex)
        {
            Debug.Log("Output pack_index="+ packIndex + "question_index=" + questionIndex);

            if (!trackKeeper.dirtyFlag) return;

            //playingPacks[pack].currentIndex = currentIndex;
            PlayingDataMart.Pack pack = PlayingDataMart.Instance.playingPacks[packIndex];
            pack.currentIndex = questionIndex;
            int currentScore = PlayingDataMart.Instance.playingData.total_score;

            PlayingData playingData = new PlayingData
            {
                playing_data_id = PlayingDataMart.Instance.playingData.id,
                total_score = currentScore,
                pack_id = pack.id,
                index = pack.currentIndex,
                modified_questions = new List<PlayingDataMart.QuestionPlayingData>(),
            };
            PlayingDataMart.Instance.playingData.total_score = currentScore;
            PlayingDataMart.Instance.playingData.current_question_indices[packIndex] = questionIndex;
            pack.playingQuestions.ForEach((playingQuestion) =>
            {
                if (playingQuestion.dirtyFlag)
                {
                    //send this one to server pls
                    //playingData.modified_questions.Add(playingQuestion);

                    bool newPlayingQuestion = true;
                    for(int i = 0; i< PlayingDataMart.Instance.playingData.playing_questions.Count; i++)
                        if(PlayingDataMart.Instance.playingData.playing_questions[i].question_id == playingQuestion.question_id)
                        {
                            PlayingDataMart.Instance.playingData.playing_questions[i] = playingQuestion;
                            newPlayingQuestion = false;
                            break;
                        }
                    if(newPlayingQuestion)
                        PlayingDataMart.Instance.playingData.playing_questions.Add(playingQuestion);

                }
            });
            Debug.Log("OnPlayingDataOutputted: Success");
            LocalProvider.Instance.SavePlayingData(PlayingDataMart.Instance.playingData);

            //HttpClient.Instance.SendQuestionPlayingDataToServer(playingData, (response) => {
            //    if (response.status.Equals("OK"))
            //    {
            //        Debug.Log("OnPlayingDataOutputted: Success");
            //        PlayingDataMart.Instance.playingData.uptodate_token = response.data;
            //        LocalProvider.Instance.SavePlayingData(PlayingDataMart.Instance.playingData);
            //    }
            //});
        }
    }
}
