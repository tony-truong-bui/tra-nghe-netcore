using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraNgheCore.Models;

namespace TraNgheCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OrderStatusController : Controller
    {
        private readonly ApplicationDbContext db;
        public OrderStatusController(ApplicationDbContext _db)
        {
            db = _db;
        }
        // GET: Admin/OrderStatus
        public IActionResult Create()
        {

            var statuses = db.OrderStatuses.OrderByDescending(s => s.Id).ToList();
            ViewBag.Statuses = statuses; //•	User navigates to /OrderStatus/Create → sees a form.
            return View();
        }

        public IActionResult Create(OrderStatusModel status)
        {
            //•	It checks if the submitted model is valid 
            if (ModelState.IsValid)
            {
                db.OrderStatuses.Add(status);
                db.SaveChanges(); // Ensure DBContext is properly configured to support SaveChanges
                return RedirectToAction("Create"); // Redirect to show updated list
            }
            ViewBag.Statuses = db.OrderStatuses.OrderByDescending(s => s.Id).ToList();
            return View(status); //•	If the model is not valid, it returns the same view with the status model to display validation errors.
        }

        [HttpPost]
        public JsonResult AjaxDelete(int id)
        {
            var status = db.OrderStatuses.Find(id); // Adjust to data access
            if (status != null)
            {
                //•	It checks if the status exists in the database
                db.OrderStatuses.Remove(status); //•	If it exists, it removes the status from the database.
                db.SaveChanges(); //•	It saves the changes to the database.
                return Json(new { success = true, message = "Status deleted successfully." });
            }
            return Json(new { success = false, message = "Status not found." });
        }

        [HttpPost]
        public JsonResult AjaxUpdate(int id, string name)
        {
            var status = db.OrderStatuses.Find(id);
            if (status != null)
            {
                //•	It checks if the status exists in the database
                status.Name = name; // Update the status's name
                db.SaveChanges(); // Save changes to the database
                return Json(new { success = true, message = "Status updated successfully." });
            }
            return Json(new { success = false, message = "Status not found." });
        }
    }
}