using System.ComponentModel.DataAnnotations;

namespace POEpt1.ViewModels
{
    public class MonthlyClaimsManagerViewModel
    {
        public string ManagerName { get; set; }
        public List<ManagerClaimInfo> Claims { get; set; } = new List<ManagerClaimInfo>();
    }

    public class ManagerClaimInfo
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

        [Display(Name = "Final Status")]
        public string FinalStatus { get; set; }
    }
}