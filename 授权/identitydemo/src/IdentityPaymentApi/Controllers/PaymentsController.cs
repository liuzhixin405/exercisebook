using IdentityPaymentApi.Application.DTOs;
using IdentityPaymentApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BusinessUser = IdentityPaymentApi.Domain.Models.BusinessUser;
using AppUser = IdentityPaymentApi.Models.ApplicationUser;

namespace IdentityPaymentApi.Controllers;

[Authorize(Policy = "Payments")]
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

    private static BusinessUser ToBusinessUser(AppUser user)
        => new() { Id = user.Id ?? string.Empty, UserName = user.UserName };

    [HttpPost]
    public async Task<IActionResult> CreatePayment(PaymentRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var businessUser = ToBusinessUser(user);
        var record = await _paymentService.CreatePaymentAsync(businessUser, request);
        var response = new PaymentResponse
        {
            Id = record.Id,
            Amount = record.Amount,
            Currency = record.Currency,
            Method = record.Method.ToString(),
            Description = record.Description,
            Status = record.Status,
            CreatedAt = record.CreatedAt
        };

        return CreatedAtAction(nameof(GetPayment), new { id = record.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var businessUser = ToBusinessUser(user);
        var payments = await _paymentService.GetPaymentsAsync(businessUser);
        return Ok(payments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var businessUser = ToBusinessUser(user);
        var payment = await _paymentService.GetPaymentAsync(businessUser, id);
        if (payment == null)
        {
            return NotFound();
        }

        return Ok(payment);
    }
}