using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POEpt1.Models;

namespace POEpt1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult MonthlyClaimsCoordinator(string CoordinatorName)
        {
            ViewBag.Name = CoordinatorName;
            //Viewbags for pouplating coordinators table
            ViewBag.ClaimIdList = new List<int> { 1042, 981, 875, 729, 1234 };
            ViewBag.StaffIdList = new List<string> { "L1042", "L1001", "L1005", "L1008", "L1000" };
            ViewBag.StaffNameList = new List<string> {
            "Denise Franklin",
            "Timmy Turner",
            "Will Smith",
            "Sarah Connor",
            "Robert Patrick"
    };
            ViewBag.ClaimDateList = new List<string> {
               "2025-03-25",
               "2025-02-28",
               "2025-02-20",
               "2025-01-31",
               "2025-04-01" };
            ViewBag.TotalHoursList = new List<decimal> { 15.5m, 12.0m, 18.0m, 14.5m, 29.0m};
            ViewBag.ClaimStatusList = new List<string> {
              "Pending",
              "Approved",
              "Rejected",
              "Pending",
              "Approved"
    };
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
