using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice;

public class Order
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid CustomerNumber { get; set; }
    
    public ICollection<Pizza> Pizzas{ get; set; } = new List<Pizza>();
}