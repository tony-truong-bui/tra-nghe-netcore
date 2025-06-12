
using System.ComponentModel.DataAnnotations;


namespace TraNgheCore.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }

        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        [Display(Name = "Customer Address")]
        public string CustomerAddress { get; set; }

        [Display(Name = "Type of Order")]
        public int? TypeOfOrder { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Order Time")]
        [DataType(DataType.Time)]
        public DateTime OrderTime { get; set; } = DateTime.Now;

        [Display(Name = "Order Status")]
        public int OrderStatus { get; set; }


        [Display(Name = "Order Item")]
        public virtual ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();


        [Display(Name = "Total Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Total price must be a positive number")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Table")]
        public int TableId { get; set; }
    }
}