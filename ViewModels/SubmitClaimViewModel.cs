using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace POEpt1.ViewModels.Claims
{
    public class SubmitClaimViewModel
    {
      
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Total Amount (R)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please upload a proof document")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size cannot exceed 5MB")]
        [Display(Name = "Proof Document")]
        public IFormFile ProofDocument { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0.5, 12.0, ErrorMessage = "Hours must be between 0.5 and 12")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(100, 1000, ErrorMessage = "Hourly rate must be between R100 and R1000")]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; } = 400m;

        [Required]
        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; } = DateTime.Today;

        // Calculated properties (read-only for validation)
        public decimal CalculatedAmount => HoursWorked * HourlyRate;

        public bool AmountMatches => Math.Abs(Amount - CalculatedAmount) < 0.01m;

        public string AmountValidationMessage =>
            AmountMatches ? "Amount matches calculation" :
            $"Amount should be R{CalculatedAmount:F2} based on {HoursWorked}h × R{HourlyRate}/h";

        public bool IsWithinAutoApproveLimit => Amount <= 5000;

        public string AutoApproveMessage =>
            IsWithinAutoApproveLimit ? "Eligible for auto-approval" : "Requires manual review";

        // Validation method
        public List<string> ValidateBusinessRules()
        {
            var errors = new List<string>();

            if (!AmountMatches)
            {
                errors.Add($"Amount (R{Amount:F2}) doesn't match calculated amount (R{CalculatedAmount:F2})");
            }

            if (HoursWorked > 12)
            {
                errors.Add("Maximum 12 hours allowed per claim");
            }
            // Validating if claim is made in the future
            if (ClaimDate > DateTime.Today)
            {
                errors.Add("Claim date cannot be in the future");
            }

            return errors;
        }
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