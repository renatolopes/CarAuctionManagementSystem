namespace CarAuctionManagementSystem.Application.Services;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Application.Interfaces;
using CarAuctionManagementSystem.Application.Mappers;
using CarAuctionManagementSystem.Application.Specifications.Vehicles;
using CarAuctionManagementSystem.Application.Validators;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

public class VehiclesService : IVehiclesService
{
    private readonly IServiceRepository<Vehicle> _vehiclesRepository;
    private readonly ILogger<VehiclesService> _logger;

    public VehiclesService(
        IServiceRepository<Vehicle> vehiclesRepository,
        ILogger<VehiclesService> logger)
    {
        _vehiclesRepository = vehiclesRepository;
        _logger = logger;
    }

    public Result<AvailableVehicle> Add(AddVehicleRequest vehicle)
    {
        if (SearchByLicensePlate(vehicle.LicensePlate))
        {
            return Result.Fail($"Vehicle with License Plate {vehicle.LicensePlate} already exists.");
        }

        var vehicleValidator = new VehicleValidator();
        var result = vehicleValidator.Validate(vehicle);

        if (result.IsValid)
        {
            var addedVehicle = _vehiclesRepository.Add(vehicle.MapToVehicle());

            _logger.LogInformation(
                "Added vehicle with license plate {LicensePlate} to inventory",
                addedVehicle.LicensePlate);

            return addedVehicle.MapToAvailableVehicle();
        }

        var errors = result.Errors
            .Select(x => x.ErrorMessage)
            .ToList();

        return Result.Fail<AvailableVehicle>(errors);
    }

    public IEnumerable<AvailableVehicle> Search(VehicleType? vehicleType, string? manufacturer, string? model, int? year)
    {
        var spec = new FindVehiclesByMultipleParameters(vehicleType, manufacturer, model, year);
        var vehicles = _vehiclesRepository.FindAll(spec).ToList();

        return vehicles.Select(x => x.MapToAvailableVehicle());
    }

    private bool SearchByLicensePlate(string licensePlate)
    {
        var spec = new FindVehicleByLicensePlateSpec(licensePlate);
        return _vehiclesRepository.Find(spec) is not null;
    }

}