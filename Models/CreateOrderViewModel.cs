


namespace TraNgheCore.Models
{
    public class CreateOrderViewModel
    {
        //Order info
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public int TypeOfOrder { get; set; } // Type of order (e.g., dine-in, takeout, delivery)
        public int? TableId { get; set; } // Table ID for dine-in orders


        //Order item
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        // Calculated fields
        public decimal TotalPrice => OrderItems?.Sum(item => item.Price * item.Quantity) ?? 0;

    }

   
}