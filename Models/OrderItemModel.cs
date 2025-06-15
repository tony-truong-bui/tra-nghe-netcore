
using System.ComponentModel.DataAnnotations.Schema;

namespace TraNgheCore.Models
{
    public class OrderItemModel
    {

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0;
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total => Price * Quantity;

        ///Navigation properties
        public virtual OrderModel Order { get; set; }
        public virtual ProductModel Product { get; set; }

    }
}