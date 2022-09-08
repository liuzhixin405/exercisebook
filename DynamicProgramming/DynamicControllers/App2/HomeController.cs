using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Reflection;

namespace App
{
public class HomeController : Controller
{

    [HttpGet("/")]
    public IActionResult Index() => View();
    [HttpPost("/")]
    public IActionResult Index(string source,
        [FromServices] ApplicationPartManager manager,
        [FromServices] ICompiler compiler,
        [FromServices] DynamicChangeTokenProvider tokenProvider)
    {
        try
        {
            manager.ApplicationParts.Add(new AssemblyPart(compiler.Compile(source, Assembly.Load(new AssemblyName("System.Runtime")),
                typeof(object).Assembly,
                typeof(ControllerBase).Assembly,
                typeof(Controller).Assembly)));
            tokenProvider.NotifyChanges();
            return Content("OK");
        }
        catch (Exception ex)
        {
            return Content(ex.Message);
        }
    }
}
}
