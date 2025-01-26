using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.OCR
{
    public interface IOcrClient
    {
        string OcrPdf(Stream pdfStream);
    }
}
