

namespace TraNgheCore.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; } = "";
        public decimal Price { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
        public int Quantity { get; set; }
        public decimal Total { get; set; } = 0;

        ///Navigation props
        public virtual OrderModel Order { get; set; }
        public virtual ProductModel Product { get; set; }

    }
}