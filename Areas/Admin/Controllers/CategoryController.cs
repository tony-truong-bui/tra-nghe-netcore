
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraNgheCore.Models;


namespace TraNgheCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")] // Specifies this controller is in the Admin area
    public class CategoryController : Controller
    {
        // 📊 Database context - handles Entity Framework operations
        private readonly ApplicationDbContext db;
        public CategoryController(ApplicationDbContext _db)
        {
            db = _db;
        }

        // GET: Categories/Create

        // 📝 GET: Display create form with existing categories list
        public IActionResult Create() //•	User navigates to /Category/Create → sees a form.
        {
            // Load all categories ordered by newest first (descending ID)
            var categories = db.Categories.OrderByDescending(c => c.Id).ToList();

            // Pass categories to view via ViewBag for display in UI
            ViewBag.Categories = categories;
            return View();
        }
        // POST: Categories/Create
        // 💾 POST: Handle form submission to create new category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel category)
        {
            // ✅ Server - side validation check
            if (ModelState.IsValid) //•	It checks if the submitted model is valid 
            {
                db.Categories.Add(category);
                db.SaveChanges(); // Ensure DBContext is properly configured to support SaveChanges

                // 🔄 Redirect to same page (PRG pattern - Post-Redirect-Get)
                // This prevents duplicate submissions on page refresh
                return RedirectToAction("Create"); // Redirect to show updated list
            }
            // ❌ If validation fails, reload categories list and return form with errors
            ViewBag.Categories = db.Categories.OrderByDescending(c => c.Id).ToList();
            return View(category); //•	If the model is not valid, it returns the same view with the category model to display validation errors.
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int id)
        {
            var category = db.Categories.Find(id); // Adjust to data access
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        // AJAX: Delete a category
        [HttpPost]
        public JsonResult AjaxDelete(int id)
        {
            try
            {
                var category = db.Categories.Find(id);

                if (category == null)
                {
                    return Json(new { success = false, message = "Category not found." });
                }

                // 🔍 CHECK: Are there any products using this category?
                var referencingProducts = db.Products.Where(p => p.Category == id).ToList();


                if (category != null)
                {
                    if (referencingProducts.Any())
                    {
                        // ❌ Cannot delete - show which products are using it
                        var productNames = referencingProducts.Select(p => p.Name).Take(5).ToList();
                        var message = $"Cannot delete category '{category.Name}'. " +
                                     $"It is being used by {referencingProducts.Count} product(s): " +
                                     $"{string.Join(", ", productNames)}" +
                                     (referencingProducts.Count > 5 ? "..." : "");


                        return Json(new
                        {
                            success = false,
                            message = message,
                            referencingCount = referencingProducts.Count
                        });
                    }

                    // ✅ Safe to delete - no references found
                    db.Categories.Remove(category);
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = $"Category '{category.Name}' deleted successfully."
                    });
                }
                return Json(new { success = false, message = "Category not found." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while deleting: " + ex.Message
                });
            }
        }

        // POST: Category/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var category = db.Categories.Find(id);
        //    if (category != null)
        //    {
        //        db.Categories.Remove(category);
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public JsonResult AjaxUpdate(int id, string name)
        {
            var category = db.Categories.Find(id);
            if (category != null)
            {
                // Here you can update the category properties as needed
                // For example, category.Name = "Updated Name";
                category.Name = name; // Update the name with the provided value
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Category not found." });
        }
    }
}