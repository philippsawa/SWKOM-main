using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.DAL.Entities
{
    public class Document
    {
        public string Id { get; set; } = String.Empty;
        public required string Filename { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
