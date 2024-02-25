using Contracts;
using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine("--> Consumiendo subasta con error");

            var ex = context.Message.Exceptions.FirstOrDefault();
            if (ex.ExceptionType == "System.ArgumentException")
            {
                context.Message.Message.Model = "FooBar";
                await context.Publish(context.Message.Message);
            }
            else
            {
                Console.WriteLine("--> No es un error de argumentos - ver error en rabbit");
            }
        }
    }
}
