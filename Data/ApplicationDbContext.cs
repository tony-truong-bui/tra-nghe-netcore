using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TraNgheCore.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityModel>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // ... other DbSets
    public DbSet<ProductModel> Products { get; set; }

    public DbSet<CategoryModel> Categories { get; set; }

    public DbSet<TableModel> Tables { get; set; }
    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<TypeOfOrder> OrdersTypes { get; set; }
    public DbSet<OrderItemModel> OrderItems { get; set; }

    public DbSet<OrderStatusModel> OrderStatuses { get; set; }
}