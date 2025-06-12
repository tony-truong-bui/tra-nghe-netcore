using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TraNgheCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")] // Specifies that this controller is in the Admin area
    public class DashboardController : Controller
    {
        // GET: User/Dashboard
        public IActionResult Index()
        {
            return View();
        }
    }
}