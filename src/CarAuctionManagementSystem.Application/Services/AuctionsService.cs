namespace CarAuctionManagementSystem.Application.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Bids;
using CarAuctionManagementSystem.Application.Interfaces;
using CarAuctionManagementSystem.Application.Mappers;
using CarAuctionManagementSystem.Application.Specifications.Auctions;
using CarAuctionManagementSystem.Application.Specifications.Vehicles;
using CarAuctionManagementSystem.Application.Validators;
using CarAuctionManagementSystem.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;

public class AuctionsService : IAuctionsService
{
    private readonly IRepository<Auction> _auctionRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly ILogger<AuctionsService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AuctionsService(
        IRepository<Auction> auctionRepository,
        IRepository<Vehicle> vehicleRepository,
        ILogger<AuctionsService> logger,
        IUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _vehicleRepository = vehicleRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AvailableAuction>> AddAsync(AddAuctionRequest auction, CancellationToken cancellationToken)
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
        var existAuctionForVehicle = await _auctionRepository.AnyAsync(auctionSpec, cancellationToken);

        if (existAuctionForVehicle)
        {
            return Result.Fail($"Auction for vehicle with license plate {auction.LicensePlate} already exists.");
        }

        var vehicleSpec = new FindVehicleByLicensePlateSpec(auction.LicensePlate);
        var vehicleList = await _vehicleRepository.FindAsync(vehicleSpec, cancellationToken);

        if (!vehicleList.Any())
        {
            return Result.Fail($"Vehicle with license plate {auction.LicensePlate} not found.");
        }

        var newAuction = new Auction(auction.StartingBid, vehicleList.First().Id);
        _auctionRepository.Add(newAuction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdAuction = await FindAuctionAsync(newAuction.Code, cancellationToken);
        _logger.LogInformation(
            "Auction for vehicle with license plate {LicensePlate} created.",
            createdAuction!.Vehicle.LicensePlate);

        return Result.Ok(createdAuction.MapToAvailableAuction());
    }

    public async Task<IEnumerable<AvailableAuction>> GetAllAuctionsAsync(CancellationToken cancellationToken)
    {
        var auctions = await _auctionRepository.GetAllAsync(cancellationToken, false, "Vehicle");
        return auctions.Select(x => x.MapToAvailableAuction());
    }

    public async Task<Result> StartAsync(string auctionId, CancellationToken cancellationToken)
    {
        var auction = await FindAuctionAsync(auctionId, cancellationToken);

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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Auction {Auction} for vehicle with license plate {LicensePlate} started.",
            auction.Code,
            auction.Vehicle.LicensePlate);

        return Result.Ok();
    }

    public async Task<Result> CloseAsync(string auctionId, CancellationToken cancellationToken)
    {
        var auction = await FindAuctionAsync(auctionId, cancellationToken);

        if (auction is null)
        {
            return Result.Fail("Auction not found.");
        }

        if (auction.StartDate is null)
        {
            return Result.Fail("Auction not started yet.");
        }

        if (auction.CloseDate is not null)
        {
            return Result.Fail("Auction already closed.");
        }

        auction.Close();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Auction {Auction} for vehicle with license plate {LicensePlate} closed.",
            auction.Code,
            auction.Vehicle.LicensePlate);

        return Result.Ok();
    }

    public async Task<Result> BidAsync(string auctionId, AddBidRequest bid, CancellationToken cancellationToken)
    {
        var auction = await FindAuctionAsync(auctionId, cancellationToken);

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

        auction.Bids.Add(new Bid(bid.Value, bid.Bidder, DateTime.UtcNow, auction.Id));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Bid with value {Value} for auction {Auction} with vehicle with license plate {LicensePlate}.",
            bid.Value,
            auction.Code,
            auction.Vehicle.LicensePlate);

        return Result.Ok();
    }

    private async Task<Auction?> FindAuctionAsync(string auctionCode, CancellationToken cancellationToken)
    {
        var spec = new FindAuctionByCodeSpec(auctionCode);
        return await _auctionRepository.SingleAsync(spec, cancellationToken, false, "Vehicle");
    }
}