namespace CarAuctionManagementSystem.Application.Validators;

using CarAuctionManagementSystem.Application.DTOs.Auctions;
using FluentValidation;

public class AuctionValidator : AbstractValidator<AddAuctionRequest>
{
    public AuctionValidator()
    {
        RuleFor(x => x.StartingBid)
            .GreaterThan(0)
            .WithMessage("Invalid starting bid value.");
    }
}