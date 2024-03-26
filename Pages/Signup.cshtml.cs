using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Security.Cryptography;
using LanguageApp.Model;
using System.Data.SqlClient;


namespace LanguageApp.Pages
{
    public class SignupModel : PageModel
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

        public void OnPost()
        {
            try
            {

                user.UserName = Request.Form["username"];
                String encrypted = encryptPasswd(Request.Form["password"]);
                user.Password = encrypted;
                user.UserType = Request.Form["UserType"];

            }
            catch (Exception ex)
            {
                message = "Hello!: " + ex.Message;
            }
            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string query = "INSERT INTO users(username,UserType,password)VALUES(@username, @UserType, @password)";
                try
                {

                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", user.UserName);
                        cmd.Parameters.AddWithValue("@UserType", user.UserType);
                        cmd.Parameters.AddWithValue("@password", user.Password);

                        int rowAffected = cmd.ExecuteNonQuery();

                        if (rowAffected > 0)
                        {
                            message = "New User Created";
                            user = new User();
                        }
                        else
                        {
                            message = "User Not Created";
                        }

                    }

                    con.Close();
                }
                catch (Exception ex)
                {
                    message = "Hello!: " + ex.Message;
                }
            }
        }


    }
}
