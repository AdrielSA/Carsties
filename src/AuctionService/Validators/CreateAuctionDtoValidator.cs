using AuctionService.DTOs;
using FluentValidation;

namespace AuctionService.Validators
{
    public class CreateAuctionDtoValidator : AbstractValidator<CreateAuctionDto>
    {
        public CreateAuctionDtoValidator()
        {
            RuleFor(x => x.Make)
                .NotNull().NotEmpty();

            RuleFor(x => x.Model)
                .NotNull().NotEmpty();

            RuleFor(x => x.Year)
                .NotNull().NotEmpty();

            RuleFor(x => x.Color)
                .NotNull().NotEmpty();

            RuleFor(x => x.Mileage)
                .NotNull().NotEmpty();

            RuleFor(x => x.ImageUrl)
                .NotNull().NotEmpty();

            RuleFor(x => x.ReservePrice)
                .NotNull().NotEmpty();

            RuleFor(x => x.AuctionEnd)
                .NotNull().NotEmpty();
        }
    }
}
