using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _dBcontext;

    public BidPlacedConsumer(AuctionDbContext dbContext)
    {
        _dBcontext = dbContext;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> consuming bid placed");

        var auction = await _dBcontext.Auctions.FindAsync(context.Message.AuctionId);

        if (auction.CurrentHighBid == null 
        || context.Message.BidStatus.Contains("Accepted")
        && context.Message.Amount > auction.CurrentHighBid) 
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _dBcontext.SaveChangesAsync();
        }
    }
}