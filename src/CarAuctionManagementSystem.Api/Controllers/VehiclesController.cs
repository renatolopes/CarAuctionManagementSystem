namespace CarAuctionManagementSystem.Api.Controllers;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Application.Interfaces;
using CarAuctionManagementSystem.Domain;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehiclesService _vehiclesService;

    public VehiclesController(IVehiclesService vehiclesService)
    {
        _vehiclesService = vehiclesService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public IActionResult Add([FromBody] AddVehicleRequest request)
    {
        var result = _vehiclesService.Add(request);

        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList()});
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    public IActionResult Search(
        [FromQuery] VehicleType? vehicleType,
        [FromQuery] string? manufacturer,
        [FromQuery] string? model,
        [FromQuery] int? year)
    {
        var result = _vehiclesService.Search(
            vehicleType,
            manufacturer,
            model,
            year);

        return Ok(result);
    }
}