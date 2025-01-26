using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.OCR
{
    public class OcrOptions
    {
        public const string OCR = "OCR";

        public string Language { get; set; } = "eng";
        public string TessDataPath { get; set; } = "./tessdata";
    }
}
