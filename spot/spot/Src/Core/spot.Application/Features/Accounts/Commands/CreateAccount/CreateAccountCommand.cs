using MediatR;
using spot.Application.Wrappers;
using spot.Domain.Accounts.Entities;

namespace spot.Application.Features.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommand : IRequest<Response<Account>>
    {
        public string UserId { get; set; }
        public string Currency { get; set; }
        public decimal InitialBalance { get; set; } = 0;
    }
}