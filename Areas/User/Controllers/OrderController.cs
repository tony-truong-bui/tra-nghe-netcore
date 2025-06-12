
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TraNgheCore.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;



namespace TraNgheCore.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    [Area("User")]
    public class OrderController : Controller
    {


        private readonly ApplicationDbContext db;
        public OrderController(ApplicationDbContext _db)
        {
            db = _db;
        }

        // 📝 GET: Display order creation form for customers
        // URL: /User/Order/Create
        public IActionResult Create(int index)
        {
            // Initialize empty ViewModel for new order
            var model = new CreateOrderViewModel();

            // Load available products for dropdown selection
            // Only shows products that are currently being served
            ViewBag.Products = GetProductSelectList();

            //Load available tables for dine-in orders
            // Only shows tables that are currently available
            ViewBag.Tables = GetTableSelectList();

            ViewBag.TypesOfOrders = GetTypeOfOdersSelectList();

            // Render the partial with the correct prefix for model binding
            var templateInfo = new TemplateInfo { HtmlFieldPrefix = $"Orders[{index}]" };
            ViewData["TemplateInfo"] = templateInfo;

            return PartialView("_CreateOrderForm", model);
        }

        // 💾 POST: Process customer's order submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateOrderViewModel model)
        {
            // ✅ Validation: Check model is valid AND has at least one item
            if (ModelState.IsValid && model.OrderItems.Any())
            {
                try
                {
                    // Get the current user's ID (string)
                    string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    // 🏗️ STEP 1: Create the main order record
                    var order = new OrderModel
                    {
                        CustomerName = model.CustomerName,
                        CustomerPhone = model.CustomerPhone,
                        CustomerAddress = model.CustomerAddress,
                        OrderDate = DateTime.Now,
                        TotalPrice = model.OrderItems.Sum(item => item.Price * item.Quantity),
                        TypeOfOrder = model.TypeOfOrder, // Default to 0 if not specified'
                        OrderStatus = 0, // Default status - Creating
                        UserId = userId,

                        TableId = model.TableId ?? default(int), // Nullable for dine-in orders  
                        //TableId = (int)(model.TableId.HasValue ? model.TableId.Value : (int?) null), // Nullable for dine-in orders

                    };
                    db.Orders.Add(order);
                    db.SaveChanges(); // Get the Order ID

                    // 🏗️ STEP 2: Create individual order items
                    foreach (var item in model.OrderItems)
                    {
                        var orderItem = new OrderItemModel
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            Price = item.Price,
                            Quantity = item.Quantity
                        };
                        db.OrderItems.Add(orderItem);
                    }
                    // 🔄 Update status to 2 - Complete after saving items
                    order.OrderStatus = 2; // 2 - Complete
                    db.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();

                    // ✅ Success feedback to user
                    TempData["Success"] = "Order created successfully!";

                    // 🔄 Redirect to order details page (PRG pattern)
                    return RedirectToAction("Details", new { id = order.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while creating the order: " + ex.Message);
                }
            }
            // 🔄 If validation fails or error occurs, reload form with products
            ViewBag.Products = GetProductSelectList();
            return View(model);
        }

        // ⚡ AJAX: Fetch product details for dynamic form updates
        // AJAX: Get product details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetProductDetails(int productId)
        {
            var product = db.Products.Find(productId);
            if (product != null)
            {
                // ✅ Return product data as JSON for frontend JavaScript
                return Json(new
                {
                    success = true,
                    name = product.Name,
                    price = product.Price
                });
            }
            return Json(new { success = false });
        }

        // 🛠️ Helper: Create dropdown list of available products
        private SelectList GetProductSelectList()
        {
            return new SelectList(
                // 📋 Query: Only products that are currently being served
                db.Products.Where(p => p.IsServed).ToList(),
                "Id",       // Value field (what gets submitted)
                "Name",     // Text field (what user sees)
                null        // Selected value (none by default)
            );
        }

        //Create dropdown list of available tables
        private SelectList GetTableSelectList()
        {
            return new SelectList(
                db.Tables.Where(t => t.IsAvailable).ToList(), // Only available tables
                "Id",       // Value field (what gets submitted)
                "Name"      // Text field (what user sees)
            );
        }

        //Create dropdown list of types of orders
        private SelectList GetTypeOfOdersSelectList()
        {
            return new SelectList(
                db.OrdersTypes.Where(to => to.Id > 0).ToList(),
                "Id",       // Value field (what gets submitted)
                "Name"      // Text field (what user sees)
            );
        }
        // 🧹 Resource cleanup: Properly dispose database context
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose(); // Clean up database connection
            }
            base.Dispose(disposing);
        }


        [HttpGet]
        public JsonResult SearchProducts(string term)
        {
            var products = db.Products
                .Where(p => p.IsServed && (p.Name.Contains(term) || term == null))
                .Select(p => new { id = p.Id, name = p.Name, price = p.Price })
                .Take(10)
                .ToList();
            return Json(new { results = products });
        }
    }
}