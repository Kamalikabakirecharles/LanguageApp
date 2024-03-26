    using LanguageApp.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Security.Cryptography;
    using System.Data.SqlClient;
    using System.Text;

    namespace LanguageApp.Pages
    {
        public class LoginModel : PageModel
        {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM;Integrated Security=True;";
        public string message = "";
            User user = new User();

            public void OnGet()
            {
            }

            private string encryptPasswd(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    string base64Hash = Convert.ToBase64String(hashBytes);
                    return base64Hash;
                }
            }

        public IActionResult OnPost()
        {
            try
            {
                user.UserName = Request.Form["username"];
                string encrypted = encryptPasswd(Request.Form["password"]);
                user.Password = encrypted;
            }
            catch (Exception ex)
            {
                message = "Hello!: " + ex.Message;
            }

            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string query = "SELECT UserID, usertype FROM users WHERE UserName=@username AND password=@password";

                try
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", user.UserName);
                        cmd.Parameters.AddWithValue("@password", user.Password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string userType = reader.GetString(1);

                                HttpContext.Session.SetInt32("UserId", userId);

                                if (userType == "Instructor")
                                {
                                    return RedirectToPage("/TeacherDashboard");
                                }
                                else if (userType == "Learner")
                                {
                                    return RedirectToPage("/StudentDashboard");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "Error: " + ex.Message;
                }
            }

            return Page();
        }



    }
}
