using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TraNgheCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        public IActionResult Index()
        {
            return View();
        }


    }
}