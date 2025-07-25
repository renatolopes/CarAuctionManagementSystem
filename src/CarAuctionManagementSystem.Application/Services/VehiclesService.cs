namespace CarAuctionManagementSystem.Application.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Application.Interfaces;
using CarAuctionManagementSystem.Application.Mappers;
using CarAuctionManagementSystem.Application.Specifications.Vehicles;
using CarAuctionManagementSystem.Application.Validators;
using CarAuctionManagementSystem.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;

public class VehiclesService : IVehiclesService
{
    private readonly IRepository<Vehicle> _vehiclesRepository;
    private readonly ILogger<VehiclesService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public VehiclesService(
        IRepository<Vehicle> vehiclesRepository,
        ILogger<VehiclesService> logger,
        IUnitOfWork unitOfWork)
    {
        _vehiclesRepository = vehiclesRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AvailableVehicle>> AddAsync(AddVehicleRequest vehicle, CancellationToken cancellationToken)
    {
        var vehicleAlreadyExists = await _vehiclesRepository.AnyAsync(new FindVehicleByLicensePlateSpec(vehicle.LicensePlate), cancellationToken);
        if (vehicleAlreadyExists)
        {
            return Result.Fail<AvailableVehicle>(new Error($"Vehicle with License Plate {vehicle.LicensePlate} already exists.").CausedBy("LicensePlate"));
        }

        var vehicleValidator = new VehicleValidator();
        var result = vehicleValidator.Validate(vehicle);

        if (result.IsValid)
        {
            var newVehicle = vehicle.MapToVehicle();
            _vehiclesRepository.Add(newVehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Added vehicle with license plate {LicensePlate} to inventory",
                newVehicle.LicensePlate);

            return newVehicle.MapToAvailableVehicle();
        }

        var errors = result.Errors
            .Select(x => x.ErrorMessage)
            .ToList();

        return Result.Fail<AvailableVehicle>(errors);
    }

    public async Task<IEnumerable<AvailableVehicle>> SearchAsync(VehicleType? vehicleType, string? manufacturer, string? model, int? year, CancellationToken cancellationToken)
    {
        var spec = new FindVehiclesByMultipleParameters(vehicleType, manufacturer, model, year);
        var vehicles = await _vehiclesRepository.FindAsync(spec, cancellationToken);

        return vehicles.Select(x => x.MapToAvailableVehicle());
    }
}