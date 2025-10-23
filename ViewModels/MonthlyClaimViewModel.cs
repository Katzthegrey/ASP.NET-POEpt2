using POEpt1.Models;

namespace POEpt1.ViewModels.Account
{
    public class MonthlyClaimsViewModel
    {
        public string Name { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public decimal TotalHours { get; set; }
    }
}
