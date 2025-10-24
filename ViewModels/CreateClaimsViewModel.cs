using System.ComponentModel.DataAnnotations;

namespace POEpt1.ViewModels
{
    public class CreateClaimsViewModel
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Claim Amount")]
        public decimal Amount { get; set; } 

        [Required(ErrorMessage = "Description detail is required")]
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        [Display(Name = "Description / Course Name")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a file")]
        [Display(Name = "Supporting Document")]
        public IFormFile ClaimFile { get; set; }


    }
}
