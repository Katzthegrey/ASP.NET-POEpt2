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
            if (Roles == "2") // Coordinator
            {
                return RedirectToAction("MonthlyClaimsCoordinator", "Home", new { CoordinatorName = Name });
            }
            else if(Roles == "3")
            {
                return RedirectToAction("MonthlyClaimManager", "Home", new { ManagerName = Name });
            }
            else // Lecturer
            {
                return RedirectToAction("MonthlyClaimsLecturer", new { Name = Name, Password = Password });
            }
            }
        public IActionResult MonthlyClaimsLecturer(string Name, string Password)
        {
         
            ViewBag.Name = Name; 
            ViewBag.pass = Password;
            // Using ViewBags List Generic for table data
            ViewBag.ClaimIdList = new List<int> { 1042, 981, 875, 729 };
            ViewBag.ClaimDateList = new List<string> {
               "2025-03-25",
               "2025-02-28",
               "2025-02-20",
               "2025-01-31"
               };
            ViewBag.CourseList = new List<string> {
                "BSc Computer Science",
                "BSc Computer Science",
                "BSc Data Science",
                "BSc Computer Science"
    };
            ViewBag.HourlyRateList = new List<decimal> { 400.00m, 400.00m, 400.00m, 400.00m }; // Same rate for all their claims
            ViewBag.HoursWorkedList = new List<decimal> { 10.0m, 12.5m, 8.5m, 15.0m };
            ViewBag.ClaimStatusList = new List<string> {
            "Submitted",  
            "Approved",   
            "Paid",       
            "Rejected"   
    };

            // Calculate the total hours for all claims
            ViewBag.TotalHours = ((List<decimal>)ViewBag.HoursWorkedList).Sum();
            return View();
        }
        
    }
}
