using MediatR;
using spot.Application.Interfaces;
using spot.Application.Interfaces.Repositories.Accounts;
using spot.Application.Wrappers;
using spot.Domain.Accounts.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Features.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Response<Account>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<Account>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if account already exists for this user and currency
                var existingAccount = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(request.UserId, request.Currency);
                if (existingAccount != null)
                {
                    return new Response<Account>($"Account already exists for user {request.UserId} and currency {request.Currency}");
                }

                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedAt = now,
                    UpdatedAt = now,
                    UserId = request.UserId,
                    Currency = request.Currency,
                    Available = request.InitialBalance,
                    Hold = 0
                };

                var createdAccount = await _accountRepository.AddAsync(account);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new Response<Account>(createdAccount);
            }
            catch (Exception ex)
            {
                return new Response<Account>($"An error occurred while creating the account: {ex.Message}");
            }
        }
    }
}