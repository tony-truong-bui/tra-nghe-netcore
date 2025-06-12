
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TraNgheCore.Models;
using Microsoft.AspNetCore.Authorization;



namespace TraNgheCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext db;
        public ProductController(ApplicationDbContext _db)
        {
            db = _db;
        }

        // GET: Products/Create

        /// <summary>
        /// Displays the product creation form and the latest products list.
        /// </summary>
        public IActionResult Create() //•	User navigates to /Product/Create → sees a form.
        {
            var categories = db.Categories.Select(c => new { c.Id, c.Name }).ToList();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");



            var products = db.Products.OrderByDescending(p => p.Id).ToList();
            ViewBag.Products = products;
            ViewBag.Categories = db.Categories.OrderByDescending(c => c.Id).ToList();
            return View();
        }

        /// <summary>
        /// Handles the POST request to create a new product.
        /// </summary>
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductModel product)
        {
            if (ModelState.IsValid) //•	It checks if the submitted model is valid 
            {
                //if (ImageFile != null && ImageFile.ContentLength > 0)
                //{
                //    //•	It checks if an image file is uploaded
                //    var fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                //    var path = System.IO.Path.Combine(Server.MapPath("~/Images/Products"), fileName);
                //    ImageFile.SaveAs(path); // Save the image to the server
                //    product.ImageUrl = "/Images/Products/" + fileName; // Set the image URL in the product model
                //}
                db.Products.Add(product);
                db.SaveChanges(); // Ensure DBContext is properly configured to support SaveChanges
                return RedirectToAction("Create"); // Redirect to show updated list

            }
            ViewBag.Products = db.Products.OrderByDescending(p => p.Id).ToList();
            ViewBag.Categories = db.Categories.ToList();

            return View(product); //•	If the model is not valid, it returns the same view with the product model to display validation errors.
        }

        /// <summary>
        /// Handles AJAX request to delete a product by ID.
        /// </summary>
        [HttpPost]
        public JsonResult AjaxDelete(int id)
        {
            try
            {
                var product = db.Products.Find(id); // Adjust to data access
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }

                // 🔍 CHECK: Are there any order items using this product?
                var referencingOrderItems = db.OrderItems.Where(oi => oi.ProductId == id).ToList();


                if (product != null)
                {
                    if (referencingOrderItems.Any())
                    {
                        // Get unique order count
                        var orderCount = referencingOrderItems.Select(oi => oi.OrderId).Distinct().Count();

                        // ❌ Cannot delete - show order details
                        var message = $"Cannot delete product '{product.Name}'. " +
                                     $"It has been ordered {referencingOrderItems.Count} time(s) " +
                                     $"across {orderCount} order(s).";

                        return Json(new
                        {
                            success = false,
                            message = message,
                            orderCount = orderCount,
                            orderItemCount = referencingOrderItems.Count
                        });
                    }



                    // ✅ Safe to delete - no references found
                    db.Products.Remove(product); //•	If it exists, it removes the product from the database.
                    db.SaveChanges(); //•	It saves the changes to the database.
                    return Json(new
                    {
                        success = true,
                        message = $"Product '{product.Name}' deleted successfully."
                    });
                }
                return Json(new { success = false, message = "Product not found." });
            } catch (Exception ex)
            {
                // Log the exception (not shown here)
                return Json(new { success = false, message = "An error occurred while deleting the product: " + ex.Message });
            }
        }
        


        /// <summary>
        /// Handles AJAX request to update product details, including category, in the product list table.
        /// </summary>
        [HttpPost]
        public JsonResult AjaxUpdate(int id, string name, string description, string price, bool isServed, int category)
        {
            var product = db.Products.Find(id); // Adjust to data access
            if (product != null)
            {
                //•	It checks if the product exists in the database
                product.Name = name;
                product.Price = decimal.Parse(price);
                product.Description = description;
                product.Category = category; // Update the product's category
                product.IsServed = isServed; // Update the product's properties
                db.SaveChanges(); // Save changes to the database
                return Json(new { success = true, message = "Product updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Product not found." });
            }
        }
    }
}