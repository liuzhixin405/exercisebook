
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoComponent.Models;

namespace TodoComponent.ViewComponents;
public class PriorityList:ViewComponent
{
    protected readonly ToDoContext _toDoContext;
    public PriorityList(ToDoContext toDoContext)
    {
        _toDoContext = toDoContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(int maxPriority,bool isDone)
    {
        var items = await GetItemsAsync(maxPriority, isDone);
        return View(items);
    }

    private Task<List<ToDoItem>> GetItemsAsync(int maxPriority, bool isDone)
    {
        return _toDoContext.ToDo.Where(x => x.IsDone == isDone && x.Priority <= maxPriority).ToListAsync();
    }
}
