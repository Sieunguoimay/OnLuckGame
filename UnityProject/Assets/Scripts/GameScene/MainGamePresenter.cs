using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.DataMarts;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameScene
{
    public class MainGamePresenter
    {


        public enum Modes{TYPING_ANSWER,MCQ}

        private static MainGamePresenter s_instance = null;
        public static MainGamePresenter Instance 
            { get { if (s_instance == null) s_instance = new MainGamePresenter(); return s_instance; } }

        public bool m_isPlayingInMCQNow { get; private set; }
        public Modes m_mode { private set; get; }

        private MainGame m_rMainGame = null;
        private QuestionDataMart.Pack pack = null;
        private PlayingDataMart.Pack playingPack = null;
        private int m_currentQuestionIndex = 0;
        private int m_recentlyUnlockedQuestionIndex;
        private int packIndex;

        public delegate void OutputDataOnExit(int packId, int recentlyUnlockedQuestionIndex);
        public OutputDataOnExit m_outputDataOnExit = null;
        public Utils.Neuron outputPlayingDataNeuron = new Utils.Neuron(2);

        private MainGamePresenter()
        {
            Debug.Log("MainGamePresenter created: stuff created once goes here");
            PlayingDataMart.Instance.m_scoreChangedCallback = OnScoreChanged;
            m_outputDataOnExit = GameLogic.Instance.OnPlayingDataOutputted;
            outputPlayingDataNeuron.output = () =>
            {
                m_outputDataOnExit(packIndex, m_recentlyUnlockedQuestionIndex);
                outputPlayingDataNeuron.inputs[0].Reset();
                outputPlayingDataNeuron.inputs[1].Reset();
            };

        }

        public void StartGame(QuestionDataMart.Pack pack,PlayingDataMart.Pack playingPack, int packIndex)
        {
            this.pack = pack;
            this.playingPack = playingPack;
            this.packIndex = packIndex;

            m_currentQuestionIndex = playingPack.currentIndex==playingPack.playingQuestions.Count? playingPack.currentIndex-1: playingPack.currentIndex;
            m_recentlyUnlockedQuestionIndex = playingPack.currentIndex;

            if (pack.question_type == 0) m_mode = Modes.TYPING_ANSWER;
            else if (pack.question_type == 1) m_mode = Modes.MCQ;

            Debug.Log("MainGamePresenter: Start the game");
            SceneManager.LoadScene("main_game");
        }


        public MainGamePresenter Ready(MainGame rMainGame)
        {
            m_rMainGame = rMainGame;
            if (UserDataMart.Instance.m_userData != null)
            {
                m_rMainGame.SetAvatar(UserDataMart.Instance.m_userData.texProfilePicture);
                m_rMainGame.SetUserName(UserDataMart.Instance.m_userData.name);
            }
            m_rMainGame.SetScore(PlayingDataMart.Instance.m_score);
            m_rMainGame.SetAnswerMode(m_mode);
            m_rMainGame.SetQuestionPackTitleText(pack.title);
            m_rMainGame.SetSeasonText(QuestionDataMart.Instance.season.name, m_recentlyUnlockedQuestionIndex, playingPack.playingQuestions.Count);

            outputPlayingDataNeuron.inputs[0].Reset();
            outputPlayingDataNeuron.inputs[1].Reset();

            GameLogic.Instance.trackKeeper.Reset();

            OpenQuestion(m_currentQuestionIndex);
            Debug.Log("MainGamePresenter: The Game Start from here");
            return this;
        }

        public void Terminate()
        {
            if (m_isPlayingInMCQNow) MCQForceEnding();
            Debug.Log("Escaped from the game");
            SceneManager.LoadScene("menu");
            outputPlayingDataNeuron.inputs[0].Signal();
        }
        public void OnQuit()
        {
            if (m_isPlayingInMCQNow) MCQForceEnding();
            outputPlayingDataNeuron.inputs[0].Reset();
            outputPlayingDataNeuron.inputs[1].Reset();
            outputPlayingDataNeuron.inputs[0].Signal();
            outputPlayingDataNeuron.inputs[1].Signal();
            Debug.Log("MainGamePresenter:OnQuit");
        }


        public void OpenQuestion(int index)
        {
            m_currentQuestionIndex = index;
            if(m_currentQuestionIndex< getQuestionPackCount())
            {
                QuestionDataMart.Question question = getQuestion(index);
                //Set all other info to the question panel here.
                m_rMainGame.HideHintPanel();
                m_rMainGame.SetQuestion(question.question);
                m_rMainGame.SetQuestionNumber(index + 1);
                m_rMainGame.SetQuestionImages(question.images);
                m_rMainGame.SetUiPrevButtonInteractable(m_currentQuestionIndex!=0);

                bool state = false;
                if (m_currentQuestionIndex + 1 < playingPack.playingQuestions.Count)
                    if (!IsQuestionLocked(m_currentQuestionIndex + 1))
                        state = true;
                //Debug.Log("SetUiNextButtonInteractable " + state);
                m_rMainGame.SetUiNextButtonInteractable(state);

                if (!IsQuestionLocked(m_currentQuestionIndex))
                {
                    GameLogic.Instance.trackKeeper.ActivateQuestionPlayingData(playingPack.playingQuestions[m_currentQuestionIndex]);
                    GameLogic.Instance.hintManager.remainingScore = question.score;
                    int usedHintCount = playingPack.playingQuestions[m_currentQuestionIndex].used_hint_count;

                    if (question.hints.Count > 0&& PlayingDataMart.Instance.m_score > 0)
                    {
                        m_rMainGame.SetHintUiInteractable(true);
                        if (IsQuestionActive(m_currentQuestionIndex)&& usedHintCount < question.hints.Count)
                        {
                            m_rMainGame.SetHintPriceActive(true);
                            m_rMainGame.SetHintPrice(GameLogic.Instance.hintManager.GetHintPrice(usedHintCount));
                        }
                        else
                        {
                            m_rMainGame.SetHintPriceActive(false);
                        }
                        Debug.Log("OpenQuestion: Enough money");
                    }
                    else
                    {
                        Debug.Log("OpenQuestion: Not enough money: your money = " + PlayingDataMart.Instance.m_score+", hints count = "+ question.hints.Count);
                        if(!IsQuestionActive(m_currentQuestionIndex))
                            m_rMainGame.SetHintUiInteractable(true);
                        else
                        {
                            if(usedHintCount>0)
                                m_rMainGame.SetHintUiInteractable(true);
                            else
                                m_rMainGame.SetHintUiInteractable(false);
                        }
                    }
                }
                else
                {
                    Debug.Log("OpenQuestion: This question is locked ");
                }

                if (m_mode == Modes.TYPING_ANSWER)
                {
                    m_rMainGame.ClearAnswerInputField();
                    if (IsQuestionActive(m_currentQuestionIndex))
                    {
                        m_rMainGame.SetUiInteractableInTypingMode(true);
                        m_rMainGame.SetPrevAnswer("");
                    }
                    else
                    {
                        m_rMainGame.SetUiInteractableInTypingMode(false);
                        m_rMainGame.SetPrevAnswer(pack.typed_questions[m_currentQuestionIndex].answer);
                    }
                }
                else if (m_mode == Modes.MCQ)
                {
                    m_rMainGame.SetMCQChoices(pack.mcq_questions[m_currentQuestionIndex].choices);
                    m_rMainGame.ClearMCQChoicesColor();

                    if(IsQuestionActive(m_currentQuestionIndex))
                    {
                        m_rMainGame.StartTimer(pack.mcq_questions[m_currentQuestionIndex].time);
                        m_isPlayingInMCQNow = true;
                    }
                    else
                    {
                        int correctAnswer = pack.mcq_questions[m_currentQuestionIndex].answer;
                        m_rMainGame.ShowMCQAnswer(-1, correctAnswer);
                        m_rMainGame.ResetTimer(0);
                        m_isPlayingInMCQNow = false;
                    }
                }
            }
        }

        public void NextQuestion()
        {
            if (m_currentQuestionIndex < getQuestionPackCount()-1
                && !IsQuestionLocked(m_currentQuestionIndex + 1))
            {
                if (m_isPlayingInMCQNow)
                {
                    m_rMainGame.AskForLeavingTimingQuestion((success) =>
                    {
                        if (success)
                        {
                            MCQForceEnding();
                            OpenQuestion(m_currentQuestionIndex + 1);
                        }
                    });
                }
                else
                {
                    OpenQuestion(m_currentQuestionIndex + 1);
                }
            }
        }
        public void PrevQuestion()
        {
            if (m_currentQuestionIndex > 0)
            {
                if (m_isPlayingInMCQNow)
                {
                    m_rMainGame.AskForLeavingTimingQuestion((success) =>
                    {
                        if (success)
                        {
                            MCQForceEnding();
                            OpenQuestion(m_currentQuestionIndex - 1);
                        }
                    });
                }
                else
                {
                    OpenQuestion(m_currentQuestionIndex - 1);
                }
            }
        }

        public void VerifyTypedAnswer(string answer)
        {
            answer = Utils.Instance.convertToUnSign3(answer).ToLower();
            if (Utils.Instance.convertToUnSign3(pack.typed_questions[m_currentQuestionIndex].answer.ToLower()).Equals(answer))
            {
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[m_currentQuestionIndex], 'c');
                bool unlockStatus = UnlockQuestion(m_currentQuestionIndex + 1);

                if (m_currentQuestionIndex == playingPack.playingQuestions.Count-1) 
                    m_rMainGame.ShowCongratePopup(pack.title, PlayingDataMart.Instance.playingData.total_score);
                else 
                    m_rMainGame.ShowCorrectAnswer(AssetsDataMart.Instance.assetsData.strings.next_question,PlayingDataMart.Instance.playingData.total_score,GameLogic.Instance.hintManager.remainingScore);

                m_rMainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                    unlockStatus ? m_recentlyUnlockedQuestionIndex : m_recentlyUnlockedQuestionIndex + 1, 
                    playingPack.playingQuestions.Count);
                m_rMainGame.SetUiInteractableInTypingMode(false);
                m_rMainGame.SetHintPriceActive(false);
                //m_rMainGame.SetHintUiInteractable(false);
                m_rMainGame.SetPrevAnswer(answer);
                m_rMainGame.SetUiNextButtonInteractable(false);
                if(m_currentQuestionIndex + 1 < playingPack.playingQuestions.Count)
                    if(!IsQuestionLocked(m_currentQuestionIndex + 1))
                        m_rMainGame.SetUiNextButtonInteractable(true);

                PlayingDataMart.Instance.AddScore(pack.typed_questions[m_currentQuestionIndex].score);
                //playingPack.playingQuestions[m_currentQuestionIndex].ended = (new System.DateTime()).ToString();

                Debug.Log("Dung roi ban ey");
                m_rMainGame.PlayAudio(AssetsDataMart.Instance.correctAudioClip);
            }
            else
            {
                m_rMainGame.ShowWrongAnswer(
                    AssetsDataMart.Instance.assetsData.strings.try_again,
                    AssetsDataMart.Instance.assetsData.strings.incorrect, 
                    GameLogic.Instance.hintManager.remainingScore);
                Debug.Log("Sai roi ban ey");
                m_rMainGame.PlayAudio(AssetsDataMart.Instance.wrongAudioClip);
            }
        }
        public void VerifyMCQAnswer(int answer,bool isForcingEnding = false)
        {
            m_isPlayingInMCQNow = false;
            QuestionDataMart.MCQQuestion question = pack.mcq_questions[m_currentQuestionIndex];
            int time = m_rMainGame.StopTimer();
            int thinkingTime = question.time - time;
            int correctAnswer = question.answer;

            bool unlockStatus = UnlockQuestion(m_currentQuestionIndex + 1);


            m_rMainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                unlockStatus ? m_recentlyUnlockedQuestionIndex : m_recentlyUnlockedQuestionIndex + 1,
                playingPack.playingQuestions.Count);

            m_rMainGame.ShowMCQAnswer(answer, correctAnswer);
            //m_rMainGame.SetHintUiInteractable(false);
            m_rMainGame.SetHintPriceActive(false);
            m_rMainGame.SetUiNextButtonInteractable(m_currentQuestionIndex + 1 < playingPack.playingQuestions.Count &&
                !IsQuestionLocked(m_currentQuestionIndex+1));


            if (correctAnswer == answer)
            {
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[m_currentQuestionIndex], 'c');
                //playingPack.playingQuestions[m_currentQuestionIndex].status = 'c';
                m_rMainGame.ShowCorrectAnswer((m_currentQuestionIndex == playingPack.playingQuestions.Count - 1)?"   OK   ":
                        AssetsDataMart.Instance.assetsData.strings.next_question,
                    PlayingDataMart.Instance.playingData.total_score, 
                    GameLogic.Instance.hintManager.remainingScore);

                PlayingDataMart.Instance.AddScore(pack.mcq_questions[m_currentQuestionIndex].score);

                Debug.Log("Dung roi ban ey : "+ thinkingTime+"s");
                m_rMainGame.PlayAudio(AssetsDataMart.Instance.correctAudioClip);
            }
            else
            {
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[m_currentQuestionIndex], 'w');
                //playingPack.playingQuestions[m_currentQuestionIndex].status = 'w';
                if (!isForcingEnding) {
                    m_rMainGame.ShowWrongAnswer(
                        (m_currentQuestionIndex == playingPack.playingQuestions.Count - 1) ? "   OK   " : 
                        AssetsDataMart.Instance.assetsData.strings.next_question,
                        AssetsDataMart.Instance.assetsData.strings.incorrect+"\n"+
                        AssetsDataMart.Instance.assetsData.strings.the_answer_is+" " +(char)(65 + correctAnswer),
                        PlayingDataMart.Instance.playingData.total_score
                        );
                }
                Debug.Log("Sai roi ban ey: " + thinkingTime + "s");
                m_rMainGame.PlayAudio(AssetsDataMart.Instance.wrongAudioClip);
            }
        }
        public void MCQTimeout()
        {
            m_isPlayingInMCQNow = false;

            bool unlockStatus = UnlockQuestion(m_currentQuestionIndex + 1);
            int correctAnswer = pack.mcq_questions[m_currentQuestionIndex].answer;
            m_rMainGame.ShowMCQAnswer(-1,correctAnswer);
            m_rMainGame.SetHintPriceActive(false);
            m_rMainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                unlockStatus ? m_recentlyUnlockedQuestionIndex : m_recentlyUnlockedQuestionIndex + 1,
                playingPack.playingQuestions.Count);

            GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[m_currentQuestionIndex], 'w');

            m_rMainGame.ShowWrongAnswer(
                (m_currentQuestionIndex == playingPack.playingQuestions.Count - 1) ? "   OK   " : 
                        AssetsDataMart.Instance.assetsData.strings.next_question,
                        AssetsDataMart.Instance.assetsData.strings.timeout + "\n"+
                        AssetsDataMart.Instance.assetsData.strings.the_answer_is+" " +(char)(65 + correctAnswer),
                       PlayingDataMart.Instance.playingData.total_score
                );

            Debug.Log("Sai roi ban ey");
                m_rMainGame.PlayAudio(AssetsDataMart.Instance.wrongAudioClip);
        }
        public void NextQuestionAfterAnswering()
        {
            m_rMainGame.HideAnswerResultPopupPanel();
            if (m_mode == Modes.TYPING_ANSWER)
            {
                if (IsQuestionAnsweredCorrectly(m_currentQuestionIndex))
                {
                    m_rMainGame.SetPrevAnswer(pack.typed_questions[m_currentQuestionIndex].answer);
                    NextQuestion();
                }
            }
            else if (m_mode == Modes.MCQ)
            {
                if (IsQuestionAnsweredCorrectly(m_currentQuestionIndex) ||
                    IsQuestionAnsweredWrongly(m_currentQuestionIndex))
                    NextQuestion();
            }
            Debug.Log("Next it babe");
        }
        public void BuyHint()
        {
            QuestionDataMart.Question question = getQuestion(m_currentQuestionIndex);
            PlayingDataMart.QuestionPlayingData playingQuestion = playingPack.playingQuestions[m_currentQuestionIndex];
            if (IsQuestionActive(m_currentQuestionIndex))
            {
                if (playingQuestion.used_hint_count < question.hints.Count && PlayingDataMart.Instance.m_score > 0)
                {
                    int subtractingScore = GameLogic.Instance.hintManager.GetHintPrice(playingQuestion.used_hint_count);
                    int nextScore = GameLogic.Instance.hintManager.GetHintPrice(playingQuestion.used_hint_count + 1);

                    PlayingDataMart.Instance.SubtractScore(subtractingScore);
                    GameLogic.Instance.hintManager.SubtractRemainingScoreOfQuestion(subtractingScore);
                    GameLogic.Instance.trackKeeper.ChangeUsedHintCount(playingQuestion, playingQuestion.used_hint_count + 1);
                    //playingQuestion.used_hint_count++;
                    if (playingQuestion.used_hint_count < question.hints.Count)
                        m_rMainGame.SetHintPrice(nextScore);
                    else
                        m_rMainGame.SetHintPriceActive(false);
                    Debug.Log("Hint count " + playingQuestion.used_hint_count +" "+ question.hints.Count);
                }
                else
                {
                    //no hints left or not enough money.
                }
                string hints = "";
                for (int i = 0; i < playingQuestion.used_hint_count; i++)
                    hints += (i+1) + ". " + question.hints[i] + (i < playingQuestion.used_hint_count - 1 ? "\n" : "");
                m_rMainGame.ShowHintPanel(hints);
            }
            else
            {
                string hints = "";
                for(int i = 0; i< question.hints.Count;i++)
                    hints += i + ". " + question.hints[i] + (i < question.hints.Count-1 ? "\n" : "");
                m_rMainGame.ShowHintPanel(hints);
            }
        }
        public bool UnlockQuestion(int unlockIndex, int howMany = 1)
        {
            if (unlockIndex > m_recentlyUnlockedQuestionIndex)
            {
                if(unlockIndex < playingPack.playingQuestions.Count)
                {
                    m_recentlyUnlockedQuestionIndex = unlockIndex;
                    for (int i = 0; i < howMany; i++)
                    {
                        if (i + unlockIndex < playingPack.playingQuestions.Count)
                        {
                            GameLogic.Instance.trackKeeper.UnlockQuestionData(playingPack.playingQuestions[unlockIndex + i]);//.status = 'u';
                            Debug.Log("Unlocked question " + (unlockIndex + i));
                        }
                    }
                    return true;
                }
                else if(unlockIndex == playingPack.playingQuestions.Count) {
                    //this pack is finished
                    m_recentlyUnlockedQuestionIndex = playingPack.playingQuestions.Count;
                }
            }
            return false;
        }
        public void OnScoreChanged(bool increased)
        {
            m_rMainGame.SetScore(PlayingDataMart.Instance.m_score);
            if (increased)
            {
                //display some cool effect here
            }
            else
            {
                //display some sad effect here..
            }
        }

        public void MCQForceEnding()
        {
            VerifyMCQAnswer(-1,true);
        }

        public void SaveImageOfTheCurrentQuestion(int index)
        {
            QuestionDataMart.Question question = getQuestion(m_currentQuestionIndex);
            LocalProvider.Instance.SaveImage(question.images[index].sprite.texture,
                Path.GetFileName(question.images[0].path), (status) => {
                if (status)
                {
                    Debug.Log("saved image bro...");
                }
            });
        }
        public bool IsLastQuestion()
        {
            return (m_currentQuestionIndex == playingPack.playingQuestions.Count - 1);
        }
        public bool IsInMCQMode()
        {
            return m_mode == Modes.MCQ;
        }
        public string GetCurrentQuestionPackName()
        {
            return pack.title;
        }

        private QuestionDataMart.Question getQuestion(int index)
        {
            if (m_mode == Modes.TYPING_ANSWER)
                return pack.typed_questions[index];
            else if (m_mode == Modes.MCQ)
                return pack.mcq_questions[index];
            return null;
        }
        private int getQuestionPackCount()
        {
            if (m_mode == Modes.TYPING_ANSWER)
                return pack.typed_questions.Count;
            else if (m_mode == Modes.MCQ)
                return pack.mcq_questions.Count;
            return 0;
        }



        public bool IsQuestionLocked(int index)
        {
            return (playingPack.playingQuestions[index].status == 'l');
        }
        public bool IsQuestionUnlocked(int index)
        {
            return (playingPack.playingQuestions[index].status == 'u');
        }
        public bool IsQuestionActive(int index)
        {
            return (playingPack.playingQuestions[index].status == 'a');
        }
        public bool IsQuestionAnsweredCorrectly(int index)
        {
            return (playingPack.playingQuestions[index].status == 'c');
        }
        public bool IsQuestionAnsweredWrongly(int index)
        {
            return (playingPack.playingQuestions[index].status == 'w');
        }
        public bool CheckCurrentQuestionIndex(int index)
        {
            return m_currentQuestionIndex == index;
        }

        public void LoadQuestionListIntoUiList(ScrollList uiList)
        {
            uiList.CreateList<QuestionItem>(playingPack.playingQuestions.Count, (item, index) =>
            {
                item.SetIndex(index);
                item.SetState(playingPack.playingQuestions[index].status);
            });
        }
    }
}