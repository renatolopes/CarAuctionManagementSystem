namespace CarAuctionManagementSystem.Api.IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class AuctionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;


    public AuctionsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        IServiceRepository<Auction> auctionRepository = _factory.Services.GetRequiredService<IServiceRepository<Auction>>();
        IServiceRepository<Vehicle> vehicleRepository = _factory.Services.GetRequiredService<IServiceRepository<Vehicle>>();

        auctionRepository.DeleteAll();
        vehicleRepository.DeleteAll();
    }

    [Fact]
    public async Task PostAuction_ReturnsCreatedStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var licensePlate = "12-AB-34";
        var auctionRequest = Fixtures.AddAuctionFixture.GetAddAuction(licensePlate: licensePlate);
        var vehicleRequest = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: licensePlate);

        await client.PostAsJsonAsync("api/v1/vehicles", vehicleRequest);

        // Act
        var response = await client.PostAsJsonAsync("api/v1/auctions", auctionRequest);
        var content = await response.Content.ReadFromJsonAsync<AvailableAuction>(GetJsonSerializerOptions());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content!.Id.Should().NotBeNull();
        content.StartDate.Should().BeNull();
        content.CloseDate.Should().BeNull();
        content.Active.Should().BeFalse();
        content.StartingBid.Should().BeGreaterThan(0);
        content.Bids.Count().Should().Be(0);
        content.Vehicle.LicensePlate.Should().Be(licensePlate);
    }

    [Fact]
    public async Task PostAuction_VehicleWithExistentAuction_ReturnsBadRequest()
        {
        // Arrange
        var client = _factory.CreateClient();
        var licensePlate = "12-AB-34";
        var auctionRequest = Fixtures.AddAuctionFixture.GetAddAuction(licensePlate: licensePlate);
        var vehicleRequest = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: licensePlate);

        await client.PostAsJsonAsync("api/v1/vehicles", vehicleRequest);
        await client.PostAsJsonAsync("api/v1/auctions", auctionRequest);

        // Act
        var response = await client.PostAsJsonAsync("api/v1/auctions", auctionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    [Fact]
    public async Task GetAuctions_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var licensePlate = "12-AB-34";
        var auctionRequest = Fixtures.AddAuctionFixture.GetAddAuction(licensePlate: licensePlate);
        var vehicleRequest = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: licensePlate);

        await client.PostAsJsonAsync("api/v1/vehicles", vehicleRequest);
        await client.PostAsJsonAsync("api/v1/auctions", auctionRequest);

        // Act
        var response = await client.GetAsync("api/v1/auctions/");
        var content = await response.Content.ReadFromJsonAsync<IEnumerable<AvailableAuction>>(GetJsonSerializerOptions());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content!.Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task StartAuction_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var licensePlate = "12-AB-34";
        var auctionRequest = Fixtures.AddAuctionFixture.GetAddAuction(licensePlate: licensePlate);
        var vehicleRequest = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: licensePlate);

        await client.PostAsJsonAsync("api/v1/vehicles/", vehicleRequest);
        var auctionResponse = await client.PostAsJsonAsync("api/v1/auctions/", auctionRequest);

        var auctionContent = await auctionResponse.Content.ReadFromJsonAsync<AvailableAuction>(GetJsonSerializerOptions());

        // Act
        var response = await client.PostAsync($"api/v1/auctions/{auctionContent!.Id}/start/", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StartAuction_UnexistentVehicle_ReturnsNotFound()
        {
        // Arrange
        var client = _factory.CreateClient();
        var auctionRequest = Fixtures.AddAuctionFixture.GetAddAuction(licensePlate: "12-AB-34");
        var vehicleRequest = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: "12-CD-34");

        await client.PostAsJsonAsync("api/v1/vehicles/", vehicleRequest);
        var auctionResponse = await client.PostAsJsonAsync("api/v1/auctions/", auctionRequest);

        var auctionContent = await auctionResponse.Content.ReadFromJsonAsync<AvailableAuction>(GetJsonSerializerOptions());

        // Act
        var response = await client.PostAsync($"api/v1/auctions/{auctionContent!.Id}/start/", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    [Fact]
    public async Task BidAuction_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var licensePlate = "12-AB-34";
        var auctionRequest = Fixtures.AddAuctionFixture.GetAddAuction(licensePlate: licensePlate);
        var vehicleRequest = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: licensePlate);

        await client.PostAsJsonAsync("api/v1/vehicles/", vehicleRequest);
        var auctionResponse = await client.PostAsJsonAsync("api/v1/auctions/", auctionRequest);

        var auctionContent = await auctionResponse.Content.ReadFromJsonAsync<AvailableAuction>(GetJsonSerializerOptions());
        var auctionId = auctionContent!.Id;

        var bidRequest = Fixtures.AddBidFixture.GetAddBid(auctionId);
        await client.PostAsync($"api/v1/auctions/{auctionId}/start/", null);

        // Act
        var response = await client.PostAsJsonAsync($"api/v1/auctions/{auctionId}/bid/", bidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CloseAuction_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var licensePlate = "12-AB-34";
        var auctionRequest = Fixtures.AddAuctionFixture.GetAddAuction(licensePlate: licensePlate);
        var vehicleRequest = Fixtures.AddVehicleFixture.GetAddVehicle(licensePlate: licensePlate);

        await client.PostAsJsonAsync("api/v1/vehicles/", vehicleRequest);
        var auctionResponse = await client.PostAsJsonAsync("api/v1/auctions/", auctionRequest);

        var auctionContent = await auctionResponse.Content.ReadFromJsonAsync<AvailableAuction>(GetJsonSerializerOptions());
        await client.PostAsync($"api/v1/auctions/{auctionContent!.Id}/start/", null);

        // Act
        var response = await client.PostAsync($"api/v1/auctions/{auctionContent!.Id}/close/", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        return jsonOptions;
    }
}