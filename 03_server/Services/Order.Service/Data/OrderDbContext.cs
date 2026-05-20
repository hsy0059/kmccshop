using Microsoft.EntityFrameworkCore;
using Order.Service.Models.Entities;

namespace Order.Service.Data;

public class OrderDbContext : Campus.Infrastructure.BaseDbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<DeliveryOrder> DeliveryOrders => Set<DeliveryOrder>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ErrandOrder> ErrandOrders => Set<ErrandOrder>();
    public DbSet<OrderComment> OrderComments => Set<OrderComment>();
}