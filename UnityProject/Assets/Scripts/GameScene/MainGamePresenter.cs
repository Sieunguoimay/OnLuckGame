using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.DataMarts;
using UnityEngine.SceneManagement;
using System;

namespace Assets.Scripts.GameScene
{
    public class MainGamePresenter
    {


        public enum Modes{TYPING_ANSWER,MCQ}

        private static MainGamePresenter s_instance = null;
        public static MainGamePresenter Instance { get { if (s_instance == null) s_instance = new MainGamePresenter(); return s_instance; } }

        public bool IsPlayingInMCQNow { get; private set; }
        public Modes Mode { private set; get; }

        private MainGame mainGame = null;
        private QuestionDataMart.Pack pack = null;
        private PlayingDataMart.Pack playingPack = null;
        private int currentQuestionIndex = 0;
        private int recentlyUnlockedQuestionIndex;
        private int packIndex;
        private QuestionDataMart.TypedQuestion downloadedTypedQuestion;
        private QuestionDataMart.MCQQuestion downloadedMCQQuestion;

        public Action<int,int> outputDataOnExit = null;
        public Utils.Neuron outputPlayingDataNeuron = new Utils.Neuron(2);

        private MainGamePresenter()
        {
            Debug.Log("MainGamePresenter created: stuff created once goes here");

            PlayingDataMart.Instance.m_scoreChangedCallback = OnScoreChanged;

            outputDataOnExit = GameLogic.Instance.OnPlayingDataOutputted;

            outputPlayingDataNeuron.output = () =>
            {
                outputDataOnExit(packIndex, recentlyUnlockedQuestionIndex);

                outputPlayingDataNeuron.inputs[0].Reset();
                outputPlayingDataNeuron.inputs[1].Reset();
            };

        }

        public void StartGame(QuestionDataMart.Pack pack,PlayingDataMart.Pack playingPack, int packIndex)
        {
            this.pack = pack;
            this.playingPack = playingPack;
            this.packIndex = packIndex;

            currentQuestionIndex = playingPack.currentIndex==playingPack.playingQuestions.Count? playingPack.currentIndex-1: playingPack.currentIndex;
            recentlyUnlockedQuestionIndex = playingPack.currentIndex;

            if (pack.question_type == 0) Mode = Modes.TYPING_ANSWER;
            else if (pack.question_type == 1) Mode = Modes.MCQ;

            Debug.Log("MainGamePresenter: Start the game");
            SceneManager.LoadScene("main_game");
        }


        public MainGamePresenter Ready(MainGame rMainGame)
        {
            mainGame = rMainGame;
            if (UserDataMart.Instance.m_userData != null)
            {
                mainGame.SetAvatar(UserDataMart.Instance.m_userData.texProfilePicture);
                mainGame.SetUserName(UserDataMart.Instance.m_userData.name);
            }
            mainGame.SetScore(PlayingDataMart.Instance.m_score);
            mainGame.SetAnswerMode(Mode);
            mainGame.SetQuestionPackTitleText(pack.title);
            mainGame.SetSeasonText(QuestionDataMart.Instance.season.name, recentlyUnlockedQuestionIndex, playingPack.playingQuestions.Count);

            outputPlayingDataNeuron.inputs[0].Reset();
            outputPlayingDataNeuron.inputs[1].Reset();

            GameLogic.Instance.trackKeeper.Reset();

            OpenQuestion(currentQuestionIndex);
            Debug.Log("MainGamePresenter: The Game Start from here");
            return this;
        }

        public void Terminate()
        {
            if (IsPlayingInMCQNow) MCQForceEnding();
            Debug.Log("Escaped from the game");
            SceneManager.LoadScene("menu");
            outputPlayingDataNeuron.inputs[0].Signal();
        }
        public void OnQuit()
        {
            if (IsPlayingInMCQNow) MCQForceEnding();
            outputPlayingDataNeuron.inputs[0].Reset();
            outputPlayingDataNeuron.inputs[1].Reset();
            outputPlayingDataNeuron.inputs[0].Signal();
            outputPlayingDataNeuron.inputs[1].Signal();
            Debug.Log("MainGamePresenter:OnQuit");
        }


