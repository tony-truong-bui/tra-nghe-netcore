

namespace TraNgheCore.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        ///Navigation props
        public virtual OrderModel Order { get; set; }
        public virtual ProductModel Product { get; set; }

    }
}