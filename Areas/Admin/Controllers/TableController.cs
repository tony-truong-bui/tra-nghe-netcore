
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraNgheCore.Models; // Ensure this namespace matches your project structure

namespace TraNgheCore.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class TableController : Controller
    {
        private readonly ApplicationDbContext db;
        public TableController(ApplicationDbContext _db)
        {
            db = _db;
        }

        // GET: Table/Create
        public IActionResult Create() //•	User navigates to /Table/Create → sees a form.
        {
            var tables = db.Tables.OrderByDescending(t => t.Id).ToList();
            ViewBag.Tables = tables;
            return View();
        }

        // POST: Table/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TableModel table)
        {
            if (ModelState.IsValid) //•	It checks if the submitted model is valid 
            {
                db.Tables.Add(table);
                db.SaveChanges(); // Ensure DBContext is properly configured to support SaveChanges
                return RedirectToAction("Create"); // Redirect to show updated list
            }
            ViewBag.Tables = db.Tables.OrderByDescending(t => t.Id).ToList();
            return View(table); //•	If the model is not valid, it returns the same view with the table model to display validation errors.
        }

        [HttpPost]
        public JsonResult AjaxDelete(int id)
        {
            var table = db.Tables.Find(id); // Adjust to data access

            var referencingOrders = db.Orders.Where(o => o.TableId == id).ToList(); //•	It checks if there are any orders associated with the table being deleted.
            if (table != null)
            {
                //•	It checks if the table exists in the database
                db.Tables.Remove(table); //•	If it exists, it removes the table from the database.
                db.SaveChanges(); //•	It saves the changes to the database.
                return Json(new { success = true, message = "Table deleted successfully." });
            }
            return Json(new { success = false, message = "Table not found." });
        }

        [HttpPost]
        public JsonResult AjaxUpdate(int id, string name, bool isAvailable)
        {
            var table = db.Tables.Find(id); // Adjust to data access
            if (table != null)
            {
                //•	It checks if the table exists in the database
                table.Name = name;
                table.IsAvailable = isAvailable;// Update the table's name
                db.SaveChanges(); // Save changes to the database
                return Json(new { success = true, message = "Table updated successfully." });
            }
            return Json(new { success = false, message = "Table not found." });
        }

    }
}