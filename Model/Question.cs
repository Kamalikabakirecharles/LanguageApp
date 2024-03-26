namespace LanguageApp.Model
{
    public class Question
    {
        public int QuestionID { get; set; }
        public int CourseID { get; set; }
        public int TeacherID { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }

        public Question()
        {
        }

        public Question(int questionID, int courseID, int teacherID, string questionText, string correctAnswer)
        {
            QuestionID = questionID;
            CourseID = courseID;
            TeacherID = teacherID;
            QuestionText = questionText;
            CorrectAnswer = correctAnswer;
        }
    }

}
