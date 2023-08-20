using BaseEntityFramework.Implementations.Entitys;
using IServiceEF;
using Microsoft.AspNetCore.Mvc;

namespace AbsEFWork.Controllers
{
    public class TestController : AbsEfWorkController<Product>
    {
        public TestController(IEFCoreService eFCoreService)
        {
            //TODO 剔除UltramanController
        }

        protected override Task<IActionResult> CreateOrUpdate(Product product)
        {
            throw new NotImplementedException();
        }

        protected override Task<IActionResult> Delete<KeyType>(KeyType entity)
        {
            throw new NotImplementedException();
        }

        protected override Task<IActionResult> PageResult()
        {
            throw new NotImplementedException();
        }

        protected override Task<IActionResult> Search<KeyType>(KeyType searchId)
        {
            throw new NotImplementedException();
        }
    }
}
