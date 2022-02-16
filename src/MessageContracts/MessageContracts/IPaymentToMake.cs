namespace MessageContracts;

public interface IPaymentToMake
{
   Guid PaymentNumber { get; set; } 
   double TotalAmount { get; set; }
   IOrderCreated OrderData { get; set; }
}