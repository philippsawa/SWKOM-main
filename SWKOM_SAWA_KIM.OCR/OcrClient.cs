using ImageMagick;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace SWKOM_SAWA_KIM.OCR
{
    public class OcrClient : IOcrClient
    {
        private readonly string tessDataPath;
        private readonly string language;
        private ILogger<OcrClient> _logger;

        public OcrClient(IOptions<OcrOptions> options, ILogger<OcrClient> logger)
        {
            this.tessDataPath = options.Value.TessDataPath;
            this.language = options.Value.Language;
            _logger = logger;
        }

        public string OcrPdf(Stream pdfStream)
        {
            _logger.LogInformation("Starting OCR on PDF");

            var stringBuilder = new StringBuilder();

            using (var magickImages = new MagickImageCollection())
            {
                magickImages.Read(pdfStream);
                foreach (var magickImage in magickImages)
                {
                    // Set the resolution and format of the image (adjust as needed)
                    magickImage.Density = new Density(300, 300);
                    magickImage.Format = MagickFormat.Png;

                    // Perform OCR on the image
                    using (var tesseractEngine = new TesseractEngine(tessDataPath, language, EngineMode.Default))
                    {
                        using (var page = tesseractEngine.Process(Pix.LoadFromMemory(magickImage.ToByteArray())))
                        {
                            var extractedText = page.GetText();
                            stringBuilder.Append(extractedText);
                        }
                    }
                }
            }


            return stringBuilder.ToString();
        }
    }
}
