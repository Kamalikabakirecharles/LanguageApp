using LanguageApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace LanguageApp.Pages
{
    public class IndexModel : PageModel


    {



        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM;Integrated Security=True;";
        public string message = "";
        public List<Question> QuestionList = new List<Question>();
        public List<UserAnswer> AnswersList = new List<UserAnswer>();
        Question question = new Question();
        UserAnswer answer = new UserAnswer();

        [BindProperty(SupportsGet = true)]
        public int SelectedCourseId { get; set; }


        public SelectList CourseList { get; set; }
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

            // Populate the course dropdown
            PopulateCourseList();

            // Get questions for the default course
            getQuestions();
            getAnswers();

        }


        public void getQuestions()
        {
            try
            {
                QuestionList.Clear(); 

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM Questions ";
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
                answer.UserID = userId.Value;

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM UserAnswers where UserID=@UserID";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", answer.UserID);

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

        private void PopulateCourseList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT CourseID, CourseName FROM Courses";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            List<SelectListItem> courses = new List<SelectListItem>();
                            while (reader.Read())
                            {
                                courses.Add(new SelectListItem
                                {
                                    Value = reader.GetInt32(0).ToString(),
                                    Text = reader.GetString(1)
                                });
                            }

                            CourseList = new SelectList(courses, "Value", "Text");
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
