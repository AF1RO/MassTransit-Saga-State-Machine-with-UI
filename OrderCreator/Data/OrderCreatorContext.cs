using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderCreator.Models;

namespace OrderCreator.Data
{
    public class OrderCreatorContext : DbContext
    {
        public OrderCreatorContext (DbContextOptions<OrderCreatorContext> options)
            : base(options)
        {
        }

        public DbSet<OrderCreator.Models.OrderState> OrderState { get; set; } = default!;
    }
}
