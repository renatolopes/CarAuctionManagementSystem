namespace CarAuctionManagementSystem.Application.Validators;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;
using FluentValidation;

public class VehicleValidator : AbstractValidator<AddVehicleRequest>
{
    public VehicleValidator()
    {
        RuleFor(x => x.Year)
            .LessThanOrEqualTo(DateTime.Now.Year)
            .GreaterThanOrEqualTo(1886) // Date of construction of the first modern vehicle Benz Patent-Motorwagen
            .WithMessage("Invalid vehicle year");

        RuleFor(x => x.DoorsNumber)
            .NotEmpty()
            .When(x => x.VehicleType is VehicleType.Hatchback or VehicleType.Sedan)
            .WithMessage($"Number of doors is mandatory for {VehicleType.Hatchback} or {VehicleType.Sedan}.");

        RuleFor(x => x.DoorsNumber)
            .GreaterThanOrEqualTo(3)
            .When(x => x.VehicleType is VehicleType.Hatchback or VehicleType.Sedan)
            .WithMessage($"Minimum number of doors for {VehicleType.Hatchback} or {VehicleType.Sedan} is 3.");

        RuleFor(x => x.DoorsNumber)
            .LessThanOrEqualTo(5)
            .When(x => x.VehicleType is VehicleType.Hatchback or VehicleType.Sedan)
            .WithMessage($"Maximum number of doors for {VehicleType.Hatchback} or {VehicleType.Sedan} is 5.");

        RuleFor(x => x.SeatsNumber)
            .NotEmpty()
            .When(x => x.VehicleType is VehicleType.SUV)
            .WithMessage($"Number of seats is mandatory for {VehicleType.SUV}.");

        RuleFor(x => x.SeatsNumber)
            .GreaterThanOrEqualTo(5)
            .When(x => x.VehicleType is VehicleType.SUV)
            .WithMessage($"Minimum number of seats for a {VehicleType.SUV} is 5.");

        RuleFor(x => x.SeatsNumber)
            .LessThanOrEqualTo(8)
            .When(x => x.VehicleType is VehicleType.SUV)
            .WithMessage($"Maximum number of seats for a {VehicleType.SUV} is 8.");

        RuleFor(x => x.LoadCapacity)
            .NotEmpty()
            .When(x => x.VehicleType is VehicleType.Truck)
            .WithMessage($"Load capacity is mandatory for {VehicleType.Truck}.");

        RuleFor(x => x.LoadCapacity)
            .GreaterThanOrEqualTo(10000)
            .When(x => x.VehicleType is VehicleType.Truck)
            .WithMessage($"Minimum load capacity for a {VehicleType.Truck} is 10000.");

        RuleFor(x => x.LoadCapacity)
            .LessThanOrEqualTo(50000)
            .When(x => x.VehicleType is VehicleType.Truck)
            .WithMessage($"Maximum load capacity for a {VehicleType.Truck} is 50000.");

        RuleFor(x => x.LoadCapacity)
            .Null()
            .When(x => x.VehicleType is VehicleType.Hatchback or VehicleType.Sedan or VehicleType.SUV)
            .WithMessage($"Load capacity is not allowed for {VehicleType.Hatchback}, {VehicleType.Sedan} or {VehicleType.SUV} vehicles.");

        RuleFor(x => x.SeatsNumber)
            .Null()
            .When(x => x.VehicleType is VehicleType.Hatchback or VehicleType.Sedan or VehicleType.Truck)
            .WithMessage($"Number of seats is not allowed for {VehicleType.Hatchback}, {VehicleType.Sedan} or {VehicleType.Truck} vehicles.");

        RuleFor(x => x.DoorsNumber)
            .Null()
            .When(x => x.VehicleType is VehicleType.SUV or VehicleType.Truck)
            .WithMessage($"Number of doors is not allowed for {VehicleType.SUV}, or {VehicleType.Truck} vehicles.");
    }
}