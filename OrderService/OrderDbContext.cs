using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Collections.Generic;

namespace OrderService
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
    }
}
