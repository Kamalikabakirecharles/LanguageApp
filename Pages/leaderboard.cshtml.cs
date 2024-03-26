using System.Collections.Generic;
using System.Linq;
using LanguageApp.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Reflection.Metadata;
using DocumentIText = iText.Layout.Document;

namespace LanguageApp.Pages
{
    public class leaderboardModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM;Integrated Security=True;";

        public List<UserPerformance> UserPerformances { get; set; }

        public void OnGet()
        {
            // Assuming you have a way to identify the current user
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                GetUserPerformance(userId.Value);
            }
        }

        private void GetUserPerformance(int userId)
        {
            UserPerformances = new List<UserPerformance>();

            try
            {
                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = @"
                SELECT C.CourseID, C.CourseName,
                    COUNT(UA.AnswerID) AS TotalAnswers,
                    COUNT(DISTINCT Q.QuestionID) AS TotalQuestions,
                    SUM(CAST(UA.IsCorrect AS INT)) AS CorrectAnswers,
                    (COUNT(UA.AnswerID) - SUM(CAST(UA.IsCorrect AS INT))) AS WrongAnswers
                FROM Courses C
                LEFT JOIN Questions Q ON C.CourseID = Q.CourseID
                LEFT JOIN UserAnswers UA ON Q.QuestionID = UA.QuestionID
                WHERE UA.UserID = @UserID
                GROUP BY C.CourseID, C.CourseName";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserPerformance userPerformance = new UserPerformance
                                {
                                    CourseID = reader.GetInt32(0),
                                    CourseName = reader.GetString(1),
                                    TotalAnswers = reader.GetInt32(2),
                                    TotalQuestions = reader.GetInt32(3),
                                    CorrectAnswers = reader.GetInt32(4),
                                    WrongAnswers = reader.GetInt32(5)
                                };

                                UserPerformances.Add(userPerformance);
                            }
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        public class UserPerformance
        {
            public int CourseID { get; set; }
            public string CourseName { get; set; }
            public int TotalAnswers { get; set; }
            public int TotalQuestions { get; set; }
            public int CorrectAnswers { get; set; }
            public int WrongAnswers { get; set; }
        }

        public IActionResult OnPostDownloadPdf()
        {
            // Ensure that riddleList is populated by calling OnGet
            OnGet();

            byte[] pdfBytes = GeneratePdf();

            // Set the content type and file name for the response
            Response.Headers["Content-Disposition"] = "attachment; filename=Report.pdf";
            Response.Headers["Content-Type"] = "application/pdf";

            // Return the PDF as a file download
            return File(pdfBytes, "application/pdf");
        }

        private byte[] GeneratePdf()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(stream))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        // Use the alias for iText.Layout.Document
                        using (DocumentIText document = new DocumentIText(pdf))
                        {
                            // Create a table with the same number of columns as your displayed table
                            float[] columnWidths = { 2, 2, 2, 2, 2 }; // Adjust the widths as needed
                            Table table = new Table(columnWidths);

                            // Add table header
                            table.AddHeaderCell("Course");
                            table.AddHeaderCell("Total Questions");
                            table.AddHeaderCell("Total Answers");
                            table.AddHeaderCell("Correct Answers");
                            table.AddHeaderCell("Wrong Answers");

                            // Add the table content to the PDF
                            foreach (var performance in UserPerformances)
                            {
                                table.AddCell(performance.CourseName);
                                table.AddCell(performance.TotalQuestions.ToString());
                                table.AddCell(performance.TotalAnswers.ToString());
                                table.AddCell(performance.CorrectAnswers.ToString());
                                table.AddCell(performance.WrongAnswers.ToString());
                            }

                            // Add the table to the document
                            document.Add(table);

                            document.Close();
                        }
                    }
                }

                return stream.ToArray();
            }
        }

    }
}
