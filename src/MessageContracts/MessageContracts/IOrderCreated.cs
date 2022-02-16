namespace MessageContracts;

public interface IOrderCreated
{
    public Guid OrderNumber { get; set; }
    public IOrderToCreate OrderData { get; set; }
}