using System.Linq;
using IdentityPaymentApi.Application.DTOs;
using IdentityPaymentApi.Application.Services;
using IdentityPaymentApi.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BusinessUser = IdentityPaymentApi.Domain.Models.BusinessUser;
using AppUser = IdentityPaymentApi.Models.ApplicationUser;

namespace IdentityPaymentApi.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly UserManager<AppUser> _userManager;

    public PaymentsController(IPaymentService paymentService, UserManager<AppUser> userManager)
    {
        _paymentService = paymentService;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize(Policy = "payments.create")]
    public async Task<IActionResult> CreatePayment(PaymentCreateRequest request)
    {
        var businessUser = await GetBusinessUserAsync();
        if (businessUser == null)
        {
            return Unauthorized();
        }

        var result = await _paymentService.CreatePaymentAsync(businessUser, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetPayment), new { id = result.Data!.Id }, result);
    }

    [HttpGet]
    [Authorize(Policy = "payments.read")]
    public async Task<IActionResult> GetPayments([FromQuery] PaymentQueryParameters query)
    {
        var businessUser = await GetBusinessUserAsync();
        if (businessUser == null)
        {
            return Unauthorized();
        }

        var response = await _paymentService.GetPaymentsAsync(businessUser, query);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "payments.view")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var businessUser = await GetBusinessUserAsync();
        if (businessUser == null)
        {
            return Unauthorized();
        }

        var result = await _paymentService.GetPaymentAsync(businessUser, id);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "payments.manage")]
    public async Task<IActionResult> UpdatePaymentStatus(Guid id, PaymentStatusUpdateRequest request)
    {
        var businessUser = await GetBusinessUserAsync();
        if (businessUser == null)
        {
            return Unauthorized();
        }

        var result = await _paymentService.UpdatePaymentStatusAsync(businessUser, id, request);
        return HandleResult(result);
    }

    private IActionResult HandleResult(BaseResult<PaymentResponse> result)
    {
        if (result.Success)
        {
            return Ok(result);
        }

        if (result.Errors?.Any(e => e.ErrorCode == ErrorCode.NotFound) == true)
        {
            return NotFound(result);
        }

        return BadRequest(result);
    }

    private async Task<BusinessUser?> GetBusinessUserAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return null;
        }

        return new BusinessUser
        {
            Id = user.Id ?? string.Empty,
            UserName = user.UserName
        };
    }
}