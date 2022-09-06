using AspNetCoreFilters.AttributeModel;
using AspNetCoreFilters.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreFilters.Controllers
{
    [TypeFilter(typeof(CustomExceptionFilter))]
    [Route("[controller]")]
    public class FailingController : Controller
    {
        [AddHeader("Failing Controller",
            "Won't appear when exception is handled")]
        [Route("Index")]
        public IActionResult Index()
        {
            throw new Exception("Testing custom exception filter.");
        }
        [Route("CustomError")]
        public IActionResult CustomError()
        {
            return View();
        }
        [Route("Empty")]
        [TypeFilter(typeof(MyAsyncResponseFilter))]
        public IActionResult Empty()
        {
            return View();
        }
    }
}
