namespace LanguageApp.Model
{
    public class UserAnswer
    {
        public int? AnswerID { get; set; }
        public int? UserID { get; set; }
        public int? QuestionID { get; set; }
        public string? UserProvidedAnswer { get; set; }
        public bool? IsCorrect { get; set; }

        public UserAnswer()
        {
        }

        public UserAnswer(int answerID, int userID, int questionID, string userProvidedAnswer, bool isCorrect)
        {
            AnswerID = answerID;
            UserID = userID;
            QuestionID = questionID;
            UserProvidedAnswer = userProvidedAnswer;
            IsCorrect = isCorrect;
        }
    }
}
