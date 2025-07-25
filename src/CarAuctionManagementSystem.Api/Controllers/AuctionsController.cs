namespace CarAuctionManagementSystem.Api.Controllers;

using System.Threading.Tasks;
using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Bids;
using CarAuctionManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionsService _auctionsService;

    public AuctionsController(IAuctionsService auctionsService)
    {
        _auctionsService = auctionsService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public async Task<IActionResult> AddAsync([FromBody] AddAuctionRequest request, CancellationToken cancellationToken)
    {
        var result = await _auctionsService.AddAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList()});
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var result = await _auctionsService.GetAllAuctionsAsync(cancellationToken);

        return Ok(result);
    }

    [HttpPost("{auctionId}/start")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public async Task<ActionResult> StartAsync(string auctionId, CancellationToken cancellationToken)
    {
        var result = await _auctionsService.StartAsync(auctionId, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { Message = "Auction started successfully." });
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList() });
    }

    [HttpPost("{auctionId}/close")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public async Task<IActionResult> CloseAsync(string auctionId, CancellationToken cancellationToken)
    {
        var result = await _auctionsService.CloseAsync(auctionId, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { Message = "Auction closed successfully."});
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList()});
    }

    [HttpPost("{auctionId}/bid")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public async Task<IActionResult> BidAsync(string auctionId, [FromBody] AddBidRequest request, CancellationToken cancellationToken)
    {
        var result = await _auctionsService.BidAsync(auctionId, request, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { Message = "Bid placed successfully" });
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList() });
    }
}