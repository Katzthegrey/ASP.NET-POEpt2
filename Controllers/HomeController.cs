using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POEpt1.Models;
using POEpt1.Services;
using POEpt1.ViewModels;
using POEpt1.ViewModels.Claims;
using System.Diagnostics;

namespace POEpt1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContextClass _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContextClass context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> MonthlyClaimsCoordinator(string CoordinatorName, string statusFilter = "All")
        {
            var viewModel = new MonthlyClaimsCoordinatorViewModel
            {
                CoordinatorName = CoordinatorName,
                StatusFilter = statusFilter
            };

            // Query claims from database with user information
            var claimsQuery = _context.Claims
                .Include(c => c.User)
                .AsQueryable();

            // Apply status filter
            if (statusFilter != "All")
            {
                claimsQuery = claimsQuery.Where(c => c.Status == statusFilter);
            }

            var claimsWithUsers = await claimsQuery
                .OrderByDescending(c => c.ClaimDate)
                .ToListAsync();

            foreach (var claim in claimsWithUsers)
            {
                viewModel.Claims.Add(new CoordinatorClaimInfo
                {
                    ClaimId = claim.ClaimId,
                    UserID = claim.UserID,
                    UserName = claim.User.UserName,
                    ClaimDate = claim.ClaimDate,
                    Amount = claim.Amount,
                    ClaimStatus = claim.Status,
                    Description = claim.Description,
                    FileName = claim.FileName,
                    Email = claim.User.Email
                });
            }

            return View(viewModel);
        }

        // Manager View - Using database instead of ViewBags
        public async Task<IActionResult> MonthlyClaimManager(string ManagerName)
        {
            var viewModel = new MonthlyClaimsManagerViewModel
            {
                ManagerName = ManagerName
            };

            // Get approved claims from database with user information
            var approvedClaims = await _context.Claims
                .Include(c => c.User)
                .Where(c => c.Status == "Approved") // Only show claims approved by coordinators
                .OrderByDescending(c => c.ClaimDate)
                .ToListAsync();

            foreach (var claim in approvedClaims)
            {
                viewModel.Claims.Add(new ManagerClaimInfo
                {
                    ClaimId = claim.ClaimId,
                    UserID = claim.UserID,
                    StaffId = $"L{claim.UserID:D4}",
                    StaffName = claim.User.UserName,
                    CourseName = claim.Description, // Using Description as CourseName
                    ClaimDate = claim.ClaimDate,
                    TotalHours = CalculateHoursFromAmount(claim.Amount),
                    TotalAmount = claim.Amount,
                    FinalStatus = GetManagerStatus(claim), // Determine status for manager view
                    CoordinatorName = claim.Approver?.UserName ?? "Pending Coordinator"
                });
            }

            return View(viewModel);
        }

        // Approve Claim Action
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null)
                {
                    return NotFound();
                }

                claim.Status = "Approved";
                claim.ApprovedDate = DateTime.Now;
                // Note: You'll need to set claim.ApprovedBy to the current coordinator's UserID
                // claim.ApprovedBy = currentCoordinatorUserId;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Claim #{claimId} has been approved successfully.";
                return RedirectToAction(nameof(MonthlyClaimsCoordinator), new { CoordinatorName = User.Identity.Name });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving claim {ClaimId}", claimId);
                TempData["ErrorMessage"] = "Error approving claim. Please try again.";
                return RedirectToAction(nameof(MonthlyClaimsCoordinator), new { CoordinatorName = User.Identity.Name });
            }
        }

        // Reject Claim Action
        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null)
                {
                    return NotFound();
                }

                claim.Status = "Rejected";
                claim.ApprovedDate = DateTime.Now;
                // claim.ApprovedBy = currentCoordinatorUserId;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Claim #{claimId} has been rejected.";
                return RedirectToAction(nameof(MonthlyClaimsCoordinator), new { CoordinatorName = User.Identity.Name });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting claim {ClaimId}", claimId);
                TempData["ErrorMessage"] = "Error rejecting claim. Please try again.";
                return RedirectToAction(nameof(MonthlyClaimsCoordinator), new { CoordinatorName = User.Identity.Name });
            }
        }

        // View Claim Details Using ClaimDetailsVM
        public async Task<IActionResult> ClaimDetails(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.Approver)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
            {
                return NotFound();
            }

            var viewModel = new ClaimDetailsViewModel
            {
                ClaimID = claim.ClaimId,
                UserName = claim.User.UserName,
                ClaimDate = claim.ClaimDate,
                Amount = claim.Amount,
                Description = claim.Description,
                Status = claim.Status,
                FileName = claim.FileName,
                FileType = claim.FileType,
                FileSize = claim.FileSize,
                ApprovedByName = claim.Approver?.UserName ?? string.Empty,
                ApprovedDate = claim.ApprovedDate
            };

            return View(viewModel);
        }

        // Authorize Payment Action
        [HttpPost]
        public async Task<IActionResult> AuthorizePayment(int claimId)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null)
                {
                    return NotFound();
                }

                // Update claim status to "Paid"
                claim.Status = "Paid";
                // You might want to add additional fields like:
                // claim.PaidDate = DateTime.Now;
                // claim.PaidBy = currentManagerUserId;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Payment authorized for Claim #{claimId}. Status updated to 'Paid'.";
                return RedirectToAction(nameof(MonthlyClaimManager));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authorizing payment for claim {ClaimId}", claimId);
                TempData["ErrorMessage"] = "Error authorizing payment. Please try again.";
                return RedirectToAction(nameof(MonthlyClaimManager));
            }
        }

        // Place on Hold Action
        [HttpPost]
        public async Task<IActionResult> PlaceOnHold(int claimId)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null)
                {
                    return NotFound();
                }

                // Update claim status to "On Hold"
                claim.Status = "On Hold";
               

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Claim #{claimId} has been placed on hold.";
                return RedirectToAction(nameof(MonthlyClaimManager));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing claim {ClaimId} on hold", claimId);
                TempData["ErrorMessage"] = "Error placing claim on hold. Please try again.";
                return RedirectToAction(nameof(MonthlyClaimManager));
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Helper methods
        private decimal CalculateHoursFromAmount(decimal amount)
        {
            const decimal hourlyRate = 400m;
            return amount / hourlyRate;
        }

        private string GetManagerStatus(Claim claim)
        {
           
            if (claim.Amount > 10000m) 
                return "On Hold";
            else
                return "Ready for Payment";
        }
    }
}