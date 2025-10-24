using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POEpt1.Models;
using POEpt1.Services;
using POEpt1.ViewModels;
using POEpt1.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using SecurityClaim = System.Security.Claims.Claim;
using Claim = POEpt1.Models.Claim;

namespace POEpt1.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContextClass _context;

        public LoginController(ApplicationDbContextClass context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl = null)
        {
            var viewModel = new SignInViewModel
            {
                ReturnUrl = returnUrl,
                AvailableRoles = GetAvailableRoles()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            var viewModel = new SignUpViewModel
            {
                AvailableRoles = GetAvailableRoles()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = GetAvailableRoles();
                return View(model);
            }

            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive == true);

            if (user != null)
            {
                // Verify password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.passwordHashed);

                if (!isPasswordValid)
                {
                    return InvalidCredentials(model);
                }

                // Verify role
                if (user.RoleID.ToString() != model.CustomRole)
                {
                    return InvalidRole(model);
                }

                await CreateAuthenticationCookie(user);

                return RedirectToDashboard(user);
            }
            else
            {
                return InvalidCredentials(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = GetAvailableRoles();
                return View(model);
            }

            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email address already registered");
                model.AvailableRoles = GetAvailableRoles();
                return View(model);
            }

            // Hash password and create user
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                passwordHashed = passwordHash,
                RoleID = int.Parse(model.CustomRole),
                CreatedDate = DateTime.Now,
                IsActive = true,
                Claims = new List<Claim>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account created successfully! Please sign in.";
            return RedirectToAction("SignIn");
        }
[HttpGet]
public async Task<IActionResult> MonthlyClaimsLecturer(string name)
{
    // Try multiple ways to get the username
    var userName = name ?? TempData["UserName"]?.ToString();
    
    if (string.IsNullOrEmpty(userName))
    {
        return RedirectToAction("SignIn");
    }

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.UserName == userName);

    if (user == null)
    {
        return RedirectToAction("SignIn");
    }

    var claims = await _context.Claims
        .Where(c => c.UserID == user.UserID)
        .OrderByDescending(c => c.ClaimDate)
        .ToListAsync();

    var viewModel = new MonthlyClaimsLecturerViewModel
    {
        UserName = user.UserName,
        Claims = claims,
        TotalHours = claims.Sum(c => c.Amount),
        TotalAmount = claims.Sum(c => c.Amount)
    };

    // Clear TempData after use
    TempData.Remove("UserName");
    
    return View(viewModel);
}

        // Helper methods
        private async Task CreateAuthenticationCookie(User user)
        {
            var claims = new List<SecurityClaim>
    {
        new SecurityClaim(ClaimTypes.Name, user.Email),
        new SecurityClaim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new SecurityClaim("UserName", user.UserName),
        new SecurityClaim("RoleID", user.RoleID.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
        private IActionResult InvalidCredentials(SignInViewModel model)
        {
            TempData["ErrorMessage"] = "Invalid email or password";
            model.AvailableRoles = GetAvailableRoles();
            return View("SignIn", model);
        }

        private IActionResult InvalidRole(SignInViewModel model)
        {
            TempData["ErrorMessage"] = "Role selection does not match your account";
            model.AvailableRoles = GetAvailableRoles();
            return View("SignIn", model);
        }

        private IActionResult RedirectToDashboard(User user)
        {
            var redirectData = new { name = user.UserName };

            return user.RoleID switch
            {
                2 => RedirectToAction("MonthlyClaimsCoordinator", "Home", redirectData), // Coordinator
                3 => RedirectToAction("MonthlyClaimManager", "Home", redirectData),      // Manager
                _ => RedirectToAction("MonthlyClaimsLecturer", redirectData)             // Lecturer (1)
            };
        }

        private List<SelectListItem> GetAvailableRoles()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Lecturer", Value = "1" },
                new SelectListItem { Text = "Coordinator", Value = "2" },
                new SelectListItem { Text = "Manager", Value = "3" }
            };
        }
    }
}
