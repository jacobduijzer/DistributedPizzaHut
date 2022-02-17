using Microsoft.EntityFrameworkCore;

namespace OrderMicroservice;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Order> Orders { get; set; }
}