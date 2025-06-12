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
    public class TypeOfOrderController : Controller
    {
        private readonly ApplicationDbContext db;
        public TypeOfOrderController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IActionResult Create()
        {

            //•	User navigates to /TypeOfOrder/Create → sees a form.
            var types = db.OrdersTypes.OrderByDescending(t => t.Id).ToList();
            ViewBag.Types = types;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TypeOfOrder type)
        {
            //•	It checks if the submitted model is valid 
            if (ModelState.IsValid)
            {
                db.OrdersTypes.Add(type);
                db.SaveChanges(); // Ensure DBContext is properly configured to support SaveChanges
                return RedirectToAction("Create"); // Redirect to show updated list
            }
            ViewBag.Types = db.OrdersTypes.OrderByDescending(t => t.Id).ToList();
            return View(type); //•	If the model is not valid, it returns the same view with the type model to display validation errors.
        }

        // GET: TypeOfOrder/Delete/5
        [HttpPost]
        public JsonResult AjaxDelete(int id)
        {
            var type = db.OrdersTypes.Find(id); // Adjust to data access
            if (type != null)
            {
                //•	It checks if the type exists in the database
                db.OrdersTypes.Remove(type); //•	If it exists, it removes the type from the database.
                db.SaveChanges(); //•	It saves the changes to the database.
                return Json(new { success = true, message = "Type deleted successfully." });
            }
            return Json(new { success = false, message = "Type not found." });
        }

        // POST: TypeOfOrder/Update/5
        [HttpPost]
        public JsonResult AjaxUpdate(int id, string name)
        {
            var type = db.OrdersTypes.Find(id);
            if (type != null)
            {
                type.Name = name;
                db.SaveChanges(); // Save changes to the database
                return Json(new { success = true, message = "Type updated successfully." });
            }
            return Json(new { success = false, message = "Type not found." });
        }

        }
}