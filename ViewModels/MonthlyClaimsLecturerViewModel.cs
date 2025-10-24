using POEpt1.ViewModels.Claims;

namespace POEpt1.ViewModels
{
    public class MonthlyClaimsLecturerViewModel
    {
        public string UserName { get; set; }
        public List<POEpt1.Models.Claim> Claims { get; set; } = new List<POEpt1.Models.Claim>();

        public decimal TotalHours { get; set; }

        public decimal TotalAmount { get; set; }

        public SubmitClaimViewModel NewClaim { get; set; } = new SubmitClaimViewModel();

    }
}
