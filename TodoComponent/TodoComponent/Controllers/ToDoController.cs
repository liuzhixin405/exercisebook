using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoComponent.Models;

namespace TodoComponent.Controllers;
public class ToDoController : Controller
{
    public readonly ToDoContext _ToDoContext;
    public ToDoController(ToDoContext todoContext)
    {
        _ToDoContext = todoContext;
        _ToDoContext.Database.EnsureCreated();
    }
    public IActionResult Index()
    {
        var model = _ToDoContext.ToDo.ToList();
        return View(model);
    }
    public string Index2()
    {
        return "View()";
    }
    public async Task<IActionResult> IndexFinal()
    {
        return View(await _ToDoContext.ToDo.ToListAsync());
    }
}
