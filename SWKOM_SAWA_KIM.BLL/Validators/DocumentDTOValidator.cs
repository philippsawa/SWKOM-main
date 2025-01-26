using FluentValidation;
using SWKOM_SAWA_KIM.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.BLL.Validators
{
    public class DocumentDTOValidator : AbstractValidator<DocumentDTO>
    {
        public DocumentDTOValidator()
        {
            RuleFor(doc => doc.Filename)
                .NotEmpty().WithMessage("Filename is required")
                .MaximumLength(100).WithMessage("Filename must not exceed 100 characters")
                .Must(filename => filename.ToLower().EndsWith(".pdf")).WithMessage("Filename must end with .pdf");

            RuleFor(doc => doc.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future");
        }
    }
}
