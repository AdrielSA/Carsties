using AuctionService.Data.DbContext;
using Contracts;
using MassTransit;
using System.Threading.Tasks;
using AuctionService.Enums;
using System;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer(AuctionDbContext dbContext) : IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consumiendo subasta finalizada...");

            var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);
            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }
            auction.Status = auction.SoldAmount > auction.ReservePrice ? Status.Finished : Status.ReserveNotMet;
            await _dbContext.SaveChangesAsync();
        }
    }
}
