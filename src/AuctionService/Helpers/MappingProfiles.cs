using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDto>()
                .IncludeMembers(x => x.Item).ReverseMap();

            CreateMap<Item, AuctionDto>()
                .ReverseMap();

            CreateMap<Auction, CreateAuctionDto>()
                .ReverseMap().ForMember(x => x.Item, o => o.MapFrom(s => s));

            CreateMap<Item, CreateAuctionDto>()
                .ReverseMap();

            CreateMap<AuctionDto, AuctionCreated>();
        }
    }
}
