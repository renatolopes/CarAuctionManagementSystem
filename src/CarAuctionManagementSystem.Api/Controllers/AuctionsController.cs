namespace CarAuctionManagementSystem.Api.Controllers;

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
    public IActionResult Add([FromBody] AddAuctionRequest request)
    {
        var result = _auctionsService.Add(request);

        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList()});
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    public IActionResult Get()
    {
        var result = _auctionsService.GetAllAuctions();

        return Ok(result);
    }

    [HttpPost("{auctionId}/start")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public IActionResult Start(string auctionId)
    {
        var result = _auctionsService.Start(auctionId);

        if (result.IsSuccess)
        {
            return Ok(new { Message = "Auction started successfully." });
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList() });
    }

    [HttpPost("{auctionId}/close")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public IActionResult Close(string auctionId)
    {
        var result = _auctionsService.Close(auctionId);

        if (result.IsSuccess)
        {
            return Ok(new { Message = "Auction closed successfully."});
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList()});
    }

    [HttpPost("{auctionId}/bid")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public IActionResult Bid(string auctionId, [FromBody] AddBidRequest request)
    {
        var result = _auctionsService.Bid(auctionId, request);

        if (result.IsSuccess)
        {
            return Ok(new { Message = "Bid placed successfully" });
        }

        return BadRequest(new { Message = result.Errors.Select(e => e.Message).ToList() });
    }
}