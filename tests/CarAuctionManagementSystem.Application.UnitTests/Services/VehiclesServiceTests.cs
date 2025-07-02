namespace CarAuctionManagementSystem.Application.UnitTests.Services;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Application.Services;
using CarAuctionManagementSystem.Application.Specifications.Vehicles;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class VehiclesServiceTests
{
    [Theory]
    [InlineData(VehicleType.Sedan, 5, null, null)]
    [InlineData(VehicleType.Hatchback, 5, null, null)]
    [InlineData(VehicleType.SUV, null, 6, null)]
    [InlineData(VehicleType.Truck, null, null, 10000f)]
    public void AddVehicle_WithValidInformation_ShouldAddVehicle(
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

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

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

        var result = vehiclesService.Add(vehicle);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
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
    public void AddVehicleWithDoorsNumberSpec_WithoutDoorsNumber_ShouldNotAddVehicle(VehicleType vehicleType)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of doors is mandatory for {VehicleType.Hatchback} or {VehicleType.Sedan}.");
    }

    [Fact]
    public void AddVehicleWithSeatsNumberSpec_WithoutSeatsNumber_ShouldNotAddVehicle()
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";
        const VehicleType vehicleType = VehicleType.SUV;

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of seats is mandatory for {VehicleType.SUV}.");
    }

    [Fact]
    public void AddVehicleWithLoadCapacitySpec_WithoutLoadCapacity_ShouldNotAddVehicle()
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";
        const VehicleType vehicleType = VehicleType.Truck;

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Load capacity is mandatory for {VehicleType.Truck}.");
    }

    [Theory]
    [InlineData(VehicleType.Sedan, 5, null)]
    [InlineData(VehicleType.Hatchback, 5, null)]
    [InlineData(VehicleType.SUV, null, 6)]
    public void AddVehicleWithoutLoadCapacitySpec_WithLoadCapacity_ShouldNotAddVehicle(
        VehicleType vehicleType,
        int? doorsNumber,
        int? seatsNumber)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber, seatsNumber, 10000);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Load capacity is not allowed for {VehicleType.Hatchback}, {VehicleType.Sedan} or {VehicleType.SUV} vehicles.");
    }

    [Theory]
    [InlineData(VehicleType.Hatchback, 5, 6, null)]
    [InlineData(VehicleType.Truck, null, 3, 10000f)]
    public void AddVehicleWithoutSeatsNumberSpec_WithSeatsNumber_ShouldNotAddVehicle(
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

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber, seatsNumber, loadCapacity);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of seats is not allowed for {VehicleType.Hatchback}, {VehicleType.Sedan} or {VehicleType.Truck} vehicles.");
    }

    [Theory]
    [InlineData(VehicleType.SUV, 5, 6, null)]
    [InlineData(VehicleType.Truck, 5, null, 10000f)]
    public void AddVehicleWithoutDoorsNumberSpec_WithDoorsNumber_ShouldNotAddVehicle(
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

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber, seatsNumber, loadCapacity);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be($"Number of doors is not allowed for {VehicleType.SUV}, or {VehicleType.Truck} vehicles.");
    }

    [Theory]
    [InlineData(VehicleType.Hatchback, 10)]
    [InlineData(VehicleType.Hatchback, 0)]
    [InlineData(VehicleType.Sedan, 0)]
    [InlineData(VehicleType.Sedan, 10)]
    public void AddVehicleWithDoorsNumberSpec_WithInvalidDoorsNumber_ShouldNotAddVehicle(VehicleType vehicleType, int doorsNumber)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, doorsNumber);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .BeOneOf(
                $"Minimum number of doors for {VehicleType.Hatchback} or {VehicleType.Sedan} is 3.",
                $"Maximum number of doors for {VehicleType.Hatchback} or {VehicleType.Sedan} is 5.");
    }

    [Theory]
    [InlineData(VehicleType.SUV, 10)]
    [InlineData(VehicleType.SUV, 0)]
    public void AddVehicleWithSeatsNumberSpec_WithInvalidSeatsNumber_ShouldNotAddVehicle(VehicleType vehicleType, int seatsNumber)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, null, seatsNumber);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .BeOneOf(
                $"Minimum number of seats for a {VehicleType.SUV} is 5.",
                $"Maximum number of seats for a {VehicleType.SUV} is 8.");
    }

    [Theory]
    [InlineData(VehicleType.Truck, 0)]
    [InlineData(VehicleType.Truck, 100000f)]
    public void AddVehicleWithLoadCapacitySpec_WithInvalidLoadCapacity_ShouldNotAddVehicle(VehicleType vehicleType, int loadCapacity)
    {
        // Arrange
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const string licensePlate = "12-AB-34";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, vehicleType, licensePlate, null, null, loadCapacity);
        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
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
    public void AddVehicle_WithInvalidYear_ShouldNotAddVehicle(
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

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

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

        var result = vehiclesService.Add(vehicle);

        // Assert
        var addedVehicle = vehiclesRepository.Find(new FindVehicleByLicensePlateSpec(licensePlate));
        addedVehicle.Should().BeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Invalid vehicle year");
    }

    [Fact]
    public void AddVehicle_WithVehicleWithSameLicensePlate_ShouldReturnError()
    {
        // Arrange
        const int doorsNumber = 4;
        const string manufacturer = "Manufacturer";
        const string model = "Model";
        const int year = 2025;
        const VehicleType type = VehicleType.Sedan;
        const string licensePlate = "12-AB-34";

        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(manufacturer, model, year, type, licensePlate, doorsNumber);
        vehiclesService.Add(vehicle);
        var result = vehiclesService.Add(vehicle);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors[0].Message.Should().Be($"Vehicle with License Plate {licensePlate} already exists.");
    }

    [Theory]
    [InlineData("Sedan", "manufacturer", "model", 2025, 1)]
    [InlineData(null, "manufacturer", "model", 2025, 1)]
    [InlineData("Sedan", null, "model", 2025, 1)]
    [InlineData("Sedan", "manufacturer", null, 2025, 1)]
    [InlineData("Sedan", "manufacturer", "model", null, 1)]
    [InlineData("Hatchback", "manufacturer", "model", null, 0)]
    [InlineData("Sedan", "manufacturer2", "model", null, 0)]
    [InlineData("Sedan", "manufacturer", "model2", 2025, 0)]
    [InlineData("Sedan", "manufacturer", "model", 2021, 0)]
    [InlineData(null, null, null, null, 1)]
    public void SearchVehicle_ShouldReturnResultsAsExpected(
    string? vehicleType,
    string? manufacturer,
    string? model,
    int? year,
    int expectedCountResult)
    {
        // Arrange
        var vehiclesRepository = new ServiceRepository<Vehicle>();
        var mockLogger = new Mock<ILogger<VehiclesService>>();

        var vehiclesService = new VehiclesService(vehiclesRepository, mockLogger.Object);

        // Act
        var vehicle = new AddVehicleRequest(
            "manufacturer",
            "model",
            2025,
            VehicleType.Sedan,
            "12-AB-34",
            5,
            null,
            null);
        vehiclesService.Add(vehicle);

        var result = vehiclesService.Search(
            !string.IsNullOrEmpty(vehicleType) ? Enum.Parse<VehicleType>(vehicleType) : null,
            manufacturer,
            model,
            year).ToList();

        // Assert
        result.Count.Should().Be(expectedCountResult);
    }
}