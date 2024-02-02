using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data.DbContext
{
    public class AuctionDbContext(DbContextOptions<AuctionDbContext> options) 
        : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Auction> Auctions { get; set; }
    }
}
