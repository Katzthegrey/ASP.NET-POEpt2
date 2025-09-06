using Microsoft.AspNetCore.Mvc;

namespace POEpt1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignIn(string Name, string Password, string Email, string Roles)
        {
            // Store the signup details in ViewBag to pre-populate the SignIn form
            ViewBag.SignUpName = Name;
            ViewBag.SignUpPassword = Password;
            ViewBag.SignUpEmail = Email;
            ViewBag.SignUpRole = Roles;

            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult ValidateSignIn(string Name, string Password, string Roles)
        {
            // Check if both fields are empty
            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Password))
            {
                ViewBag.ErrorMessage = "Please enter both username and password";
                return View("SignIn");
            }

            // Check if only Name is empty
            if (string.IsNullOrEmpty(Name))
            {
                ViewBag.ErrorMessage = "Please enter username";
                return View("SignIn");
            }

            // Check if only Password is empty
            if (string.IsNullOrEmpty(Password))
            {
                ViewBag.ErrorMessage = "Please enter password";
                return View("SignIn");
            }
            // Redirect based on role
            if (Roles == "2") // Lecturer
            {
                return RedirectToAction("MonthlyClaimsLecturer");
            }
            else // Student
            {
                return RedirectToAction("MonthlyClaimsStudent", new { Name = Name, Password = Password });
            }
            }
        public IActionResult MonthlyClaimsStudent(string Name, string Password)
        {
         
            ViewBag.Name = Name; 
            ViewBag.pass = Password;
            return View();
        }
        public IActionResult MonthlyClaimsLecturer()
        {
            return View();
        }
    }
}
