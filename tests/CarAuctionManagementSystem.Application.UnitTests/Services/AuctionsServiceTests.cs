namespace CarAuctionManagementSystem.Application.UnitTests.Services;

using System.Data;
using System.Threading.Tasks;
using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Bids;
using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Application.Services;
using CarAuctionManagementSystem.Application.Specifications.Auctions;
using CarAuctionManagementSystem.Application.Specifications.Vehicles;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class AuctionsServiceTests
{
    private readonly Mock<IRepository<Auction>> _auctionsRepository;
    private readonly Mock<IRepository<Vehicle>> _vehicleRepository;
    private readonly Mock<ILogger<AuctionsService>> _auctionsServiceLogger;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly AuctionsService _service;

    public AuctionsServiceTests()
    {
        _auctionsRepository = new Mock<IRepository<Auction>>();
        _vehicleRepository = new Mock<IRepository<Vehicle>>();
        _auctionsServiceLogger = new Mock<ILogger<AuctionsService>>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _service = new AuctionsService(_auctionsRepository.Object, _vehicleRepository.Object, _auctionsServiceLogger.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task AddAuction_WithValidData_ShouldAddAuctionAsync()
    {
        // Arrange
        var mocks = GetMockRequests();
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        var result = await _service.AddAsync(mocks.AddAuction, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var addedAuction = result.Value;
        addedAuction.Should().NotBeNull();
        addedAuction!.Code.Should().Be(result.Value.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task AddAuction_WithInvalidStartingBidData_ShouldReturnErrorAsync(float startingBid)
    {
        // Arrange
        var mocks = GetMockRequests(startingBid);

        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        // Act
        var result = await _service.AddAsync(mocks.AddAuction, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Invalid starting bid value.");
    }

    [Fact]
    public async Task AddAuction_WhenAuctionAlreadyExists_ShouldReturnErrorAsync()
    {
        // Arrange
        var mocks = GetMockRequests();
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, mocks.AddAuction.LicensePlate);
        string[] includes = new string[] { };
        _auctionsRepository
           .Setup(x => x.AnyAsync(It.IsAny<FindAuctionByVehicleLicensePlate>(), It.IsAny<CancellationToken>(), includes))
           .ReturnsAsync(true)
           .Verifiable();

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        // Act
        var result = await _service.AddAsync(mocks.AddAuction, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Auction for vehicle with license plate {mocks.AddAuction.LicensePlate} already exists.");
    }

    [Fact]
    public async Task AddAuction_ForNonExistentVehicle_ShouldReturnErrorAsync()
    {
        // Arrange
        var mocks = GetMockRequests();
        string[] includes = new string[] { };
        _auctionsRepository
           .Setup(x => x.AnyAsync(It.IsAny<FindAuctionByVehicleLicensePlate>(), It.IsAny<CancellationToken>(), includes))
           .ReturnsAsync(false)
           .Verifiable();

        mocks.AddAuction = mocks.AddAuction with { LicensePlate = "OverridingLicensePlace" };

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([])
            .Verifiable();

        // Act
        var result = await _service.AddAsync(mocks.AddAuction, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Vehicle with license plate {mocks.AddAuction.LicensePlate} not found.");
    }

    [Fact]
    public async Task StartAuction_ForValidAuction_ShouldStartAuctionAsync()
    {
        // Arrange
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        var result = await _service.StartAsync(createdAuction.Code, default);

        // Assert
        result.IsFailed.Should().BeFalse();
    }

    [Fact]
    public async Task StartAuction_ForInValidAuctionId_ShouldNotStartAuctionAsync()
    {
        // Arrange
        const string invalidAuctionId = "InvalidAuctionId";

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .Verifiable();

        // Act
        var result = await _service.StartAsync(invalidAuctionId, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not found.");
    }

    [Fact]
    public async Task CloseAuction_ForStartedAuction_ShouldClosedAuctionAsync()
    {
        // Arrange
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);
        createdAuction.Start();

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        var result = await _service.CloseAsync(createdAuction.Code, default);

        // Assert
        result!.IsFailed.Should().BeFalse();
    }

    [Fact]
    public async Task CloseAuction_ForInValidAuctionId_ShouldNotCloseAuctionAsync()
    {
        // Arrange
        const string invalidAuctionId = "InvalidAuctionId";

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .Verifiable();

        // Act
        var result = await _service.CloseAsync(invalidAuctionId, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not found.");
    }

    [Fact]
    public async Task StartAuction_ForAlreadyClosedAuction_ShouldNotStartAuctionAsync()
    {
        // Arrange
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);
        createdAuction.Start();

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        var result = await _service.StartAsync(createdAuction.Code, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction already started.");
    }

    [Fact]
    public async Task StartAuction_ForAlreadyStartedAuction_ShouldNotStartAuctionAsync()
    {
        // Arrange
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);
        createdAuction.Close();

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        var result = await _service.StartAsync(createdAuction.Code, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction already closed.");
    }

    [Fact]
    public async Task CloseAuction_ForNotStartedAuction_ShouldNotCloseAuctionAsync()
    {
        // Arrange
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        var result = await _service.CloseAsync(createdAuction.Code, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not started yet.");
    }

    [Fact]
    public async Task PlaceBid_WithValidData_ShouldAddBidAsync()
    {
        // Arrange
        const string bidder = "BidderName";
        var addBid = new AddBidRequest(3000, bidder);
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);
        createdAuction.Start();

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        var result = await _service.BidAsync(createdAuction.Code, addBid, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        //var auctionWithBid = result.Value;
        //auctionWithBid.Should().NotBeNull();
        //auctionWithBid!.Bids.Count.Should().Be(1); //TODO
    }

    [Fact]
    public async Task PlaceBid_ToInvalidAuction_ShouldReturnErrorAsync()
    {
        // Arrange
        const string bidder = "BidderName";
        const string invalidAuctionId = "invalidAuctionId";
        var addBid = new AddBidRequest(3000, bidder);

        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .Verifiable();

        // Act
        var result = await _service.BidAsync(invalidAuctionId, addBid, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Auction not found.");
    }

    [Theory]
    [InlineData(5, "Bid value is less or equal than the starting bid value.")]
    [InlineData(10, "Bid value is less or equal than the starting bid value.")]
    [InlineData(1000, "Bid value is less or equal than the previous bid.")]
    [InlineData(3000, "Bid value is less or equal than the previous bid.")]
    public async Task PlaceBid_WithInvalidValue_ShouldReturnErrorAsync(float bid, string expectedErrorMessage)
    {
        // Arrange
        const string bidder = "BidderName";
        const float startingBid = 10;
        var startBid = new AddBidRequest(3000, bidder);
        var nextBid = new AddBidRequest(bid, bidder);

        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);
        createdAuction.Start();

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.SingleAsync(It.IsAny<FindAuctionByCodeSpec>(), It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync(createdAuction)
            .Verifiable();

        // Act
        await _service.BidAsync(createdAuction.Code, startBid, default);
        var result = await _service.BidAsync(createdAuction.Code, nextBid, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedErrorMessage);
    }

    [Fact]
    public async Task GetAuctions_WithCreatedAuctions_ShouldReturnAuctionsAsync()
    {
        // Arrange
        var vehicle = new Vehicle("manufacturer", "model", 2025, VehicleType.Sedan, "AB-XX-01");
        var createdAuction = new Auction(100, 1);
        createdAuction.SetVehicle(vehicle);
        createdAuction.Close();

        _vehicleRepository
            .Setup(x => x.FindAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync([vehicle])
            .Verifiable();

        _auctionsRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync([createdAuction])
            .Verifiable();

        // Act
        var auctions = await _service.GetAllAuctionsAsync(default);

        // Assert
        auctions.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetAuctions_WithoutCreatedAuctions_ShouldReturnEmptyListAsync()
    {
        // Arrange
        _auctionsRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), false, "Vehicle"))
            .ReturnsAsync([])
            .Verifiable();

        // Act
        var auctions = await _service.GetAllAuctionsAsync(default);

        // Assert
        auctions.Count().Should().Be(0);
    }

    private static (AddAuctionRequest AddAuction, AddVehicleRequest AddVehicle) GetMockRequests(float startingBid = 100)
    {
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-CD-34";
        var vehicleType = VehicleType.Hatchback;
        const int doorsNumber = 5;

        var addAuction = new AddAuctionRequest(startingBid, licensePlate);
        var addVehicle = new AddVehicleRequest(
            manufacturer,
            model,
            year,
            vehicleType,
            licensePlate,
            doorsNumber);

        return (addAuction, addVehicle);
    }
}