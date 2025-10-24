using POEpt1.Models;
using System.ComponentModel.DataAnnotations;

namespace POEpt1.ViewModels
{
    public class MonthlyClaimsCoordinatorViewModel
    {
        public string CoordinatorName { get; set; }
        public List<CoordinatorClaimInfo> Claims { get; set; } = new List<CoordinatorClaimInfo>();

        // For filtering
        public string StatusFilter { get; set; } = "All";

        [Display(Name = "Filter by Status")]
        public List<string> AvailableStatuses { get; set; } = new List<string>
        {
            "All", "Pending", "Approved", "Rejected"
        };
    }

    public class CoordinatorClaimInfo
    {
        public int ClaimId { get; set; }
        public int UserID { get; set; }

        [Display(Name = "Staff ID")]
        public string StaffId => $"L{UserID:D4}";

        [Display(Name = "Lecturer Name")]
        public string UserName { get; set; }  

        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }

        [Display(Name = "Total Hours")]
        public decimal TotalHours => CalculateHoursFromAmount(Amount);

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Status")]
        public string ClaimStatus { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        private decimal CalculateHoursFromAmount(decimal amount)
        {
            // Assuming R400 per hour rate - you can make this configurable
            const decimal hourlyRate = 400m;
            return amount / hourlyRate;
        }
    }
}