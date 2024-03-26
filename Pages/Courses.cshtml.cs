using LanguageApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace LanguageApp.Pages
{
    public class CoursesModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM;Integrated Security=True;";
        public string message = "";
        Course course = new Course();

        public void OnGet()
        {
        }
        public void OnPost()
        {

            try
            {

                course.CourseName = Request.Form["Courses"];
                
            }
            catch (Exception ex)
            {
                message = "Hello! " + ex.Message;
            }
            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string query = "INSERT INTO Courses(CourseName) VALUES(@CourseName)";

                try
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CourseName", course.CourseName);
                        

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Course Posted";
                            course = new Course();
                        }
                        else
                        {
                            message = "Course Failed";

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

    }
}
