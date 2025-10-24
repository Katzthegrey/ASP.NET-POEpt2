using POEpt1.Models;

namespace POEpt1.ViewModels.Claims
{
    public class ClaimsHistoryViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<POEpt1.Models.Claim> Claims { get; set; } = new List<POEpt1.Models.Claim>();

        // Calculated properties for the view
        public decimal TotalAmount => Claims.Sum(c => c.Amount);
        public int TotalClaims => Claims.Count;
        public int PendingClaims => Claims.Count(c => c.Status == "Pending");
        public int ApprovedClaims => Claims.Count(c => c.Status == "Approved");
    }
}
