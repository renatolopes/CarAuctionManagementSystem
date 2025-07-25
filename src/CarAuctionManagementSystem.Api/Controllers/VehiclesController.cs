namespace CarAuctionManagementSystem.Api.Controllers;

using System.Threading.Tasks;
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
    public async Task<IActionResult> AddAsync([FromBody] AddVehicleRequest request, CancellationToken cancellationToken)
    {
        var result = await _vehiclesService.AddAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList()});
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] VehicleType? vehicleType,
        [FromQuery] string? manufacturer,
        [FromQuery] string? model,
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var result = await _vehiclesService.SearchAsync(
            vehicleType,
            manufacturer,
            model,
            year,
            cancellationToken);

        return Ok(result);
    }
}