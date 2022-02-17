using Microsoft.EntityFrameworkCore;

namespace OrderMicroservice;

public class OrderStorage : IOrderStorage
{
    private readonly OrderDbContext _orderDbContext;

    public OrderStorage(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task StoreOrder(Order order)
    {
        await _orderDbContext.Orders.AddAsync(order);
        await _orderDbContext.SaveChangesAsync();
    }

    public async Task<List<Order>> GetOrdersForCustomer(Guid customerId)
    {
        return await _orderDbContext
            .Orders
            .Include(x => x.Pizzas)
            .Where(order => order.CustomerNumber == customerId)
            .ToListAsync();
    }
}