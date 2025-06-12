
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TraNgheCore.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    [Area("User")] // Specifies that this controller is part of the User area
    public class DashboardController : Controller
    {
        // GET: User/Dashboard
        public IActionResult Index()
        {
            return View();
        }
    }
}