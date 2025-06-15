

namespace TraNgheCore.Models
{
    public class OrderListViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatus { get; set; }
        public string TypeOfOrder { get; set; } // e.g., Dine-in, Takeout, Delivery
        public string TableName { get; set; } // Name of the table if applicable
        public string UserName { get; set; } // Name of the user who created the order

        public List<OrderItemViewModel> OrderItems { get; set; } // List of items in the order
    }
}