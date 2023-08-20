using AbsEFWork.Implementations.Dto;
using BaseEntityFramework.Implementations.Entitys;
using IServiceEF;
using IServiceEF.DefaultImplement;

namespace BaseEntityFramework.Controllers
{
    public class ProductController : CustomController<CreateProductDto,Product,GetByIdDto>
    {
        public ProductController(IEFCoreService efCoreService) : base(efCoreService)
        {
            //放弃
        }
    }
}
