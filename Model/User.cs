namespace LanguageApp.Model
{
    public class User
    {
        public int? UserID { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? UserType { get; set; }

        public User()
        {
        }

        public User(int? userID, string? userName, string? password, string? userType)
        {
            UserID = userID;
            UserName = userName;
            Password = password;
            UserType = userType;
        }
    }
}
