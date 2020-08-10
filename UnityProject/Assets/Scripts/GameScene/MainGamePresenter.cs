using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.DataMarts;
namespace Assets.Scripts.GameScene
{
    public class MainGamePresenter
    {


        public enum Modes
        {
            TYPING_ANSWER,
            MCQ
        }
        //public enum Packs
        //{
        //    PACK_80,
        //    PACK_1000,
        //    PACK_30
        //}
        //public enum AnswerStatuses
        //{
        //    NOT_ANSWERED,
        //    CORRECTLY_ANSWERED,
        //    WRONGLY_ANSWERED
        //}
        private static MainGamePresenter s_instance = null;
        public static MainGamePresenter Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new MainGamePresenter();
                }
                return s_instance;
            }
        }
        private Modes mode;
        public Modes m_mode {
            get
            {
                return mode;
            }
            set{
                mode = value;
                if(m_rMainGame!=null)
                    m_rMainGame.SetAnswerMode(value);
            }
        }
        //public Packs m_pack { get; private set; }

        private MainGame m_rMainGame = null;
        public bool m_isPlayingInMCQNow { get; private set; }


        private List<QuestionDataMart.TypedQuestion> m_typedQuestionPack;
        //{
        //    get
        //    {
        //        if (m_pack == Packs.PACK_80)
        //        {
        //            return m_80QuestionPack;
        //        }
        //        else if (m_pack == Packs.PACK_1000)
        //        {
        //            return m_1000QuestionPack;
        //        }
        //        return null;
        //    }
        //}
        private List<QuestionDataMart.MCQQuestion> m_mcqQuestionPack;
        //{
        //    get
        //    {
        //        if (m_pack == Packs.PACK_30)
        //        {
        //            return m_30QuestionPack;
        //        }
        //        return null;
        //    }
        //}
        private List<DataMarts.PlayingDataMart.QuestionPlayingData> m_questionPlayingImage;
        //private int[] m_currentQuestionIndices = new int[3] { 0, 0, 0 };
        private int m_currentQuestionIndex;
        private int m_recentlyUnlockedQuestionIndex;
        private int m_packIndex;
        //{
        //    get
        //    {
        //        return m_currentQuestionIndices[(int)m_pack];
        //    }
        //    set
        //    {
        //        m_currentQuestionIndices[(int)m_pack] = value;
        //    }
        //}



        private MainGamePresenter()
        {
            Debug.Log("MainGamePresenter created");
            //m_pack = Packs.PACK_30;
            m_currentQuestionIndex = 0;
            PlayingDataMart.Instance.m_scoreChangedCallback = OnScoreChanged;
        }

        public void SetToMCQMode(
            List<QuestionDataMart.MCQQuestion> questionPack,
            List<DataMarts.PlayingDataMart.QuestionPlayingData> questionPlayingImage,
            int unlockedQuestionIndex,int packId)
        {
            m_mcqQuestionPack = questionPack;
            m_questionPlayingImage = questionPlayingImage;
            m_currentQuestionIndex = unlockedQuestionIndex;
            m_recentlyUnlockedQuestionIndex = unlockedQuestionIndex;
            m_mode = Modes.MCQ;
            m_packIndex = packId;
        }
        public void SetToTypingMode(
            List<QuestionDataMart.TypedQuestion> questionPack,
            List<DataMarts.PlayingDataMart.QuestionPlayingData> questionPlayingImage,
            int unlockedQuestionIndex, int packId)
        {
            m_typedQuestionPack = questionPack;
            m_questionPlayingImage = questionPlayingImage;
            m_currentQuestionIndex = unlockedQuestionIndex;
            m_recentlyUnlockedQuestionIndex = unlockedQuestionIndex;
            m_mode = Modes.TYPING_ANSWER;
            m_packIndex = packId;
        }
        public delegate void OutputDataOnExit(int packId, int recentlyUnlockedQuestionIndex);
        public OutputDataOnExit m_outputDataOnExit = null;
        public void Terminate()
        {
            if(m_isPlayingInMCQNow)
                MCQForceEnding();

            if(m_outputDataOnExit!=null)
                m_outputDataOnExit(m_packIndex, m_recentlyUnlockedQuestionIndex);
            Debug.Log("Escaped from the game");
        }


        public void Init(MainGame rMainGame)
        {
            m_rMainGame = rMainGame;
            m_outputDataOnExit = PlayingDataMart.Instance.OnPlayingDataOutputted;
        }

        public void Start()
        {
            if (m_rMainGame != null)
            {
                m_rMainGame.SetAnswerMode(m_mode);
                m_rMainGame.SetQuestionPackTitleText(GetCurrentQuestionPackName());
                updateSeasonUiText(m_recentlyUnlockedQuestionIndex - 1);
            }
            OpenQuestion(m_currentQuestionIndex);
            Debug.Log("The Game Start from here");
        }


        public void CheckAndDownloadNewGameData()
        {
            //Check if there is new data, download it regardless of where the player currently is.???
        }

        public void LoadQuestionListIntoUiList(ScrollList uiList)
        {
            uiList.CreateList<QuestionItem>(m_questionPlayingImage.Count, (item, index) =>
            {
                item.SetIndex(index);
                item.SetState(QuestionItem.CharToState(m_questionPlayingImage[index].m_status));
            });
        }
        public void OpenQuestion(int index)
        {
            m_currentQuestionIndex = index;
            if(m_currentQuestionIndex< getQuestionPackCount())
            {
                QuestionDataMart.Question question = getQuestion(index);
                //Set all other info to the question panel here.
                m_rMainGame.SetQuestionNumber(index);
                m_rMainGame.SetQuestion(question.question);
                m_rMainGame.SetQuestionNumber(question.number + 1);
                m_rMainGame.SetQuestionImage(question.texImage);
                m_rMainGame.HideHintPanel();
                m_rMainGame.SetUiNextButtonInteractable(
                    m_currentQuestionIndex + 1 < m_questionPlayingImage.Count &&
                    m_questionPlayingImage[m_currentQuestionIndex + 1].m_status != 'l');

                if (m_questionPlayingImage[m_currentQuestionIndex].m_status == 'u')
                {
                    m_questionPlayingImage[m_currentQuestionIndex].m_started = new System.DateTime();
                    m_questionPlayingImage[m_currentQuestionIndex].m_score = question.score;
                    if (question.hints.Count > 0&& PlayingDataMart.Instance.m_score > 0)
                    {
                        m_rMainGame.SetHintUtInteractable(true);
                        m_rMainGame.SetHintPrice(getScoreToSubtract(0));
                        Debug.Log("Enough money");
                    }
                    else
                    {
                        Debug.Log("Not enough money "+ PlayingDataMart.Instance.m_score+" "+ question.hints.Count);
                    }
                }
                else
                {

                }

                if (m_mode == Modes.TYPING_ANSWER)
                {
                    m_rMainGame.ClearAnswerInputField();
                    m_rMainGame.SetUiInteractableInTypingMode(true);
                }
                else if (m_mode == Modes.MCQ)
                {
                    QuestionDataMart.MCQQuestion mcqQuestion = m_mcqQuestionPack[m_currentQuestionIndex];
                    PlayingDataMart.QuestionPlayingData playingData = m_questionPlayingImage[m_currentQuestionIndex];
                    string[] choices = mcqQuestion.choices;
                    m_rMainGame.SetMCQChoices(choices);
                    m_rMainGame.ClearMCQChoicesColor();

                    if(playingData.m_status=='c'|| playingData.m_status == 'w')
                    {
                        int correctAnswer = mcqQuestion.answer;
                        m_rMainGame.ShowMCQAnswer(-1, correctAnswer);
                        m_rMainGame.ResetTimer(0);
                        m_isPlayingInMCQNow = false;
                    }
                    else
                    {

                        m_rMainGame.StartTimer(m_mcqQuestionPack[m_currentQuestionIndex].time);
                        m_isPlayingInMCQNow = true;
                    }
                }
            }
        }
        public void HideQuestion()
        {
            m_rMainGame.ShowQuestionListPanel();
            m_rMainGame.HideQuestionPanel();
        }

        public void NextQuestion()
        {

            if (m_currentQuestionIndex < getQuestionPackCount()-1
                && IsUnlockedQuestion(m_currentQuestionIndex + 1))
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
        public void SaveImageOfTheCurrentQuestion()
        {
            QuestionDataMart.Question question = getQuestion(m_currentQuestionIndex);
            LocalProvider.Instance.SaveImage(question.texImage, Path.GetFileName(question.image),(status)=> {
                if (status)
                {
                    Debug.Log("saved image bro...");
                }
            });
        }
        public void VerifyTypedAnswer(string answer)
        {
            answer = Utils.Instance.convertToUnSign3(answer).ToLower();
            if (Utils.Instance.convertToUnSign3(m_typedQuestionPack[m_currentQuestionIndex].answer.ToLower()).Equals(answer))
            {
                //m_typedQuestionPack[m_currentQuestionIndex].answerStatus = AnswerStatuses.CORRECTLY_ANSWERED;
                m_questionPlayingImage[m_currentQuestionIndex].m_status = 'c';
                if(m_currentQuestionIndex == m_questionPlayingImage.Count-1)
                    m_rMainGame.ShowCongratePopup(QuestionDataMart.Instance.GetQuestionPacksMetadata()[m_packIndex].title);
                else
                    m_rMainGame.ShowCorrectAnswer("CAU TIEP THEO");
                m_rMainGame.SetUiNextButtonInteractable(true);
                m_rMainGame.SetUiInteractableInTypingMode(false);
                m_rMainGame.SetPrevAnswer(answer);
                m_rMainGame.SetHintUtInteractable(false);

                PlayingDataMart.Instance.AddScore(m_typedQuestionPack[m_currentQuestionIndex].score);

                if(UnlockQuestion(m_currentQuestionIndex + 1))
                    updateSeasonUiText(m_recentlyUnlockedQuestionIndex - 1);
                else
                    updateSeasonUiText(m_recentlyUnlockedQuestionIndex);

                m_rMainGame.SetUiNextButtonInteractable(
                    m_currentQuestionIndex + 1 < m_questionPlayingImage.Count &&
                    m_questionPlayingImage[m_currentQuestionIndex + 1].m_status != 'l');

                m_questionPlayingImage[m_currentQuestionIndex].m_ended = new System.DateTime();

                Debug.Log("Dung roi ban ey");
            }
            else
            {
                //m_mcqQuestionPack[m_currentQuestionIndex].answerStatus = AnswerStatuses.WRONGLY_ANSWERED;
                m_questionPlayingImage[m_currentQuestionIndex].m_status = 'w';
                m_rMainGame.ShowWrongAnswer("THU LAI", "KHONG CHINH XAC");
                Debug.Log("Sai roi ban ey");
            }
        }
        public void VerifyMCQAnswer(int answer,bool isForcingEnding = false)
        {
            QuestionDataMart.MCQQuestion question = m_mcqQuestionPack[m_currentQuestionIndex];
            int correctAnswer = question.answer;
            m_rMainGame.ShowMCQAnswer(answer,correctAnswer);
            //disable other buttons
            m_rMainGame.SetHintUtInteractable(false);
            m_isPlayingInMCQNow = false;

            int time = m_rMainGame.StopTimer();
            int thinkingTime = question.time - time;

            if (UnlockQuestion(m_currentQuestionIndex + 1))
                updateSeasonUiText(m_recentlyUnlockedQuestionIndex - 1);
            else
                updateSeasonUiText(m_recentlyUnlockedQuestionIndex);
            m_rMainGame.SetUiNextButtonInteractable(
                m_currentQuestionIndex + 1 < m_questionPlayingImage.Count &&
                m_questionPlayingImage[m_currentQuestionIndex + 1].m_status != 'l');

            if (question.answer == answer)
            {
                //question.answerStatus = AnswerStatuses.CORRECTLY_ANSWERED;
                m_questionPlayingImage[m_currentQuestionIndex].m_status = 'c';
                m_rMainGame.ShowCorrectAnswer((m_currentQuestionIndex == m_questionPlayingImage.Count - 1)?"KET THUC":"CAU TIEP THEO");

                PlayingDataMart.Instance.AddScore(m_mcqQuestionPack[m_currentQuestionIndex].score);

                Debug.Log("Dung roi ban ey : "+ thinkingTime+"s");

            }
            else
            {
                //question.answerStatus = AnswerStatuses.WRONGLY_ANSWERED;
                m_questionPlayingImage[m_currentQuestionIndex].m_status = 'w';
                if (!isForcingEnding) {
                    m_rMainGame.ShowWrongAnswer(
                        (m_currentQuestionIndex == m_questionPlayingImage.Count - 1) ? "KET THUC" : "CAU TIEP THEO",
                        "KHONG CHINH XAC\nDap an la: " + (char)(65 + correctAnswer));

                    //m_rMainGame.ShowAnswerResultPopupPanel(false);
                    //m_rMainGame.SetCorrectAnswer("\nDap an la: " + (char)(65 + correctAnswer));
                }
                Debug.Log("Sai roi ban ey: " + thinkingTime + "s");
            }
        }
        public void MCQTimeout()
        {
            int correctAnswer = m_mcqQuestionPack[m_currentQuestionIndex].answer;
            m_rMainGame.ShowMCQAnswer(-1,correctAnswer);
            //disable other buttons
            m_rMainGame.SetHintUtInteractable(false);
            m_isPlayingInMCQNow = false;

            UnlockQuestion(m_currentQuestionIndex + 1);

            m_questionPlayingImage[m_currentQuestionIndex].m_status = 'w';

            //m_mcqQuestionPack[m_currentQuestionIndex].answerStatus = AnswerStatuses.WRONGLY_ANSWERED;
            m_rMainGame.ShowWrongAnswer(
                (m_currentQuestionIndex == m_questionPlayingImage.Count - 1) ? "KET THUC" : "CAU TIEP THEO",
                "HET THOI GIAN\nDap an la: " + (char)(65 + correctAnswer));
            //m_rMainGame.ShowAnswerResultPopupPanel(false);
            //m_rMainGame.SetCorrectAnswer("\nDap an la: " + (char)(65 + correctAnswer),true);
            Debug.Log("Sai roi ban ey");
        }
        public void MCQForceEnding()
        {
            VerifyMCQAnswer(-1,true);
        }
        public bool IsLastQuestion()
        {
            return (m_currentQuestionIndex == m_questionPlayingImage.Count - 1);
        }
        public bool IsInMCQMode()
        {
            return m_mode == MainGamePresenter.Modes.MCQ;
        }
        public string GetCurrentQuestionPackName()
        {
            return QuestionDataMart.Instance.GetQuestionPacksMetadata()[m_packIndex].title;
        }
        public void NextQuestionAfterAnswering()
        {
            m_rMainGame.HideAnswerResultPopupPanel();
            if (m_mode == Modes.TYPING_ANSWER)
            {
                if (m_questionPlayingImage[m_currentQuestionIndex].m_status == 'c')
                {
                    NextQuestion();
                    m_rMainGame.SetPrevAnswer(m_typedQuestionPack[m_currentQuestionIndex].answer);
                }
            }
            else if (m_mode == Modes.MCQ)
            {
                if (m_questionPlayingImage[m_currentQuestionIndex].m_status == 'c' ||
                    m_questionPlayingImage[m_currentQuestionIndex].m_status == 'w')
                    NextQuestion();
            }
            Debug.Log("Next it babe");
        }
        public void BuyHint()
        {
            QuestionDataMart.Question question = getQuestion(m_currentQuestionIndex);
            if (m_questionPlayingImage[m_currentQuestionIndex].m_usedHintCount < question.hints.Count&&
                PlayingDataMart.Instance.m_score > 0)
            {
                int subtractingScore = getScoreToSubtract(m_questionPlayingImage[m_currentQuestionIndex].m_usedHintCount);
                int nextScore = getScoreToSubtract(m_questionPlayingImage[m_currentQuestionIndex].m_usedHintCount + 1);
                m_rMainGame.ShowHintPanel(question.hints[m_questionPlayingImage[m_currentQuestionIndex].m_usedHintCount++]);
                m_questionPlayingImage[m_currentQuestionIndex].m_score -= subtractingScore;
                PlayingDataMart.Instance.SubtractScore(subtractingScore);
                m_rMainGame.SetHintPrice(nextScore);
            }
            else
            {
                //no hints left or not enough money.
            }
        }
        private int getScoreToSubtract(int level)
        {
            return (level+1) * 10;
        }

        private QuestionDataMart.Question getQuestion(int index)
        {
            if (m_mode == Modes.TYPING_ANSWER)
            {
                return m_typedQuestionPack[index];
            }
            else if (m_mode == Modes.MCQ)
            {
                return m_mcqQuestionPack[index];
            }
            return null;
        }
        private int getQuestionPackCount()
        {
            if (m_mode == Modes.TYPING_ANSWER)
            {
                return m_typedQuestionPack.Count;
            }
            else if (m_mode == Modes.MCQ)
            {
                return m_mcqQuestionPack.Count;
            }
            return 0;
        }
        //private Modes getModeByPack()
        //{
        //    switch (m_pack)
        //    {
        //        case Packs.PACK_80:
        //            return Modes.TYPING_ANSWER;
        //        case Packs.PACK_1000:
        //            return Modes.TYPING_ANSWER;
        //        case Packs.PACK_30:
        //            return Modes.MCQ;
        //    }
        //    return Modes.TYPING_ANSWER;
        //}
        public bool UnlockQuestion(int unlockIndex,int howMany = 1)
        {
            if (unlockIndex > m_recentlyUnlockedQuestionIndex
                &&unlockIndex<m_questionPlayingImage.Count)
            {
                m_recentlyUnlockedQuestionIndex = unlockIndex;
                for(int i = 0; i<howMany; i++)
                {
                    if(i+unlockIndex < m_questionPlayingImage.Count)
                    {
                        m_questionPlayingImage[unlockIndex + i].m_status = 'u';
                        Debug.Log("Unlocked question " + (unlockIndex+i));
                    }
                }
                return true;
            }
            return false;
        }
        public bool IsUnlockedQuestion(int index)
        {
            return (m_questionPlayingImage[index].m_status != 'l');
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
        public bool CheckCurrentQuestionIndex(int index)
        {
            return m_currentQuestionIndex == index;
        }
        private void updateSeasonUiText(int currentQuestion)
        {
            int packCount = m_questionPlayingImage.Count;
            m_rMainGame.SetSeasonText("MUA " + QuestionDataMart.Instance.GetSeasonNumber() + "(" + (currentQuestion + 1) + "/" + packCount + ")");
        }

    }
}