        public void OpenQuestion(int index)
        {
            if (index >= pack.questions.Count)
            {
                return;
            }
            HttpClient.Instance.GetQuestionById(pack.questions[index].id, pack.question_type, (questionWrapper) =>
            {
                if (questionWrapper != null)
                {
                    if(questionWrapper.type == 0)
                    {
                        HttpClient.TypedQuestion q = questionWrapper.typed_question;
                        downloadedTypedQuestion = new QuestionDataMart.TypedQuestion()
                        {
                            id = q.id,
                            question = q.question,
                            answer = q.answer,
                            score = q.score,
                            images = new List<QuestionDataMart.Image>()
                        };

                        List<string> images = Utils.Instance.FromJsonList<List<string>>(q.images);
                        foreach (string img in images)
                        {
                            QuestionDataMart.Image image = new QuestionDataMart.Image() { path = img };
                            downloadedTypedQuestion.images.Add(image);
                            Utils.Instance.LoadImage(HttpClient.Instance.BaseUrl + img, (texture) =>
                            {
                                Debug.Log("Downloaded " + img);
                                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                                mainGame.SetQuestionImages(downloadedTypedQuestion.images);
                            });
                        }
                        downloadedTypedQuestion.hints = new List<string>();
                        List<string> hints = Utils.Instance.FromJsonList<List<string>>(q.hints);
                        foreach (string hint in hints)
                            downloadedTypedQuestion.hints.Add(hint);

                    }
                    else
                    {
                        HttpClient.McqQuestion q = questionWrapper.mcq_question;
                        downloadedMCQQuestion = new QuestionDataMart.MCQQuestion()
                        {
                            id = q.id,
                            question = q.question,
                            choices = Utils.Instance.FromJsonList<List<string>>(q.choices).ToArray(),
                            answer = q.answer,
                            time = q.time,
                            score = q.score,
                            images = new List<QuestionDataMart.Image>()
                        };

                        List<string> images = Utils.Instance.FromJsonList<List<string>>(q.images);
                        foreach (string img in images)
                        {
                            QuestionDataMart.Image image = new QuestionDataMart.Image() { path = img };
                            downloadedMCQQuestion.images.Add(image);
                            Utils.Instance.LoadImage(HttpClient.Instance.BaseUrl + img, (texture) =>
                            {
                                Debug.Log("Downloaded " + img);
                                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                                mainGame.SetQuestionImages(downloadedMCQQuestion.images);
                            });
                        }
                        downloadedMCQQuestion.hints = new List<string>();
                        List<string> hints = Utils.Instance.FromJsonList<List<string>>(q.hints);
                        foreach (string hint in hints)
                            downloadedMCQQuestion.hints.Add(hint);
                    }
                    openQuestion(index);
                }
                else
                {

                }
            });
        }

        private void openQuestion(int index)
        {
            currentQuestionIndex = index;
            if (currentQuestionIndex < getQuestionPackCount())
            {
                QuestionDataMart.Question question = getQuestion(index);
                //Set all other info to the question panel here.
                mainGame.HideHintPanel();
                mainGame.SetQuestion(question.question);
                mainGame.SetQuestionNumber(index + 1);
                mainGame.SetQuestionImages(question.images);
                mainGame.SetUiPrevButtonInteractable(currentQuestionIndex != 0);

                bool state = false;
                if (currentQuestionIndex + 1 < playingPack.playingQuestions.Count)
                    if (!IsQuestionLocked(currentQuestionIndex + 1))
                        state = true;
                //Debug.Log("SetUiNextButtonInteractable " + state);
                mainGame.SetUiNextButtonInteractable(state);

                if (!IsQuestionLocked(currentQuestionIndex))
                {
                    GameLogic.Instance.trackKeeper.ActivateQuestionPlayingData(playingPack.playingQuestions[currentQuestionIndex]);
                    GameLogic.Instance.hintManager.remainingScore = question.score;
                    int usedHintCount = playingPack.playingQuestions[currentQuestionIndex].used_hint_count;

                    if (question.hints.Count > 0 && PlayingDataMart.Instance.m_score > 0)
                    {
                        mainGame.SetHintUiInteractable(true);
                        if (IsQuestionActive(currentQuestionIndex) && usedHintCount < question.hints.Count)
                        {
                            mainGame.SetHintPriceActive(true);
                            mainGame.SetHintPrice(GameLogic.Instance.hintManager.GetHintPrice(usedHintCount));
                        }
                        else
                        {
                            mainGame.SetHintPriceActive(false);
                        }
                        Debug.Log("OpenQuestion: Enough money");
                    }
                    else
                    {
                        Debug.Log("OpenQuestion: Not enough money: your money = " + PlayingDataMart.Instance.m_score + ", hints count = " + question.hints.Count);
                        if (!IsQuestionActive(currentQuestionIndex))
                            mainGame.SetHintUiInteractable(true);
                        else
                        {
                            if (usedHintCount > 0)
                                mainGame.SetHintUiInteractable(true);
                            else
                                mainGame.SetHintUiInteractable(false);
                        }
                    }
                }
                else
                {
                    Debug.Log("OpenQuestion: This question is locked ");
                }

                if (Mode == Modes.TYPING_ANSWER)
                {
                    mainGame.ClearAnswerInputField();
                    if (IsQuestionActive(currentQuestionIndex))
                    {
                        mainGame.SetUiInteractableInTypingMode(true);
                        mainGame.SetPrevAnswer("");
                    }
                    else
                    {
                        mainGame.SetUiInteractableInTypingMode(false);
                        //m_rMainGame.SetPrevAnswer(pack.typed_questions[m_currentQuestionIndex].answer);
                        mainGame.SetPrevAnswer(getTypedQuestion(currentQuestionIndex).answer);
                    }
                }
                else if (Mode == Modes.MCQ)
                {
                    var mcqQuestion = getMCQQuestion(currentQuestionIndex);
                    mainGame.SetMCQChoices(mcqQuestion.choices);
                    mainGame.ClearMCQChoicesColor();

                    if (IsQuestionActive(currentQuestionIndex))
                    {
                        mainGame.StartTimer(mcqQuestion.time);
                        IsPlayingInMCQNow = true;
                    }
                    else
                    {
                        int correctAnswer = mcqQuestion.answer;
                        mainGame.ShowMCQAnswer(-1, correctAnswer);
                        mainGame.ResetTimer(0);
                        IsPlayingInMCQNow = false;
                    }
                }
            }

        }

