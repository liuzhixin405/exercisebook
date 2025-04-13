using System.Threading.Tasks;
using spot.Application.DTOs.Account.Requests;
using spot.Application.DTOs.Account.Responses;
using spot.Application.Wrappers;

namespace spot.Application.Interfaces.UserInterfaces
{
    public interface IGetUserServices
    {
        Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model);
    }
}
