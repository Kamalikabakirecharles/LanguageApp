namespace LanguageApp.Model
{
    public class Course
    {
        public int? CourseID { get; set; }
        public string? CourseName { get; set; }

        public Course()
        {
        }

        public Course(int? courseID, string? courseName)
        {
            CourseID = courseID;
            CourseName = courseName;
        }
    }
}
