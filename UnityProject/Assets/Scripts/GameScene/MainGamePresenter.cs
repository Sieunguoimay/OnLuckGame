using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.DataMarts;
using UnityEngine.SceneManagement;
using System;

namespace Assets.Scripts.GameScene
{
    public class MainGamePresenter : MonoBehaviour, ILoadQuestionListener
    {
        public enum Modes { TYPING_ANSWER, MCQ }

        public Modes Mode { private set; get; }

        public bool IsPlayingInMCQNow { get; private set; }

        private MainGame mainGame;
        private QuestionDataMart.Pack pack = null;
        private PlayingDataMart.Pack playingPack = null;
        private int currentQuestionIndex = 0;
        private int recentlyUnlockedQuestionIndex;
        private int packIndex;
        private QuestionDataMart.TypedQuestion downloadedTypedQuestion;
        private QuestionDataMart.MCQQuestion downloadedMCQQuestion;


        private void Awake()
        {
            Debug.Log("MainGamePresenter created: stuff created once goes here");

            mainGame = GetComponent<MainGame>();

            StartGame(Main.Instance.currentPackIndex);
        }

        public void StartGame(int packIndex)
        {
            pack = QuestionDataMart.Instance.packs[packIndex];

            playingPack = PlayingDataMart.Instance.playingPacks[packIndex];

            this.packIndex = packIndex;

            currentQuestionIndex = playingPack.currentIndex == playingPack.playingQuestions.Count ? playingPack.currentIndex - 1 : playingPack.currentIndex;

            recentlyUnlockedQuestionIndex = playingPack.currentIndex;

            if (pack.question_type == 0)
            {
                Mode = Modes.TYPING_ANSWER;
            }

            else if (pack.question_type == 1)
            {
                Mode = Modes.MCQ;
            }

            if (UserDataMart.Instance.m_userData != null)
            {
                mainGame.userInfoBar.SetAvatar(UserDataMart.Instance.m_userData.texProfilePicture);
                mainGame.userInfoBar.SetUserName(UserDataMart.Instance.m_userData.name);
            }
            mainGame.userInfoBar.SetScore(PlayingDataMart.Instance.Score);
            mainGame.SetAnswerMode(Mode);
            mainGame.SetQuestionPackTitleText(pack.title);
            mainGame.SetSeasonText(QuestionDataMart.Instance.season.name, recentlyUnlockedQuestionIndex, playingPack.playingQuestions.Count);

            mainGame.utilityButtons.SeeAnswerButtonClicked += RevealAnswerByWatchingVideo;

            GameLogic.Instance.trackKeeper.Reset();
            QuestionDataMart.Instance.QuestionLoadListener = this;

            OpenQuestion(currentQuestionIndex);


            Debug.Log("MainGamePresenter: The Game Start from here");
        }



        public void Terminate()
        {
            if (IsPlayingInMCQNow)
            {
                MCQForceEnding();
            }

            GameLogic.Instance.OnPlayingDataOutputted(packIndex, recentlyUnlockedQuestionIndex);

            Main.Instance.OpenMenuScene();

            Debug.Log("Escaped from the game");
        }

        public void OnApplicationQuit()
        {
            if (IsPlayingInMCQNow)
            {
                MCQForceEnding();
            }

            GameLogic.Instance.OnPlayingDataOutputted(packIndex, recentlyUnlockedQuestionIndex);

            Debug.Log("MainGamePresenter:OnQuit");
        }


        public void OpenQuestion(int index)
        {
            if (index >= pack.questions.Count)
            {
                return;
            }

            currentQuestionIndex = index;

            mainGame.DisableAllUI();

            QuestionDataMart.Instance.LoadQuestion(pack.questions[index].id, pack.question_type);
        }

        public void OnLoadQuestionResult(QuestionDataMart.TypedQuestion typedQuestion, QuestionDataMart.MCQQuestion mcqQuestion)
        {
            downloadedTypedQuestion = typedQuestion;
            downloadedMCQQuestion = mcqQuestion;

            openQuestion();
        }

        public void OnLoadQuestionFailed()
        {
            currentQuestionIndex = getQuestionPackCount;

            mainGame.EnableUIOnLoadQuestionFailed();
        }

        public void OnLoadImageResult(QuestionDataMart.Image image)
        {
            if (image != null)
            {
                mainGame.UiImageSlideshow.SetQuestionImages(getQuestion().images);
            }
            else
            {
                mainGame.UiImageSlideshow.SetToDefault();
            }
        }

