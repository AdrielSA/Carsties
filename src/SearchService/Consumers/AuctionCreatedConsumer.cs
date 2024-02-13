using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;
using System.Threading.Tasks;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer(IMapper mapper) : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper = mapper;

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            var item = _mapper.Map<Item>(context.Message);
            await item.SaveAsync();
        }
    }
}
