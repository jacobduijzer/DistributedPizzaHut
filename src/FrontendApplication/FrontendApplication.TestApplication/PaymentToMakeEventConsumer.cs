using MassTransit;
using MessageContracts;

namespace FrontendApplication.TestApplication;

public class PaymentToMakeEventConsumer : IConsumer<IPaymentToMake>
{
    public async Task Consume(ConsumeContext<IPaymentToMake> context)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"Payment requested for order {context.Message.OrderData.OrderNumber}");
            Console.WriteLine($"Total amount: {context.Message.TotalAmount}");
        });
    } 
}
