using AuctionService.Data.DbContext;
using Contracts;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer(AuctionDbContext dbContext) : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consumiendo oferta...");

            var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);

            if(auction.CurrentHightBid is null 
                || context.Message.BidStatus.Contains("Accepted") 
                && context.Message.Amount > auction.CurrentHightBid)
            {
                auction.CurrentHightBid = context.Message.Amount;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
