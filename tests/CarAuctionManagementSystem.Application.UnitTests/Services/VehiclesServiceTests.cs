namespace CarAuctionManagementSystem.Application.UnitTests.Services;

using System.Threading.Tasks;
using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Application.Services;
using CarAuctionManagementSystem.Application.Specifications.Vehicles;
using CarAuctionManagementSystem.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class VehiclesServiceTests
{
    private readonly Mock<IRepository<Vehicle>> _vehicleRepository;
    private readonly Mock<ILogger<VehiclesService>> _vehiclesServiceLogger;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly VehiclesService _service;

    public VehiclesServiceTests()
    {
        _vehicleRepository = new Mock<IRepository<Vehicle>>();
        _vehiclesServiceLogger = new Mock<ILogger<VehiclesService>>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _service = new VehiclesService( _vehicleRepository.Object, _vehiclesServiceLogger.Object, _unitOfWork.Object);
    }

    [Theory]
    [InlineData(VehicleType.Sedan, 5, null, null)]
    [InlineData(VehicleType.Hatchback, 5, null, null)]
    [InlineData(VehicleType.SUV, null, 6, null)]
    [InlineData(VehicleType.Truck, null, null, 10000f)]
    public async Task AddVehicle_WithValidInformation_ShouldAddVehicleAsync(
        VehicleType vehicleType,
        int? doorsNumber,
        int? seatsNumber,
        float? loadCapacity)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(
            manufacturer,
            model,
            year,
            vehicleType,
            licensePlate,
            doorsNumber,
            seatsNumber,
            loadCapacity);

        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var addedVehicle = result.Value;
        addedVehicle.Should().NotBeNull();
        addedVehicle!.Manufacturer.Should().Be(manufacturer);
        addedVehicle.Model.Should().Be(model);
        addedVehicle.Year.Should().Be(year);
        addedVehicle.Type.Should().Be(vehicleType);
        addedVehicle.LicensePlate.Should().Be(licensePlate);
    }

    [Theory]
    [InlineData(VehicleType.Sedan)]
    [InlineData(VehicleType.Hatchback)]
    public async Task AddVehicleWithDoorsNumberSpec_WithoutDoorsNumber_ShouldNotAddVehicleAsync(VehicleType vehicleType)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of doors is mandatory for {VehicleType.Hatchback} or {VehicleType.Sedan}.");
    }

    [Fact]
    public async Task AddVehicleWithSeatsNumberSpec_WithoutSeatsNumber_ShouldNotAddVehicleAsync()
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";
        const VehicleType vehicleType = VehicleType.SUV;

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of seats is mandatory for {VehicleType.SUV}.");
    }

    [Fact]
    public async void AddVehicleWithLoadCapacitySpec_WithoutLoadCapacity_ShouldNotAddVehicle()
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";
        const VehicleType vehicleType = VehicleType.Truck;

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Load capacity is mandatory for {VehicleType.Truck}.");
    }

    [Theory]
    [InlineData(VehicleType.Sedan, 5, null)]
    [InlineData(VehicleType.Hatchback, 5, null)]
    [InlineData(VehicleType.SUV, null, 6)]
    public async Task AddVehicleWithoutLoadCapacitySpec_WithLoadCapacity_ShouldNotAddVehicleAsync(
        VehicleType vehicleType,
        int? doorsNumber,
        int? seatsNumber)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber, seatsNumber, 10000);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Load capacity is not allowed for {VehicleType.Hatchback}, {VehicleType.Sedan} or {VehicleType.SUV} vehicles.");
    }

    [Theory]
    [InlineData(VehicleType.Hatchback, 5, 6, null)]
    [InlineData(VehicleType.Truck, null, 3, 10000f)]
    public async Task AddVehicleWithoutSeatsNumberSpec_WithSeatsNumber_ShouldNotAddVehicleAsync(
        VehicleType vehicleType,
        int? doorsNumber,
        int? seatsNumber,
        float? loadCapacity)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber, seatsNumber, loadCapacity);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of seats is not allowed for {VehicleType.Hatchback}, {VehicleType.Sedan} or {VehicleType.Truck} vehicles.");
    }

    [Theory]
    [InlineData(VehicleType.SUV, 5, 6, null)]
    [InlineData(VehicleType.Truck, 5, null, 10000f)]
    public async Task AddVehicleWithoutDoorsNumberSpec_WithDoorsNumber_ShouldNotAddVehicleAsync(
        VehicleType vehicleType,
        int? doorsNumber,
        int? seatsNumber,
        float? loadCapacity)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber, seatsNumber, loadCapacity);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of doors is not allowed for {VehicleType.SUV}, or {VehicleType.Truck} vehicles.");
    }

    [Theory]
    [InlineData(VehicleType.Hatchback, 10)]
    [InlineData(VehicleType.Hatchback, 0)]
    [InlineData(VehicleType.Sedan, 0)]
    [InlineData(VehicleType.Sedan, 10)]
    public async Task AddVehicleWithDoorsNumberSpec_WithInvalidDoorsNumber_ShouldNotAddVehicleAsync(VehicleType vehicleType, int doorsNumber)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .BeOneOf(
                $"Minimum number of doors for {VehicleType.Hatchback} or {VehicleType.Sedan} is 3.",
                $"Maximum number of doors for {VehicleType.Hatchback} or {VehicleType.Sedan} is 5.");
    }

    [Theory]
    [InlineData(VehicleType.SUV, 10)]
    [InlineData(VehicleType.SUV, 0)]
    public async Task AddVehicleWithSeatsNumberSpec_WithInvalidSeatsNumber_ShouldNotAddVehicleAsync(VehicleType vehicleType, int seatsNumber)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, null, seatsNumber);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .BeOneOf(
                $"Minimum number of seats for a {VehicleType.SUV} is 5.",
                $"Maximum number of seats for a {VehicleType.SUV} is 8.");
    }

    [Theory]
    [InlineData(VehicleType.Truck, 0)]
    [InlineData(VehicleType.Truck, 100000f)]
    public async Task AddVehicleWithLoadCapacitySpec_WithInvalidLoadCapacity_ShouldNotAddVehicleAsync(VehicleType vehicleType, int loadCapacity)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, null, null, loadCapacity);
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .BeOneOf(
                $"Minimum load capacity for a {VehicleType.Truck} is 10000.",
                $"Maximum load capacity for a {VehicleType.Truck} is 50000.");
    }

    [Theory]
    [InlineData(VehicleType.Sedan, 3067, 5, null, null)]
    [InlineData(VehicleType.Hatchback, 3025, 5, null, null)]
    [InlineData(VehicleType.SUV, 1000, null, 6, null)]
    [InlineData(VehicleType.Truck, 2067, null, null, 10000f)]
    public async Task AddVehicle_WithInvalidYear_ShouldNotAddVehicleAsync(
        VehicleType vehicleType,
        int year,
        int? doorsNumber,
        int? seatsNumber,
        float? loadCapacity)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const string licensePlate = "12-AB-34";

        // Act
        var vehicle = new AddVehicleRequest(
            manufacturer,
            model,
            year,
            vehicleType,
            licensePlate,
            doorsNumber,
            seatsNumber,
            loadCapacity);

        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Invalid vehicle year");
    }

    [Fact]
    public async Task AddVehicle_WithVehicleWithSameLicensePlate_ShouldReturnErrorAsync()
    {
        // Arrange
        const int doorsNumber = 4;
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const VehicleType type = VehicleType.Sedan;
        const string licensePlate = "12-AB-34";
        var vehicle = new AddVehicleRequest(manufacturer, model, year, type, licensePlate, doorsNumber);
        _vehicleRepository
            .Setup(x => x.AnyAsync(It.IsAny<FindVehicleByLicensePlateSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        // Act
        var result = await _service.AddAsync(vehicle, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors[0].Message.Should().Be($"Vehicle with License Plate {licensePlate} already exists.");
    }
}