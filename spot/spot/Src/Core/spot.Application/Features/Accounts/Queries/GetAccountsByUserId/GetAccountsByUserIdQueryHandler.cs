using MediatR;
using spot.Application.Interfaces.Repositories.Accounts;
using spot.Application.Wrappers;
using spot.Domain.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Features.Accounts.Queries.GetAccountsByUserId
{
    public class GetAccountsByUserIdQueryHandler : IRequestHandler<GetAccountsByUserIdQuery, Response<List<Account>>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountsByUserIdQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Response<List<Account>>> Handle(GetAccountsByUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var accounts = await _accountRepository.GetAccountsByUserIdAsync(request.UserId);
                return new Response<List<Account>>(accounts.ToList());
            }
            catch (Exception ex)
            {
                return new Response<List<Account>>($"An error occurred while retrieving accounts: {ex.Message}");
            }
        }
    }
}