        public void NextQuestion()
        {
            if (currentQuestionIndex < getQuestionPackCount()-1
                && !IsQuestionLocked(currentQuestionIndex + 1))
            {
                if (IsPlayingInMCQNow)
                {
                    mainGame.AskForLeavingTimingQuestion((success) =>
                    {
                        if (success)
                        {
                            MCQForceEnding();
                            OpenQuestion(currentQuestionIndex + 1);
                        }
                    });
                }
                else
                {
                    OpenQuestion(currentQuestionIndex + 1);
                }
            }
        }
        public void PrevQuestion()
        {
            if (currentQuestionIndex > 0)
            {
                if (IsPlayingInMCQNow)
                {
                    mainGame.AskForLeavingTimingQuestion((success) =>
                    {
                        if (success)
                        {
                            MCQForceEnding();
                            OpenQuestion(currentQuestionIndex - 1);
                        }
                    });
                }
                else
                {
                    OpenQuestion(currentQuestionIndex - 1);
                }
            }
        }

        public void VerifyTypedAnswer(string answer)
        {
            answer = Utils.Instance.convertToUnSign3(answer).ToLower();
            var typedQuestion = getTypedQuestion(currentQuestionIndex);
            if (Utils.Instance.convertToUnSign3(typedQuestion.answer.ToLower()).Equals(answer))
            {
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], 'c');
                bool unlockStatus = UnlockQuestion(currentQuestionIndex + 1);

                if (currentQuestionIndex == playingPack.playingQuestions.Count-1) 
                    mainGame.ShowCongratePopup(pack.title, PlayingDataMart.Instance.playingData.total_score);
                else 
                    mainGame.ShowCorrectAnswer(AssetsDataMart.Instance.constantsSO.stringsSO.next_question,PlayingDataMart.Instance.playingData.total_score,GameLogic.Instance.hintManager.remainingScore);

                mainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                    unlockStatus ? recentlyUnlockedQuestionIndex : recentlyUnlockedQuestionIndex + 1, 
                    playingPack.playingQuestions.Count);
                mainGame.SetUiInteractableInTypingMode(false);
                mainGame.SetHintPriceActive(false);
                //m_rMainGame.SetHintUiInteractable(false);
                mainGame.SetPrevAnswer(answer);
                mainGame.SetUiNextButtonInteractable(false);
                if(currentQuestionIndex + 1 < playingPack.playingQuestions.Count)
                    if(!IsQuestionLocked(currentQuestionIndex + 1))
                        mainGame.SetUiNextButtonInteractable(true);

                PlayingDataMart.Instance.AddScore(typedQuestion.score);
                //playingPack.playingQuestions[m_currentQuestionIndex].ended = (new System.DateTime()).ToString();

                Debug.Log("Dung roi ban ey");
                mainGame.PlayAudio(AssetsDataMart.Instance.constantsSO.correctAudioClip);

