using LanguageApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LanguageApp.Pages
{
    public class QuestionsModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM;Integrated Security=True;";
        public string message = "";
        public List<Course> CoursesList = new List<Course>();
        Question question = new Question();

        public void OnGet()
        {
            GetCourses();
        }

        public void OnPost()
        {
                    try
                    {

                    question.CourseID = int.Parse(Request.Form["CourseID"]);
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    question.TeacherID = userId.Value;
                    question.QuestionText = Request.Form["QuestionText"];
                    question.CorrectAnswer = Request.Form["CorrectAnswer"];




                }
                    catch (Exception ex)
                    {
                        message = "Hello! " + ex.Message;
                    }
            try
            {
                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    con.Open();

                    // Adjust the SQL query based on your actual database schema
                    string query = "INSERT INTO Questions (TeacherID, CourseID, QuestionText, CorrectAnswer) VALUES (@TeacherID, @CourseID, @QuestionText, @CorrectAnswer)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameters to the SQL query
                        cmd.Parameters.AddWithValue("@TeacherID", question.TeacherID);
                        cmd.Parameters.AddWithValue("@CourseID", question.CourseID);
                        cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                        cmd.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Answer Posted";
                            question = new Question();
                        }
                        else
                        {
                            message = "Answer Failed";

                        }
                    }
                    con.Close();

                }
            }
            catch(Exception ex)
            {
                message = "Problem: " + ex.Message;
            }
              
            
            GetCourses();
            
        }

        private void GetCourses()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM Courses";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Course currentcourse = new Course
                                {
                                    CourseID = reader.GetInt32(0),
                                    CourseName = reader.GetString(1)
                                };

                                CoursesList.Add(currentcourse);
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
