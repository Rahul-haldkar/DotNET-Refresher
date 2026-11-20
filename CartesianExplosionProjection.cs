using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CartesianExplosionProjection
{
    public class CustomerSummaryDto
    {
        public string Name { get; set; } = string.Empty;
        public List<decimal> RecentOrders { get; set; } = new();
        public int OpenTicketCount { get; set; }
    }

    public class CustomerService
    {
        private readonly DbContext _dbContext;

        public CustomerService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Projection-based solution
        public async Task<List<CustomerSummaryDto>> GetCustomerSummaries()
        {
            return await _dbContext.Set<Customer>()
                .Where(c => c.IsActive)
                .Select(c => new CustomerSummaryDto
                {
                    Name = c.Name,

                    RecentOrders = c.Orders
                        .OrderByDescending(o => o.Date)
                        .Take(3)
                        .Select(o => o.Total)
                        .ToList(),

                    OpenTicketCount = c.SupportTickets
                        .Count(t => !t.IsClosed)
                })
                .ToListAsync();
        }
    }

    // Sample entities
    public class Customer
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
    }

    public class Order
    {
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }

    public class SupportTicket
    {
        public bool IsClosed { get; set; }
    }
}
