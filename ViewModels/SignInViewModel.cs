using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POEpt1.ViewModels.Account
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        [Display(Name = "Role (Optional)")]
        public string CustomRole { get; set; } = string.Empty;

        public List<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>
        {
        new SelectListItem { Text = "Lecturer", Value = "1" },      
        new SelectListItem { Text = "Coordinator", Value = "2" },  
        new SelectListItem { Text = "Manager", Value = "3" }
        };

        public string ReturnUrl { get; set; } = string.Empty;
    }
}