                //HttpClient.Instance.NotifyServerOnQuestionAnswered()
            }
            else
            {
                mainGame.ShowWrongAnswer(
                    AssetsDataMart.Instance.constantsSO.stringsSO.try_again,
                    AssetsDataMart.Instance.constantsSO.stringsSO.incorrect, 
                    GameLogic.Instance.hintManager.remainingScore);
                Debug.Log("Sai roi ban ey");
                mainGame.PlayAudio(AssetsDataMart.Instance.constantsSO.wrongAudioClip);
            }
        }
        public void VerifyMCQAnswer(int answer,bool isForcingEnding = false)
        {
            IsPlayingInMCQNow = false;
            QuestionDataMart.MCQQuestion question = getMCQQuestion(currentQuestionIndex);
            int time = mainGame.StopTimer();
            int thinkingTime = question.time - time;
            int correctAnswer = question.answer;

            bool unlockStatus = UnlockQuestion(currentQuestionIndex + 1);


            mainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                unlockStatus ? recentlyUnlockedQuestionIndex : recentlyUnlockedQuestionIndex + 1,
                playingPack.playingQuestions.Count);

            mainGame.ShowMCQAnswer(answer, correctAnswer);
            //m_rMainGame.SetHintUiInteractable(false);
            mainGame.SetHintPriceActive(false);
            mainGame.SetUiNextButtonInteractable(currentQuestionIndex + 1 < playingPack.playingQuestions.Count &&
                !IsQuestionLocked(currentQuestionIndex+1));


            if (correctAnswer == answer)
            {
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], 'c');
                //playingPack.playingQuestions[m_currentQuestionIndex].status = 'c';
                mainGame.ShowCorrectAnswer((currentQuestionIndex == playingPack.playingQuestions.Count - 1)?"   OK   ":
                        AssetsDataMart.Instance.constantsSO.stringsSO.next_question,
                    PlayingDataMart.Instance.playingData.total_score, 
                    GameLogic.Instance.hintManager.remainingScore);

                PlayingDataMart.Instance.AddScore(getMCQQuestion(currentQuestionIndex).score);

                Debug.Log("Dung roi ban ey : "+ thinkingTime+"s");
                mainGame.PlayAudio(AssetsDataMart.Instance.constantsSO.correctAudioClip);
            }
            else
            {
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], 'w');
                //playingPack.playingQuestions[m_currentQuestionIndex].status = 'w';
                if (!isForcingEnding) {
                    mainGame.ShowWrongAnswer(
                        (currentQuestionIndex == playingPack.playingQuestions.Count - 1) ? "   OK   " : 
                        AssetsDataMart.Instance.constantsSO.stringsSO.next_question,
                        AssetsDataMart.Instance.constantsSO.stringsSO.incorrect+"\n"+
                        AssetsDataMart.Instance.constantsSO.stringsSO.the_answer_is+" " +(char)(65 + correctAnswer),
                        PlayingDataMart.Instance.playingData.total_score
                        );
                }
                Debug.Log("Sai roi ban ey: " + thinkingTime + "s");
                mainGame.PlayAudio(AssetsDataMart.Instance.constantsSO.wrongAudioClip);
            }
        }
        public void MCQTimeout()
        {
            IsPlayingInMCQNow = false;

            bool unlockStatus = UnlockQuestion(currentQuestionIndex + 1);
            int correctAnswer = getMCQQuestion(currentQuestionIndex).answer;
            mainGame.ShowMCQAnswer(-1,correctAnswer);
            mainGame.SetHintPriceActive(false);
            mainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                unlockStatus ? recentlyUnlockedQuestionIndex : recentlyUnlockedQuestionIndex + 1,
                playingPack.playingQuestions.Count);

            GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], 'w');

            mainGame.ShowWrongAnswer(
                (currentQuestionIndex == playingPack.playingQuestions.Count - 1) ? "   OK   " : 
                        AssetsDataMart.Instance.constantsSO.stringsSO.next_question,
                        AssetsDataMart.Instance.constantsSO.stringsSO.timeout + "\n"+
                        AssetsDataMart.Instance.constantsSO.stringsSO.the_answer_is+" " +(char)(65 + correctAnswer),
                       PlayingDataMart.Instance.playingData.total_score
                );

            Debug.Log("Sai roi ban ey");
                mainGame.PlayAudio(AssetsDataMart.Instance.constantsSO.wrongAudioClip);
        }
        public void NextQuestionAfterAnswering()
        {
            mainGame.HideAnswerResultPopupPanel();
            if (Mode == Modes.TYPING_ANSWER)
            {
                if (IsQuestionAnsweredCorrectly(currentQuestionIndex))
                {
                    mainGame.SetPrevAnswer(getTypedQuestion(currentQuestionIndex).answer);
                    NextQuestion();
                }
            }
            else if (Mode == Modes.MCQ)
            {
                if (IsQuestionAnsweredCorrectly(currentQuestionIndex) ||
                    IsQuestionAnsweredWrongly(currentQuestionIndex))
                    NextQuestion();
            }
            Debug.Log("Next it babe");
        }
        public void BuyHint()
        {
            QuestionDataMart.Question question = getQuestion(currentQuestionIndex);
            PlayingDataMart.QuestionPlayingData playingQuestion = playingPack.playingQuestions[currentQuestionIndex];
            if (IsQuestionActive(currentQuestionIndex))
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
                        mainGame.SetHintPrice(nextScore);
                    else
                        mainGame.SetHintPriceActive(false);
                    Debug.Log("Hint count " + playingQuestion.used_hint_count +" "+ question.hints.Count);
                }
                else
                {
                    //no hints left or not enough money.
                }
                string hints = "";
                for (int i = 0; i < playingQuestion.used_hint_count; i++)
                    hints += (i+1) + ". " + question.hints[i] + (i < playingQuestion.used_hint_count - 1 ? "\n" : "");
                mainGame.ShowHintPanel(hints);
            }
            else
            {
                string hints = "";
                for(int i = 0; i< question.hints.Count;i++)
                    hints += i + ". " + question.hints[i] + (i < question.hints.Count-1 ? "\n" : "");
                mainGame.ShowHintPanel(hints);
            }
        }
        public bool UnlockQuestion(int unlockIndex, int howMany = 1)
        {
            if (unlockIndex > recentlyUnlockedQuestionIndex)
            {
                if(unlockIndex < playingPack.playingQuestions.Count)
                {
                    recentlyUnlockedQuestionIndex = unlockIndex;
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
                    recentlyUnlockedQuestionIndex = playingPack.playingQuestions.Count;
                }
            }
            return false;
        }
        public void OnScoreChanged(bool increased)
        {
            mainGame.SetScore(PlayingDataMart.Instance.m_score);
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
            QuestionDataMart.Question question = getQuestion(currentQuestionIndex);
            LocalProvider.Instance.SaveImage(question.images[index].sprite.texture,
                Path.GetFileName(question.images[0].path), (status) => {
                if (status)
                {
                    Debug.Log("saved image bro...");
                        mainGame.ShowSaveFeedback();
                    }
                    else
                    {
                        Debug.Log("Not saved image bro...");
                    }
                });
        }
        public bool IsLastQuestion()
        {
            return (currentQuestionIndex == playingPack.playingQuestions.Count - 1);
        }
        public bool IsInMCQMode()
        {
            return Mode == Modes.MCQ;
        }
        public string GetCurrentQuestionPackName()
        {
            return pack.title;
        }

        private QuestionDataMart.Question getQuestion(int index)
        {
            //if (m_mode == Modes.TYPING_ANSWER)
            //    return pack.typed_questions[index];
            //else if (m_mode == Modes.MCQ)
            //    return pack.mcq_questions[index];
            if (Mode == Modes.TYPING_ANSWER)
                return downloadedTypedQuestion;
            else if (Mode == Modes.MCQ)
                return downloadedMCQQuestion;

            //Download Question
            //int questionId = pack.questions[index].id;
            //HttpClient.Instance.GetQuestionById(questionId, (questionWrapper) =>
            //{
            //    if (questionWrapper.type == 0)
            //    {
            //        HttpClient.TypedQuestion q = questionWrapper.typed_question;
            //        QuestionDataMart.TypedQuestion typedQuestion = new QuestionDataMart.TypedQuestion()
            //        {
            //            id = q.id,
            //            question = q.question,
            //            answer = q.answer,
            //            score = q.score,
            //            hints = Utils.Instance.FromJsonList<List<string>>(q.hints),
            //            images = new List<QuestionDataMart.Image>()
            //        };
            //    }
            //});
            return null;
        }

        private QuestionDataMart.TypedQuestion getTypedQuestion(int index)
        {
            //Download Question
            //int questionId = pack.questions[index].id;
            return downloadedTypedQuestion;
        }
        private QuestionDataMart.MCQQuestion getMCQQuestion(int index)
        {
            //Download Question
            //int questionId = pack.questions[index].id;
            return downloadedMCQQuestion;
        }
        private int getQuestionPackCount()
        {
            //if (m_mode == Modes.TYPING_ANSWER)
            //    return pack.typed_questions.Count;
            //else if (m_mode == Modes.MCQ)
            //    return pack.mcq_questions.Count;
            return pack.questions.Count;
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
            return currentQuestionIndex == index;
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