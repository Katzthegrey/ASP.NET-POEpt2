using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POEpt1.Models;
using POEpt1.Services;
using POEpt1.ViewModels;
using POEpt1.ViewModels.Claims;
using System.Security.Claims;

namespace POEpt1.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContextClass _context;
        private readonly IWebHostEnvironment _environment;

        public ClaimsController(ApplicationDbContextClass context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Content("Claims controller is working!");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubmitClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Get the authenticated user's email
                var userEmail = User.Identity.Name;

                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Please sign in to submit a claim.";
                    return RedirectToAction("SignIn", "Login");
                }

                // Find user by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    // Try by username as fallback
                    user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userEmail);

                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "User not found. Please sign in again.";
                        return RedirectToAction("SignIn", "Login");
                    }
                }

                // Automation validation - Check business rules
                var businessErrors = ValidateClaimBusinessRules(model);
                if (businessErrors.Any())
                {
                    foreach (var error in businessErrors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }

                // Determine if claim can be auto-approved
                bool isAutoApproved = CanAutoApproveClaim(model);
                string autoApproveNote = isAutoApproved ? " [Auto-Approved]" : "";

                // Handle file upload
                string storedFileName = null;
                string filePath = null;
                string relativePath = null;
                string originalFileName = null;
                string fileType = "application/octet-stream";
                long fileSize = 0;

                if (model.ProofDocument != null && model.ProofDocument.Length > 0)
                {
                    if (model.ProofDocument.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ProofDocument", "File size must be less than 5MB.");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "claims");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    originalFileName = Path.GetFileName(model.ProofDocument.FileName);
                    storedFileName = $"{Guid.NewGuid()}_{originalFileName}";
                    filePath = Path.Combine(uploadsFolder, storedFileName);
                    relativePath = $"/uploads/claims/{storedFileName}";
                    fileType = model.ProofDocument.ContentType;
                    fileSize = model.ProofDocument.Length;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProofDocument.CopyToAsync(stream);
                    }
                }
                else
                {
                    originalFileName = "No file uploaded";
                    storedFileName = "no_file";
                    relativePath = "/uploads/claims/no_file";
                    fileType = "text/plain";
                }

                // Create new claim
                var claim = new POEpt1.Models.Claim
                {
                    UserID = user.UserID,
                    User = user,
                    Amount = model.Amount,                 
                    Description = $"{model.Description} | {model.HoursWorked}h @ R{model.HourlyRate}/h{autoApproveNote}",
                    //  Auto-approve if criteria met
                    Status = isAutoApproved ? "Approved" : "Pending",
                    ClaimDate = DateTime.Now,
                    FileName = originalFileName,
                    StoredFileName = storedFileName,
                    FilePath = relativePath,
                    FileSize = fileSize,
                    FileType = fileType,
                    
                    ApprovedBy = isAutoApproved ? null : null,
                    Approver = null,
                    ApprovedDate = isAutoApproved ? DateTime.Now : null
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

              // success message
                TempData["SuccessMessage"] = isAutoApproved
                    ? $"Claim submitted and auto-approved! {model.HoursWorked}h × R{model.HourlyRate} = R{model.Amount:F2}"
                    : "Claim submitted successfully! Waiting for coordinator review.";

                return RedirectToAction("MonthlyClaimsLecturer", "Login", new { name = user.UserName });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while submitting your claim: {ex.Message}";
                return View(model);
            }
        }

       // Helper method for business rule validation
        public List<string> ValidateClaimBusinessRules(SubmitClaimViewModel model)
        {
            var errors = new List<string>();

            // Check if amount matches calculation
            decimal calculatedAmount = model.HoursWorked * model.HourlyRate;
            if (Math.Abs(model.Amount - calculatedAmount) > 0.01m)
            {
                errors.Add($"Amount (R{model.Amount:F2}) doesn't match calculation: {model.HoursWorked}h × R{model.HourlyRate}/h = R{calculatedAmount:F2}");
            }

            // Check maximum hours per day
            if (model.HoursWorked > 12)
            {
                errors.Add("Maximum 12 hours allowed per claim.");
            }

            // Check if claim date is in future
            if (model.ClaimDate > DateTime.Today)
            {
                errors.Add("Claim date cannot be in the future.");
            }

            // Check hourly rate range
            if (model.HourlyRate < 100 || model.HourlyRate > 1000)
            {
                errors.Add("Hourly rate must be between R100 and R1000.");
            }

            return errors;
        }

     
        public bool CanAutoApproveClaim(SubmitClaimViewModel model)
        {
            // Auto-approve if:
            // Amount matches calculation
            decimal calculatedAmount = model.HoursWorked * model.HourlyRate;
            bool amountMatches = Math.Abs(model.Amount - calculatedAmount) < 0.01m;

            //  Claim is under R5000
            bool underAutoApproveLimit = model.Amount <= 5000;

            // Hours are reasonable
            bool reasonableHours = model.HoursWorked <= 8;

            //  Not a future date
            bool validDate = model.ClaimDate <= DateTime.Today;

            return amountMatches && underAutoApproveLimit && reasonableHours && validDate;
        }
        // method to handle file downloads
        public async Task<IActionResult> DownloadFile(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null || string.IsNullOrEmpty(claim.StoredFileName))
            {
                return NotFound();
            }

            var path = Path.Combine(_environment.WebRootPath, "uploads", "claims", claim.StoredFileName);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, claim.FileType, claim.FileName);
        }


    }
}