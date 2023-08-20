using AbsEFWork.Implementations.Dto;
using AbsEFWork.Util;
using BaseEntityFramework.Implementations.Entitys;
using EntityEF.Dto;
using EntityEF.Models;
using IServiceEF;

namespace AbsEFWork.Implementations
{
    public class ProductService : IEFCoreService
    {
        private readonly IBaseRepository<Product> _repository;
        public ProductService(IBaseRepository<Product> eFCoreService)
        {
                _repository = eFCoreService;
        }
        public async Task<bool> Add(IRequestDto entity)
        {
            var query = (Product)entity;
            var newProduct = new Product
            {
                Name = query.Name,
                Description = query.Description??"无",
                Price = query.Price,
                CreateTime = Helper.GetLongTimeStamp(DateTime.UtcNow)
            };
            return await _repository.Add(newProduct);
        }

        public async Task<bool> Delete(IRequestDto entity)
        {
            return await _repository.Delete((Product)entity);
        }


        public async Task<IResponseDto> GetEntity(IRequestDto requestDto)
        {
           var result = await _repository.GetEntity(x=>x.Id.Equals(((GetByIdDto)requestDto).Id));

            return result==null ?default : new Product { Id = result.Id, Description = result.Description, Name= result.Name,Price =result.Price }; 
        }

        public async Task<IReadOnlyCollection<IResponseDto>> GetList(IRequestDto requestDto)
        {
            if(requestDto is GetByIdDto)
            {
                return await _repository.GetAsync(x => x.Id == ((GetByIdDto)requestDto).Id,q=>q.OrderByDescending(x=>x.CreateTime),"",true);
            }
            return default;
        }

        public async Task<PageResult<IResponseDto>> GetPageResult<IRequestDto, IResponseDto>(PageInput<IRequestDto> pageInput) where IRequestDto : new() where IResponseDto : new()
        {
            var result =await _repository.GetPageResult(pageInput);
            return result as PageResult<IResponseDto>;
        }

        public async Task<bool> Update(IRequestDto entity)
        {
            return await _repository.Update((Product)entity);
        }
    }
}
