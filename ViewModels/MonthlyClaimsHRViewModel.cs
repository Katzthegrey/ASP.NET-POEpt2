using System.ComponentModel.DataAnnotations;

namespace POEpt1.ViewModels

{
    public class MonthlyClaimsHRViewModel
    {
        public string HRName { get; set; }
        public List<HRClaimInfo> Claims { get; set; } = new List<HRClaimInfo>();

        // Reporting filters
        public string StatusFilter { get; set; } = "Approved"; // Default to approved claims
        public string PeriodFilter { get; set; } = "CurrentMonth"; // CurrentMonth, LastMonth, Custom
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Summary statistics
        public decimal TotalApprovedAmount { get; set; }
        public int TotalClaimsCount { get; set; }
        public int ReadyForPaymentCount { get; set; }

        public List<string> AvailableStatuses => new List<string> { "All", "Approved", "Paid", "On Hold" };
        public List<string> AvailablePeriods => new List<string> { "CurrentMonth", "LastMonth", "Custom" };
    }

    public class HRClaimInfo
    {
        public int ClaimId { get; set; }
        public int UserID { get; set; }

        [Display(Name = "Staff ID")]
        public string StaffId { get; set; }

        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Coordinator")]
        public string CoordinatorName { get; set; }

        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }

        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Status")]
        public string FinalStatus { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedByName { get; set; }

        // New fields for payment processing
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Payment Ready")]
        public bool IsPaymentReady => FinalStatus == "Approved";

        [Display(Name = "Days Since Approval")]
        public int DaysSinceApproval => ApprovedDate.HasValue ?
            (DateTime.Now - ApprovedDate.Value).Days : 0;
    }
}