        private void openQuestion()
        {
            if (currentQuestionIndex < getQuestionPackCount)
            {
                var question = getQuestion();

                mainGame.EnableAllUI();

                mainGame.ShowQuestion(question.question, question.images);
                mainGame.SetQuestionNumber(currentQuestionIndex + 1);
                mainGame.LockPrevButton(currentQuestionIndex != 0);
                mainGame.imageSlideShowAnimator.gameObject.SetActive(true);
                mainGame.imageSlideShowAnimator.SetTrigger("show");

                bool state = false;

                if (currentQuestionIndex + 1 < playingPack.playingQuestions.Count)
                {
                    state = !IsQuestionLocked(currentQuestionIndex + 1);
                }
                mainGame.LockNextButton(state);

                if (!IsQuestionLocked(currentQuestionIndex))
                {
                    GameLogic.Instance.trackKeeper.ActivateQuestionPlayingData(playingPack.playingQuestions[currentQuestionIndex]);
                    GameLogic.Instance.hintManager.remainingScore = question.score;
                }
                else
                {
                    Debug.Log("OpenQuestion: This question is locked ");
                }

                if (Mode == Modes.TYPING_ANSWER)
                {
                    var typedQuestion = getTypedQuestion(/*currentQuestionIndex*/);

                    var isActive = IsQuestionActive(currentQuestionIndex);

                    mainGame.ClearAnswerInputField();

                    mainGame.utilityButtons.hintPanel.SetHintAndAnswer(typedQuestion.hints, typedQuestion.FirstAnswer);

                    mainGame.utilityButtons.ShouldShowHintButton = typedQuestion.hints.Count > 0;

                    mainGame.utilityButtons.ResetAll(isActive);

                    mainGame.LockAnswerPanel(isActive);

                    if (!isActive)
                    {
                        mainGame.SetPrevAnswer(AssetsDataMart.Instance.constantsSO.stringsSO.answer+":"+typedQuestion.FirstAnswer);
                        mainGame.SetQuestion(typedQuestion.question + "\n" + AssetsDataMart.Instance.constantsSO.stringsSO.you_have_already_answered_this_question);
                    }

                }
                else if (Mode == Modes.MCQ)
                {
                    var mcqQuestion = getMCQQuestion(/*currentQuestionIndex*/);

                    mainGame.ClearMCQChoicesUI();
                    mainGame.SetMCQChoices(mcqQuestion.choices);

                    if (IsQuestionActive(currentQuestionIndex))
                    {
                        mainGame.StartTimer(mcqQuestion.time);

                        IsPlayingInMCQNow = true;
                    }
                    else
                    {
                        mainGame.ShowMCQAnswer(-1, mcqQuestion.answer);
                        mainGame.ResetTimer(0);

                        IsPlayingInMCQNow = false;
                    }
                }
            }

        }

        public void NextQuestion()
        {
            if (currentQuestionIndex < getQuestionPackCount - 1
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
            mainGame.SetPrevAnswer(answer);

            var typedQuestion = getTypedQuestion(/*currentQuestionIndex*/);

            if (typedQuestion.CheckAnswer(answer))
            {

                if (currentQuestionIndex == playingPack.playingQuestions.Count - 1)
                {
                    mainGame.ShowCongratePopup(pack.title, PlayingDataMart.Instance.playingData.total_score);
                }
                else
                {
                    mainGame.ShowCorrectAnswerPopup(AssetsDataMart.Instance.constantsSO.stringsSO.next_question, PlayingDataMart.Instance.playingData.total_score, GameLogic.Instance.hintManager.remainingScore);
                }

                //mainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                //    unlockStatus ? recentlyUnlockedQuestionIndex : recentlyUnlockedQuestionIndex + 1,
                //    playingPack.playingQuestions.Count);

                EndTypedQuestion(true);


                AudioController.Instance.PlayCorrectSound();

                PlayingDataMart.Instance.Score += typedQuestion.score;

                Debug.Log("Dung roi ban ey");
            }
            else
            {
                mainGame.ShowWrongTypedAnswer();

                AudioController.Instance.PlayWrongSound();

                Debug.Log("Sai roi ban ey");
            }
        }

        private void EndTypedQuestion(bool correct)
        {
            GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], correct?'c':'w');

            UnlockQuestion(currentQuestionIndex + 1);


            mainGame.LockAnswerPanel(false);
            mainGame.LockNextButton(false);

            if (currentQuestionIndex + 1 < playingPack.playingQuestions.Count)
            {
                if (!IsQuestionLocked(currentQuestionIndex + 1))
                {
                    mainGame.LockNextButton(true);
                }
            }
        }

