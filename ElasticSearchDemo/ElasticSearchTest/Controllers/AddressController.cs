using ElasticSearchTest.Model;
using ElasticSearchTest.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private AddressContext _addressContext;
        public AddressController(AddressContext addressContext)
        {
            _addressContext = addressContext;
        }

        [HttpPost("添加地址")]
        public bool AddAddress(List<Address> addressList)
        {
            if (addressList == null || addressList.Count < 1)
            {
                return false;
            }
           return _addressContext.InsertMany(addressList);
        }
        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id"></param>
        [HttpPost("deleteAddress")]
        public void DeleteAdress(string id)
        {
            _addressContext.DeleteById(id);
        }
        /// <summary>
        /// 获取所有与地址
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllAddress")]
        public List<Address> GetAllAddress()
        {
            return _addressContext.GetAllAddresses();
        }
        /// <summary>
        /// 获取地址总数
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAddressTotalCount")]
        public long GetAddressTotalCount()
        {
            return _addressContext.GetTotalCount();
        }

        /// <summary>
        /// 分页获取（可以进一步封装查询条件）
        /// </summary>
        /// <param name="province"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("getAddressByProvince")]
        public List<Address> GetAddressByProvince(string province, int pageIndex, int pageSize)
        {
            return _addressContext.GetAddresses(province, pageIndex, pageSize);
        }

    }
}
