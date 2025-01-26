using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.BLL.DTOs
{
    public class DocumentDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Filename { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
