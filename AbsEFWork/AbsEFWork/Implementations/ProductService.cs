using AbsEFWork.Implementations.Dto;
using AbsEFWork.Util;
using BaseEntityFramework.Implementations;
using BaseEntityFramework.Implementations.Entitys;
using EntityEF.Dto;
using EntityEF.Models;
using IServiceEF;
using System.Linq.Expressions;

namespace AbsEFWork.Implementations
{
    public class ProductService : IEFCoreService
    {

        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Add(IRequestDto entity)
        {
            var query = (Product)entity;
            var newProduct = new Product
            {
                Name = query.Name,
                Description = query.Description ?? "无",
                Price = query.Price,
                CreateTime = Helper.GetLongTimeStamp(DateTime.UtcNow)
            };
            await _unitOfWork.GetRepository<Product>().Add(newProduct);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }

        public async Task<bool> Delete(IRequestDto entity)
        {
            return await _unitOfWork.GetRepository<Product>().Delete(entity);
        }


        public async Task<IResponseDto> GetEntity(IRequestDto requestDto)
        {
            var id = ((GetByIdDto)requestDto).Id;
            var result = await _unitOfWork.GetRepository<Product>().GetEntity(x => x.Id.Equals(id));

            return result == null ? default : new Product { Id = result.Id, Description = result.Description, Name = result.Name, Price = result.Price };
        }

        public async Task<IReadOnlyCollection<IResponseDto>> GetList(IRequestDto requestDto)
        {
            Expression<Func<Product, bool>> expression = x => true;
            //条件
            return await _unitOfWork.GetRepository<Product>().GetAsync(expression, q => q.OrderByDescending(x => x.CreateTime), "", true);
        }

        public async Task<PageResult<IResponseDto>> GetPageResult<IRequestDto, IResponseDto>(PageInput<IRequestDto> pageInput) where IRequestDto : new() where IResponseDto : new()
        {
            var result = await _unitOfWork.GetRepository<Product>().GetPageResult(pageInput);
            return result as PageResult<IResponseDto>;
        }

        public async Task<bool> Update(IRequestDto entity)
        {
            await _unitOfWork.GetRepository<Product>().Update((Product)entity);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }

        public async Task ExtraMethod(int id)
        {
            await _unitOfWork.GetRepository<Product>().GetByIdExtendAsync(id);  //repository扩展方法
        }
    }
}
