using MediatR;
using spot.Application.Wrappers;
using spot.Domain.Accounts.Entities;
using System.Collections.Generic;

namespace spot.Application.Features.Accounts.Queries.GetAccountsByUserId
{
    public class GetAccountsByUserIdQuery : IRequest<Response<List<Account>>>
    {
        public string UserId { get; set; }
    }
}