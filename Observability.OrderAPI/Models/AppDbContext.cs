﻿using Microsoft.EntityFrameworkCore;
using Observability.OrderAPI.OrderServices;

namespace Observability.OrderAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}