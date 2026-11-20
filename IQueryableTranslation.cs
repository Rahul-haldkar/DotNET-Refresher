using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IQueryableTranslationBoundaryTrap
{
    public record OrderDto(int Id, decimal Total);

    public class Order
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    public class OrdersService
    {
        private readonly DbContext _dbContext;

        public OrdersService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Correct implementation: compute UTC boundaries first,
        // then let SQL perform an index-friendly range scan.
        public async Task<List<OrderDto>> GetMonthlyOrders(
            int year,
            int month,
            int offsetHours)
        {
            var localStart = new DateTime(year, month, 1, 0, 0, 0,
                DateTimeKind.Unspecified);

            var localEnd = localStart.AddMonths(1);

            var utcStart = localStart.AddHours(-offsetHours);
            var utcEnd = localEnd.AddHours(-offsetHours);

            return await _dbContext.Set<Order>()
                .Where(o => o.CreatedAtUtc >= utcStart &&
                            o.CreatedAtUtc < utcEnd)
                .Select(o => new OrderDto(o.Id, o.Total))
                .ToListAsync();
        }
    }
}
