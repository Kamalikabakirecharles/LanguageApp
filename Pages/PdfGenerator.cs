using System.Collections.Generic;
using System.IO;
using DinkToPdf;
using DinkToPdf.Contracts;
using LanguageApp.Pages;

namespace LanguageApp.Pages

{
    public class PdfGenerator
    {
        private readonly IConverter _converter;

        public PdfGenerator(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdf(List<leaderboardModel.UserPerformance> userPerformances)
        {
            var htmlContent = GenerateHtmlContent(userPerformances);

            var document = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4Plus,
            },
                Objects = {
                new ObjectSettings()
                {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                }
            }
            };

            return _converter.Convert(document);
        }

        private string GenerateHtmlContent(List<leaderboardModel.UserPerformance> userPerformances)
        {
            // Generate HTML content based on user performance data
            // You can customize the HTML structure here

            return "<html><body>Your HTML content here</body></html>";
        }
    }
}
