using LanguageApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace LanguageApp.Pages
{
    public class MarkQuestionsModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM;Integrated Security=True;";
        public string message = "";
        public List<Question> QuestionList = new List<Question>();
        public List<UserAnswer> AnswersList = new List<UserAnswer>();
        Question question = new Question();
        UserAnswer answers = new UserAnswer();

        public void OnGet()
        {
            getQuestions();
            getAnswers();
        }


        public void OnPost()
        {
            try
            {
                answers.QuestionID = int.Parse(Request.Form["answer_id"]);

                // Check if 'iscorrect' exists in the form data and is not null
                if (Request.Form.TryGetValue("iscorrect", out var isCorrectValue) && !string.IsNullOrEmpty(isCorrectValue))
                {
                    // Use the boolean.TryParse directly without setting a default value
                    if (bool.TryParse(isCorrectValue, out bool isCorrect))
                    {
                        using (SqlConnection con = new SqlConnection(stgcon))
                        {
                            string query = "UPDATE UserAnswers SET IsCorrect = @isCorrect WHERE AnswerID = @answerId";

                            try
                            {
                                con.Open();
                                using (SqlCommand cmd = new SqlCommand(query, con))
                                {
                                    // Use DBNull.Value if isCorrect is null, otherwise use the boolean value
                                    cmd.Parameters.Add("@isCorrect", System.Data.SqlDbType.Bit).Value = (object)isCorrect ?? DBNull.Value;
                                    cmd.Parameters.Add("@answerId", System.Data.SqlDbType.Int).Value = answers.QuestionID;

                                    int rowsAffected = cmd.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        message = "Answer Corrected";
                                        answers = new UserAnswer();
                                    }
                                    else
                                    {
                                        message = "Answer Failed to Correct";
                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                message = "Problem: " + ex.Message;
                            }
                        }
                    }
                    else
                    {
                        message = "'iscorrect' parsing failed";
                    }
                }
                else
                {
                    // Handle the case where 'iscorrect' is not present in the form data
                    message = "'iscorrect' is missing or null in form data";
                }
            }
            catch (Exception ex)
            {
                message = "Hello! " + ex.Message;
            }

            getQuestions();
            getAnswers();
        }


        public void getQuestions()
        {
            try
            {

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM Questions";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                Question currentquestion = new Question();

                                currentquestion.QuestionID = reader.GetInt32(0);
                                currentquestion.CourseID = reader.GetInt32(1);
                                currentquestion.TeacherID = reader.GetInt32(2);
                                currentquestion.QuestionText = reader.GetString(3);
                                currentquestion.CorrectAnswer = reader.GetString(4);

                                QuestionList.Add(currentquestion);
                            }
                        }
                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                message = "Problem: " + ex.Message;
            }
        }
        public void getAnswers()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                answers.UserID = userId.Value;

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM UserAnswers";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {


                                UserAnswer currentquestion = new UserAnswer();

                                currentquestion.AnswerID = reader.GetInt32(0);
                                currentquestion.UserID = reader.GetInt32(1);
                                currentquestion.QuestionID = reader.GetInt32(2);
                                currentquestion.UserProvidedAnswer = reader.GetString(3);
                                currentquestion.IsCorrect = reader.GetBoolean(4);

                                AnswersList.Add(currentquestion);
                            }
                        }
                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                message = "Problem: " + ex.Message;
            }
        }
    }
}
