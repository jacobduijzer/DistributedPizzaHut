using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice;

public class Pizza
{
    public int Id { get; set; }
    public Guid PizzaId { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    
    public Order? Order { get; set; }
}