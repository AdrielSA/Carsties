using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;
using System;
using System.Threading.Tasks;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer(IMapper mapper) : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper = mapper;

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine($"--> Consumiendo subasta creada: {context.Message.Id}");

            var item = _mapper.Map<Item>(context.Message);

            if (item.Model == "Foo")
                throw new ArgumentException("No se puede agregar un vehículo que contenga la palabra Foo");

            await item.SaveAsync();
        }
    }
}
