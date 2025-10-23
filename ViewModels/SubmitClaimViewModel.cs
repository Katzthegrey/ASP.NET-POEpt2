using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace POEpt1.ViewModels.Claims
{
    public class SubmitClaimViewModel
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Claim Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please upload a proof document")]
        [FileExtensions(Extensions = "pdf,doc,docx,jpg,jpeg,png",
                       ErrorMessage = "Only PDF, Word, JPG, and PNG files are allowed")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size cannot exceed 5MB")]
        [Display(Name = "Proof Document")]
        public IFormFile ProofDocument { get; set; }
    }

    // Custom validation attribute for file size
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxFileSize;

        public MaxFileSizeAttribute(long maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}