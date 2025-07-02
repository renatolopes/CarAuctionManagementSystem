namespace CarAuctionManagementSystem.Application.UnitTests.Services;

using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Bids;
using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Application.Services;
using CarAuctionManagementSystem.Application.Specifications.Auctions;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class AuctionsServiceTests
{
    [Fact]
    public void AddAuction_WithValidData_ShouldAddAuction()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out var auctionsRepository);

        // Act
        vehiclesService.Add(addVehicle);
        var result = auctionsService.Add(addAuction);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var addedAuction = auctionsRepository.Find(new FindAuctionByIdSpec(result.Value.Id));
        addedAuction.Should().NotBeNull();
        addedAuction!.Id.Should().Be(result.Value.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void AddAuction_WithInvalidStartingBidData_ShouldReturnError(float startingBid)
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _,
            startingBid);

        // Act
        vehiclesService.Add(addVehicle);
        var result = auctionsService.Add(addAuction);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Invalid starting bid value.");
    }

    [Fact]
    public void AddAuction_WhenAuctionAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _);

        // Act
        vehiclesService.Add(addVehicle);
        auctionsService.Add(addAuction);
        var result = auctionsService.Add(addAuction);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Auction for vehicle with license plate {addVehicle.LicensePlate} already exists.");
    }

    [Fact]
    public void AddAuction_ForNonExistentVehicle_ShouldReturnError()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _);

        addAuction = addAuction with { LicensePlate = "OverridingLicensePlace" };

        // Act
        vehiclesService.Add(addVehicle);
        auctionsService.Add(addAuction);
        var result = auctionsService.Add(addAuction);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Vehicle with license plate {addAuction.LicensePlate} not found.");
    }

    [Fact]
    public void StartAuction_ForValidAuction_ShouldStartAuction()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out var auctionsRepository);

        // Act
        vehiclesService.Add(addVehicle);
        var result = auctionsService.Add(addAuction);
        auctionsService.Start(result.Value.Id);

        // Assert
        var addedAuction = auctionsRepository.Find(new FindAuctionByIdSpec(result.Value.Id));
        addedAuction.Should().NotBeNull();
        addedAuction!.Active.Should().BeTrue();
    }

    [Fact]
    public void StartAuction_ForInValidAuctionId_ShouldNotStartAuction()
    {
        // Arrange
        const string invalidAuctionId = "InvalidAuctionId";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var auctionsRepository = new ServiceRepository<Auction>();
        var mockAuctionsServiceLogger = new Mock<ILogger<AuctionsService>>();
        var auctionsService = new AuctionsService(
            auctionsRepository,
            vehiclesRepository,
            mockAuctionsServiceLogger.Object);

        // Act

        var result = auctionsService.Start(invalidAuctionId);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not found.");
    }

    [Fact]
    public void CloseAuction_ForStartedAuction_ShouldClosedAuction()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out var auctionsRepository);

        // Act
        vehiclesService.Add(addVehicle);
        var result = auctionsService.Add(addAuction);
        auctionsService.Start(result.Value.Id);
        auctionsService.Close(result.Value.Id);

        // Assert
        var addedAuction = auctionsRepository.Find(new FindAuctionByIdSpec(result.Value.Id));
        addedAuction.Should().NotBeNull();
        addedAuction!.Active.Should().BeFalse();
    }

    [Fact]
    public void CloseAuction_ForInValidAuctionId_ShouldNotCloseAuction()
    {
        // Arrange
        const string invalidAuctionId = "InvalidAuctionId";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var auctionsRepository = new ServiceRepository<Auction>();
        var mockAuctionsServiceLogger = new Mock<ILogger<AuctionsService>>();
        var auctionsService = new AuctionsService(
            auctionsRepository,
            vehiclesRepository,
            mockAuctionsServiceLogger.Object);

        // Act
        auctionsService.Start(invalidAuctionId);
        var result = auctionsService.Close(invalidAuctionId);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not found.");
    }

    [Fact]
    public void StartAuction_ForAlreadyClosedAuction_ShouldNotStartAuction()
        {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _);

        // Act
        vehiclesService.Add(addVehicle);
        var addedAuction = auctionsService.Add(addAuction);
        auctionsService.Start(addedAuction.Value.Id);
        auctionsService.Start(addedAuction.Value.Id);
        var result = auctionsService.Start(addedAuction.Value.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction already started.");
        }

    [Fact]
    public void StartAuction_ForAlreadyStartedAuction_ShouldNotStartAuction()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _);

        // Act
        vehiclesService.Add(addVehicle);
        var addedAuction = auctionsService.Add(addAuction);
        auctionsService.Start(addedAuction.Value.Id);
        auctionsService.Close(addedAuction.Value.Id);
        var result = auctionsService.Start(addedAuction.Value.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction already closed.");
    }

    [Fact]
    public void CloseAuction_ForNotStartedAuction_ShouldNotCloseAuction()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _);

        // Act
        vehiclesService.Add(addVehicle);
        var addedAuction = auctionsService.Add(addAuction);
        var result = auctionsService.Close(addedAuction.Value.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not started yet.");
    }

    [Fact]
    public void PlaceBid_WithValidData_ShouldAddBid()
    {
        // Arrange
        const string bidder = "BidderName";

        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out var auctionsRepository);

        vehiclesService.Add(addVehicle);
        var addedAuctionResult = auctionsService.Add(addAuction);
        var addBid = new AddBidRequest(3000, bidder);
        var auctionId = addedAuctionResult.Value.Id;
        auctionsService.Start(auctionId);

        // Act
        var result = auctionsService.Bid(addedAuctionResult.Value.Id, addBid);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var auctionWithBid = auctionsRepository.Find(new FindAuctionByIdSpec(auctionId));
        auctionWithBid.Should().NotBeNull();
        auctionWithBid!.Bids.Count.Should().Be(1);
    }

    [Fact]
    public void PlaceBid_ToInvalidAuction_ShouldReturnError()
    {
        // Arrange
        const string bidder = "BidderName";
        const string invalidAuctionId = "invalidAuctionId";

        SetupAuctionServicesAndRepositories(
            out _,
            out var auctionsService,
            out _,
            out _);

        var addBid = new AddBidRequest(3000, bidder);

        // Act
        var result = auctionsService.Bid(invalidAuctionId, addBid);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not found.");
    }

    [Theory]
    [InlineData(5, "Bid value is less or equal than the starting bid value.")]
    [InlineData(10, "Bid value is less or equal than the starting bid value.")]
    [InlineData(1000, "Bid value is less or equal than the previous bid.")]
    [InlineData(3000, "Bid value is less or equal than the previous bid.")]
    public void PlaceBid_WithInvalidValue_ShouldReturnError(float bid, string expectedErrorMessage)
    {
        // Arrange
        const string bidder = "BidderName";
        const float startingBid = 10;

        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _,
            startingBid);

        vehiclesService.Add(addVehicle);
        var addedAuctionResult = auctionsService.Add(addAuction);
        var startBid = new AddBidRequest(3000, bidder);
        var nextBid = new AddBidRequest(bid, bidder);
        var auctionId = addedAuctionResult.Value.Id;
        auctionsService.Start(auctionId);

        // Act
        auctionsService.Bid(addedAuctionResult.Value.Id, startBid);
        var result = auctionsService.Bid(addedAuctionResult.Value.Id, nextBid);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedErrorMessage);
    }

    [Fact]
    public void GetAuctions_WithCreatedAuctions_ShouldReturnAuctions()
    {
        // Arrange
        var addAuction = SetupAuctionServicesAndRepositories(
            out var addVehicle,
            out var auctionsService,
            out var vehiclesService,
            out _);

        // Act
        vehiclesService.Add(addVehicle);
        auctionsService.Add(addAuction);
        var auctions = auctionsService.GetAllAuctions();

        // Assert
        auctions.Count().Should().Be(1);
    }

    [Fact]
    public void GetAuctions_WithoutCreatedAuctions_ShouldReturnEmptyList()
    {
        // Arrange
        SetupAuctionServicesAndRepositories(
            out _,
            out var auctionsService,
            out _,
            out _);

        // Act
        var auctions = auctionsService.GetAllAuctions();

        // Assert
        auctions.Count().Should().Be(0);
    }

    private static AddAuctionRequest SetupAuctionServicesAndRepositories(
        out AddVehicleRequest addVehicle,
        out AuctionsService auctionsService,
        out VehiclesService vehiclesService, 
        out ServiceRepository<Auction> auctionsRepository,
        float startingBid = 100
        )
    {
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-CD-34";
        var vehicleType = VehicleType.Hatchback;
        const int doorsNumber = 5;

        var addAuction = new AddAuctionRequest(startingBid, licensePlate);
        addVehicle = new AddVehicleRequest(
            manufacturer,
            model,
            year,
            vehicleType,
            licensePlate,
            doorsNumber);

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        auctionsRepository = new ServiceRepository<Auction>();
        var mockAuctionsServiceLogger = new Mock<ILogger<AuctionsService>>();
        var mockVehiclesServiceLogger = new Mock<ILogger<VehiclesService>>();

        auctionsService = new AuctionsService(
            auctionsRepository,
            vehiclesRepository,
            mockAuctionsServiceLogger.Object);

        vehiclesService = new VehiclesService(
            vehiclesRepository,
            mockVehiclesServiceLogger.Object);
        return addAuction;
    }
}