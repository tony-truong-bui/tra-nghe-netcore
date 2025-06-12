using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace TraNgheCore.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    [Area("User")] // Specifies that this controller is part of the User area
    public class UserController : Controller
    {
        // GET: User/User
        public ActionResult Index()
        {
            return View();
        }


    }
}