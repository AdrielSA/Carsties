using AuctionService.DTOs;
using FluentValidation;

namespace AuctionService.Validators
{
    public class UpdateAuctionDtoValidator : AbstractValidator<UpdateAuctionDto>
    {
        public UpdateAuctionDtoValidator()
        {
            RuleFor(x => x.Model)
                .NotNull().NotEmpty();

            RuleFor(x => x.Make)
                .NotNull().NotEmpty();

            RuleFor(x => x.Color)
                .NotNull().NotEmpty();
        }
    }
}
