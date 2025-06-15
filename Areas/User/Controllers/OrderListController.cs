
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraNgheCore.Models;

namespace TraNgheCore.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    [Area("User")]
    public class OrderListController : Controller
    {
        private readonly ApplicationDbContext db;
        public OrderListController(ApplicationDbContext _db)
        {
            db = _db;
        }
        // GET: User/OrderList


        /// <summary>
        /// 📋 ORDER INDEX ACTION: Displays paginated list of all orders
        /// Optimized with dictionary lookups to avoid N+1 query problems
        /// Created by: tony-truong-bui
        /// Current Time: 2025-06-09 17:15:19 UTC
        /// </summary>
        public IActionResult Index()
        {
            // 🔍 STEP 1: OPTIMIZED ORDER QUERY
            // Select only needed fields to reduce memory usage and improve performance

            var ordersQuery = db.Orders
             .OrderByDescending(o => o.OrderDate)       // 📅 Newest orders first (restaurant priority)
             .Select(o => new                           // 🎯 Anonymous object projection (performance optimization)
             {
                 o.Id,
                 o.CustomerName,
                 o.OrderDate,
                 o.TotalPrice,
                 o.OrderStatus,
                 o.TypeOfOrder,
                 o.TableId
             });

            // ⚡ Execute query and materialize results in memory
            // Converts IQueryable to List<> - database hit happens here
            var ordersList = ordersQuery.ToList();

            // 🚀 STEP 2: LOOKUP DICTIONARIES (Performance Optimization)
            // Load ALL order types and tables into dictionaries for fast lookups
            // This prevents N+1 query problem (instead of querying for each order)
            var typeOfOrderDict = db.OrdersTypes.ToDictionary(t => t.Id, t => t.Name);
            // Example: { 1 => "Take-out", 2 => "Dine-in", 3 => "Delivery" }

            var tableDict = db.Tables.ToDictionary(t => t.Id, t => t.Name);
            // Example: { 1 => "Table 1", 2 => "Table 2", 3 => "VIP Room" }

            // 🔄 STEP 3: DATA TRANSFORMATION
            // Convert anonymous objects to strongly-typed ViewModels
            var orders = ordersList.Select(o => new OrderListViewModel
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                OrderStatus = o.OrderStatus,

                // 🔍 SAFE LOOKUP: Order Type with null checking
                TypeOfOrder = o.TypeOfOrder.HasValue && typeOfOrderDict.ContainsKey(o.TypeOfOrder.Value)
                    ? typeOfOrderDict[o.TypeOfOrder.Value]      // ✅ Found: Return actual name
                    : "N/A",                                    // ❌ Not found or null: Display "N/A"

                // 🔍 SAFE LOOKUP: Table Name with existence check
                TableName = tableDict.ContainsKey(o.TableId)     // ✅ Found: Return table name
                    ? tableDict[o.TableId]
                    : "N/A"                            // ❌ Not found: Display "N/A"
            }).ToList();

            // 📤 RETURN: Pass transformed data to view
            return View(orders);     // View receives List<OrderListViewModel>
        }
    }
    
}