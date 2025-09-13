


using System.ComponentModel.DataAnnotations;

namespace TraNgheCore.Models
{
    public class CreateOrderViewModel
    {
        //Order info
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }
        [Display(Name = "Customer Address")]
        public string CustomerAddress { get; set; }
        [Display(Name = "Type of Order")]
        public int TypeOfOrder { get; set; } // Type of order (e.g., dine-in, takeout, delivery)
        [Display(Name = "Table ID")]
        public int? TableId { get; set; } // Table ID for dine-in orders


        //Order items
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        // Calculated fields
        public decimal TotalPrice => OrderItems?.Sum(item => item.Price * item.Quantity) ?? 0;

    }

   
}