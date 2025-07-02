namespace CarAuctionManagementSystem.Api.IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class VehiclesControllerTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public VehiclesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;

        IServiceRepository<Vehicle> vehicleRepository = _factory.Services.GetRequiredService<IServiceRepository<Vehicle>>();

        vehicleRepository.DeleteAll();
    }

    [Fact]
    public async Task PostVehicle_ReturnsCreatedStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var licensePlate = "licensePlate";
        var request = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: licensePlate);

        // Act
        var response = await client.PostAsJsonAsync("api/v1/vehicles", request);
        var content = await response.Content.ReadFromJsonAsync<AvailableVehicle>(GetJsonSerializerOptions());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content!.Type.Should().Be(request.VehicleType);
        content!.Manufacturer.Should().Be(request.Manufacturer);
        content!.Model.Should().Be(request.Model);
        content!.Year.Should().Be(request.Year);
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
    public async Task SearchVehicle_ShouldReturnAsExpected(
        string? vehicleType,
        string? manufacturer,
        string? model,
        int? year,
        int expectedCountResult)
        {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var request = Fixtures.AddVehicleFixture.GetAddVehicle();
        await client.PostAsJsonAsync("api/v1/vehicles", request);

        var requestUrl = $"api/v1/vehicles/search/?";
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(vehicleType))
            {
            queryParams.Add($"vehicleType={Enum.Parse<VehicleType>(vehicleType)}");
            }

        if (!string.IsNullOrEmpty(manufacturer))
        {
            queryParams.Add($"manufacturer={manufacturer}");
        }

        if (!string.IsNullOrEmpty(model))
        {
            queryParams.Add($"model={model}");
        }

        if (year.HasValue)
        {
            queryParams.Add($"year={year}");
        }

        if (queryParams.Count > 0)
        {
        requestUrl += string.Join("&", queryParams.ToArray());
        }

        var response = await client.GetAsync(requestUrl);
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<AvailableVehicle>>(GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        content!.Count().Should().Be(expectedCountResult);
        if (expectedCountResult > 0)
        {
            content!.First().Type.Should().Be(request.VehicleType);
            content!.First().Manufacturer.Should().Be(request.Manufacturer);
            content!.First().Model.Should().Be(request.Model);
            content!.First().Year.Should().Be(request.Year);
        }
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return jsonOptions;
    }

}