        public void VerifyMCQAnswer(int answer, bool isForcingEnding = false)
        {
            IsPlayingInMCQNow = false;
            var question = getMCQQuestion(/*currentQuestionIndex*/);
            int time = mainGame.StopTimer();
            int thinkingTime = question.time - time;
            int correctAnswer = question.answer;

            UnlockQuestion(currentQuestionIndex + 1);

            //mainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
            //    unlockStatus ? recentlyUnlockedQuestionIndex : recentlyUnlockedQuestionIndex + 1,
            //    playingPack.playingQuestions.Count);

            mainGame.ShowMCQAnswer(answer, correctAnswer);
            mainGame.LockNextButton(currentQuestionIndex + 1 < playingPack.playingQuestions.Count &&!IsQuestionLocked(currentQuestionIndex + 1));


            if (correctAnswer == answer)
            {
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], 'c');

                mainGame.ShowCorrectAnswerPopup(
                    (currentQuestionIndex == playingPack.playingQuestions.Count - 1) ? AssetsDataMart.Instance.constantsSO.stringsSO.ok: AssetsDataMart.Instance.constantsSO.stringsSO.next_question,
                    PlayingDataMart.Instance.playingData.total_score,
                    GameLogic.Instance.hintManager.remainingScore);

                PlayingDataMart.Instance.Score += getMCQQuestion(/*currentQuestionIndex*/).score;

