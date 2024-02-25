using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;
using System;
using System.Threading.Tasks;

namespace SearchService.Consumers
{
    public class AuctionDeleteConsumer : IConsumer<AuctionDeleted>
    {
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            Console.WriteLine($"--> Consumiendo subasta eliminada: {context.Message.Id}");

            var result = await DB.DeleteAsync<Item>(context.Message.Id);

            if (!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionDeleted), "--> Ocurrió un error eliminando en la base de datos (mongoDb).");
        }
    }
}
