using EntityEF.Dto;
using EntityEF.Models;
using System.Linq.Expressions;

namespace IServiceEF;

public interface IEFCoreService
{

    Task<bool> Add(IRequestDto entity);
    Task<bool> Delete(IRequestDto entity);
    Task<bool> Update(IRequestDto entity);
   
    Task<PageResult<IResponseDto>> GetPageResult<IRequestDto, IResponseDto>(PageInput<IRequestDto> pagInput) where IRequestDto : new() where IResponseDto:new();
    Task<IResponseDto> GetEntity(IRequestDto requestDto);
   
}
