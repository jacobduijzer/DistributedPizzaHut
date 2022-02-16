namespace MessageContracts;

public interface IOrderToCreate
{
    Guid CustomerNumber { get; set; }
    List<Pizza> Pizzas { get; set; }
}