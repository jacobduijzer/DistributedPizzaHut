using MassTransit;
using MessageContracts;

namespace PaymentMicroservice;

public class OrderCreatedEventConsumer : IConsumer<IOrderCreated>
{
   public async Task Consume(ConsumeContext<IOrderCreated> context)
   {
      var paymentNumber = Guid.NewGuid();
      Console.WriteLine($"Creating payment with number {paymentNumber}");
      Console.WriteLine($"For order: {context.Message.OrderNumber}");
      Console.WriteLine($"Total amount: {context.Message.OrderData.Pizzas.Sum(x => x.Price)}");

      await context.Publish<IPaymentToMake>(new
      {
         PaymentNumber = paymentNumber,
         TotalAmount = context.Message.OrderData.Pizzas.Sum(x => x.Price),
         OrderData = new
         {
            OrderNumber = context.Message.OrderNumber,
            OrderData = new
            {
               CustomerNumber = context.Message.OrderData.CustomerNumber,
               Pizzas = context.Message.OrderData.Pizzas
            }
         }
      });
   }
}