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
                    Description = model.Description,
                    Status = "Pending",
                    ClaimDate = DateTime.Now,
                    FileName = originalFileName,
                    StoredFileName = storedFileName,
                    FilePath = relativePath,
                    FileSize = fileSize,
                    FileType = fileType,
                    ApprovedBy = null,
                    Approver = null,
                    ApprovedDate = null
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("MonthlyClaimsLecturer", "Login", new { name = user.UserName });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while submitting your claim: {ex.Message}";
                return View(model);
            }
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