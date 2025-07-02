namespace CarAuctionManagementSystem.Application.Services;

using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Bids;
using CarAuctionManagementSystem.Application.Interfaces;
using CarAuctionManagementSystem.Application.Mappers;
using CarAuctionManagementSystem.Application.Specifications.Auctions;
using CarAuctionManagementSystem.Application.Specifications.Vehicles;
using CarAuctionManagementSystem.Application.Validators;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

public class AuctionsService : IAuctionsService
{
    private readonly IServiceRepository<Auction> _auctionRepository;
    private readonly IServiceRepository<Vehicle> _vehicleRepository;
    private readonly ILogger<AuctionsService> _logger;

    public AuctionsService(
        IServiceRepository<Auction> auctionRepository,
        IServiceRepository<Vehicle> vehicleRepository,
        ILogger<AuctionsService> logger)
    {
        _auctionRepository = auctionRepository;
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public Result<AvailableAuction> Add(AddAuctionRequest auction)
    {
        var auctionValidator = new AuctionValidator();
        var result = auctionValidator.Validate(auction);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            return Result.Fail(errors);
        }

        var auctionSpec = new FindAuctionByVehicleLicensePlate(auction.LicensePlate);
        var auctionForVehicle = _auctionRepository.Find(auctionSpec);

        if (auctionForVehicle is not null)
        {
            return Result.Fail($"Auction for vehicle with license plate {auction.LicensePlate} already exists.");
        }

        var vehicleSpec = new FindVehicleByLicensePlateSpec(auction.LicensePlate);
        var vehicle = _vehicleRepository.Find(vehicleSpec);

        if (vehicle is null)
        {
            return Result.Fail($"Vehicle with license plate {auction.LicensePlate} not found.");
        }

        var newAuction = new Auction(auction.StartingBid, vehicle);
        var createdAuction = _auctionRepository.Add(newAuction);

        _logger.LogInformation(
            "Auction for vehicle with license plate {LicensePlate} created.",
            createdAuction.Vehicle.LicensePlate);

        return Result.Ok(createdAuction.MapToAvailableAuction());
    }

    public IEnumerable<AvailableAuction> GetAllAuctions()
    {
        var auctions = _auctionRepository.FindAll();
        return auctions.Select(x => x.MapToAvailableAuction());
    }

    public Result Start(string auctionId)
    {
        var auction = FindAuction(auctionId);

        if (auction is null)
        {
            return Result.Fail("Auction not found.");
        }

        if (auction.CloseDate is not null)
        {
            return Result.Fail("Auction already closed.");
        }

        if (auction.StartDate is not null)
        {
        return Result.Fail("Auction already started.");
        }

        auction.Start();

        _logger.LogInformation(
            "Auction {Auction} for vehicle with license plate {LicensePlate} started.",
            auction.Id,
            auction.Vehicle.LicensePlate);

        return Result.Ok();
    }

    public Result Close(string auctionId)
    {
        var auction = FindAuction(auctionId);

        if (auction is null)
        {
            return Result.Fail("Auction not found.");
        }

        if (auction.StartDate is null)
        {
            return Result.Fail("Auction not started yet.");
        }

        auction.Close();

        _logger.LogInformation(
            "Auction {Auction} for vehicle with license plate {LicensePlate} closed.",
            auction.Id,
            auction.Vehicle.LicensePlate);

        return Result.Ok();
    }

    public Result Bid(string auctionId, AddBidRequest bid)
    {
        var spec = new FindAuctionByIdSpec(auctionId);
        var auction = _auctionRepository.Find(spec);

        if (auction is null)
        {
            return Result.Fail("Auction not found.");
        }

        if (!auction.Active)
        {
            return Result.Fail("Auction not active.");
        }

        if (bid.Value <= auction.StartingBid)
        {
            return Result.Fail("Bid value is less or equal than the starting bid value.");
        }

        if (auction.Bids.Count > 0)
        {
            if (bid.Value <= auction.Bids.Last().Value)
            {
                return Result.Fail("Bid value is less or equal than the previous bid.");
            }
        }

        auction.Bids.Add(new Bid(bid.Value, bid.Bidder, DateTime.Now));

        _logger.LogInformation(
            "Bid with value {Value} for auction {Auction} with vehicle with license plate {LicensePlate}.",
            bid.Value,
            auction.Id,
            auction.Vehicle.LicensePlate);

        return Result.Ok();
    }

    private Auction? FindAuction(string auctionId)
    {
        var spec = new FindAuctionByIdSpec(auctionId);
        return _auctionRepository.Find(spec);
    }
}