                AudioController.Instance.PlayCorrectSound();
            }
            else
            {
                GameLogic.Instance.hintManager.remainingScore = downloadedMCQQuestion.minus_score;
                GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], 'w');

                if (!isForcingEnding)
                {
                    mainGame.ShowWrongMCQAnswer(
                        (currentQuestionIndex == playingPack.playingQuestions.Count - 1) ? AssetsDataMart.Instance.constantsSO.stringsSO.ok: AssetsDataMart.Instance.constantsSO.stringsSO.next_question,
                        downloadedMCQQuestion.minus_score);
                }
                AudioController.Instance.PlayWrongSound();
            }
        }

        public void MCQTimeout()
        {
            IsPlayingInMCQNow = false;

            bool unlockStatus = UnlockQuestion(currentQuestionIndex + 1);

            int correctAnswer = getMCQQuestion(/*currentQuestionIndex*/).answer;

            mainGame.ShowMCQAnswer(-1, correctAnswer);

            mainGame.SetSeasonText(QuestionDataMart.Instance.season.name,
                unlockStatus ? recentlyUnlockedQuestionIndex : recentlyUnlockedQuestionIndex + 1,
                playingPack.playingQuestions.Count);

            GameLogic.Instance.hintManager.remainingScore = downloadedMCQQuestion.minus_score;
            GameLogic.Instance.trackKeeper.TerminateQuestionData(playingPack.playingQuestions[currentQuestionIndex], 'w');

            mainGame.ShowWrongMCQAnswer(
                (currentQuestionIndex == playingPack.playingQuestions.Count - 1) ? AssetsDataMart.Instance.constantsSO.stringsSO.ok: AssetsDataMart.Instance.constantsSO.stringsSO.next_question,
                downloadedMCQQuestion.minus_score);


            AudioController.Instance.PlayWrongSound();
        }

        private void RevealAnswerByWatchingVideo()
        {
            GameLogic.Instance.hintManager.remainingScore = -150;
            mainGame.ShowAnswerPopup(downloadedTypedQuestion.FirstAnswer);
            EndTypedQuestion(false);
        }

        public void NextQuestionAfterAnswering()
        {
            if (Mode == Modes.TYPING_ANSWER)
            {
                if (IsQuestionAnsweredCorrectly(currentQuestionIndex))
                {
                    //mainGame.SetPrevAnswer(getTypedQuestion(/*currentQuestionIndex*/).FirstAnswer);
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

        public bool UnlockQuestion(int unlockIndex, int howMany = 1)
        {
            if (unlockIndex > recentlyUnlockedQuestionIndex)
            {
                if (unlockIndex < playingPack.playingQuestions.Count)
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
                else if (unlockIndex == playingPack.playingQuestions.Count)
                {
                    //this pack is finished
                    recentlyUnlockedQuestionIndex = playingPack.playingQuestions.Count;
                }
            }
            return false;
        }


        public void MCQForceEnding()
        {
            VerifyMCQAnswer(-1, true);
        }

        public void SaveImageOfTheCurrentQuestion(int index)
        {
            var question = getQuestion();
            LocalProvider.Instance.SaveImage(question.images[index].sprite.texture,
                Path.GetFileName(question.images[0].path), (status) =>
                {
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

        public bool IsLastQuestion=> (currentQuestionIndex == playingPack.playingQuestions.Count - 1);
        public bool IsInMCQMode=>Mode == Modes.MCQ;

        public string GetCurrentQuestionPackName => pack.title;

        private QuestionDataMart.Question getQuestion()
        {
            if (Mode == Modes.TYPING_ANSWER)
                return downloadedTypedQuestion;
            else if (Mode == Modes.MCQ)
                return downloadedMCQQuestion;

            return null;
        }

        private QuestionDataMart.TypedQuestion getTypedQuestion(/*int index*/)
        {
            return downloadedTypedQuestion;
        }
        private QuestionDataMart.MCQQuestion getMCQQuestion(/*int index*/)
        {
            return downloadedMCQQuestion;
        }
        private int getQuestionPackCount => pack.questions.Count;



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
//int usedHintCount = playingPack.playingQuestions[currentQuestionIndex].used_hint_count;

//if (question.hints.Count > 0 && PlayingDataMart.Instance.m_score > 0)
//{
//    mainGame.SetHintUiInteractable(true);
//    if (IsQuestionActive(currentQuestionIndex) && usedHintCount < question.hints.Count)
//    {
//        mainGame.SetHintPriceActive(true);
//        mainGame.SetHintPrice(GameLogic.Instance.hintManager.GetHintPrice(usedHintCount));
//    }
//    else
//    {
//        mainGame.SetHintPriceActive(false);
//    }
//    Debug.Log("OpenQuestion: Enough money");
//}
//else
//{
//    Debug.Log("OpenQuestion: Not enough money: your money = " + PlayingDataMart.Instance.m_score + ", hints count = " + question.hints.Count);
//    if (!IsQuestionActive(currentQuestionIndex))
//        mainGame.SetHintUiInteractable(true);
//    else
//    {
//        if (usedHintCount > 0)
//            mainGame.SetHintUiInteractable(true);
//        else
//            mainGame.SetHintUiInteractable(false);
//    }
//}






//public void BuyHint()
//{
//    QuestionDataMart.Question question = getQuestion(currentQuestionIndex);
//    PlayingDataMart.QuestionPlayingData playingQuestion = playingPack.playingQuestions[currentQuestionIndex];
//    if (IsQuestionActive(currentQuestionIndex))
//    {
//        if (playingQuestion.used_hint_count < question.hints.Count && PlayingDataMart.Instance.m_score > 0)
//        {
//            int subtractingScore = GameLogic.Instance.hintManager.GetHintPrice(playingQuestion.used_hint_count);
//            int nextScore = GameLogic.Instance.hintManager.GetHintPrice(playingQuestion.used_hint_count + 1);

//            PlayingDataMart.Instance.SubtractScore(subtractingScore);
//            GameLogic.Instance.hintManager.SubtractRemainingScoreOfQuestion(subtractingScore);
//            GameLogic.Instance.trackKeeper.ChangeUsedHintCount(playingQuestion, playingQuestion.used_hint_count + 1);
//            //playingQuestion.used_hint_count++;
//            if (playingQuestion.used_hint_count < question.hints.Count)
//                mainGame.SetHintPrice(nextScore);
//            else
//                mainGame.SetHintPriceActive(false);
//            Debug.Log("Hint count " + playingQuestion.used_hint_count +" "+ question.hints.Count);
//        }
//        else
//        {
//            //no hints left or not enough money.
//        }
//        string hints = "";
//        for (int i = 0; i < playingQuestion.used_hint_count; i++)
//            hints += (i+1) + ". " + question.hints[i] + (i < playingQuestion.used_hint_count - 1 ? "\n" : "");
//        mainGame.ShowHintPanel(hints);
//    }
//    else
//    {
//        string hints = "";
//        for(int i = 0; i< question.hints.Count;i++)
//            hints += i + ". " + question.hints[i] + (i < question.hints.Count-1 ? "\n" : "");
//        mainGame.ShowHintPanel(hints);
//    }
//}

//if (m_mode == Modes.TYPING_ANSWER)
//    return pack.typed_questions[index];
//else if (m_mode == Modes.MCQ)
//    return pack.mcq_questions[index];

//if (m_mode == Modes.TYPING_ANSWER)
//    return pack.typed_questions.Count;
//else if (m_mode == Modes.MCQ)
//    return pack.mcq_questions.Count;