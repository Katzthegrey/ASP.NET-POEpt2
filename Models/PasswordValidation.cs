using System.ComponentModel.DataAnnotations;

namespace POEpt1.Models
{
    public class PasswordValidation
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength (100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character")]
        public required string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }

        [Required]
        public required string Roles { get; set; }
    }
}
