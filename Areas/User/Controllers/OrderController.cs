
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TraNgheCore.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TraNgheCore.Areas.User.Models;



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


        // For one form partial view
        public IActionResult OrderFormPartial(int index)
        {
            var model = new CreateOrderViewModel();
            ViewBag.Products = GetProductOptions();
            ViewBag.Tables = GetTableSelectList();
            ViewBag.TypesOfOrders = GetTypeOfOdersSelectList();
            var templInfo = new TemplateInfo { HtmlFieldPrefix = $"Orders[{index}]" };
            ViewData["TemplateInfo"] = templInfo; // Pass template info for correct model binding
            return PartialView("_CreateOrderForm", model);
        }


        // 📝 GET: Display order creation form for customers
        // URL: /User/Order/Create
        public IActionResult Create(int index)
        {
            var multiModel = new MultiOrderCreateViewModel
            {
                Orders = new List<CreateOrderViewModel>()       // Initialize with one empty order
            };
            // Initialize empty ViewModel for new order
            multiModel.Orders.Add(new CreateOrderViewModel());


            // Load available products for dropdown selection
            // Only shows products that are currently being served
            ViewBag.Products = GetProductOptions();

            //Load available tables for dine-in orders
            // Only shows tables that are currently available
            ViewBag.Tables = GetTableSelectList();

            ViewBag.TypesOfOrders = GetTypeOfOdersSelectList();

            // Render the partial with the correct prefix for model binding
            var templateInfo = new TemplateInfo { HtmlFieldPrefix = $"Orders[{index}]" };
            ViewData["TemplateInfo"] = templateInfo;

            return View(multiModel);
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
                        TableId = model.TableId ?? default(int), // Nullable for dine-in orders
                        OrderDate = DateTime.Now,
                        TotalPrice = model.OrderItems.Sum(item => item.Price * item.Quantity),
                        TypeOfOrder = model.TypeOfOrder, // Default to 0 if not specified'
                        OrderStatus = 0, // Default status - Creating
                        UserId = userId,

                        // TableId = model.TableId ?? default(int), // Nullable for dine-in orders  
                        //TableId = (int)(model.TableId.HasValue ? model.TableId.Value : (int?) null), // Nullable for dine-in orders

                    };
                    Console.WriteLine("Creating order for user: " + order);
                    db.Orders.Add(order);
                    db.SaveChanges(); // Get the Order ID

                    // 🏗️ STEP 2: Create individual order items
                    foreach (var item in model.OrderItems)
                    {
                        
                        item.ProductName = db.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name; // Set product name from details
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
                    return Json(new { success = true, message = "Order created successfully!", orderId = order.Id });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while creating the order: " + ex.Message });
                }
            }
            // 🔄 If validation fails or error occurs, reload form with products
            ViewBag.Products = GetProductOptions();
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Validation failed.", errors });
        }

        //⚡AJAX: Fetch product details for dynamic form updates
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

        // 🛠️ Create dropdown list of available products
        private IEnumerable<ProductOptionViewModel> GetProductOptions()
        {
            return db.Products
                .Where(p => p.IsServed)
                .Select(p => new ProductOptionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToList();
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