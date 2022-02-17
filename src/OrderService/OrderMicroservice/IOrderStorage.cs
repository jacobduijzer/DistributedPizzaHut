namespace OrderMicroservice;

public interface IOrderStorage
{
    Task StoreOrder(Order order);
    Task<List<Order>> GetOrdersForCustomer(Guid customerId);
}