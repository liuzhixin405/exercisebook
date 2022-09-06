
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoComponent.Models;

namespace TodoComponent.ViewComponents;
public class PriorityListViewComponentFinal:ViewComponent
{
    private readonly ToDoContext db;

    public PriorityListViewComponentFinal(ToDoContext context)
    {
        db = context;
    }

    private Task<List<ToDoItem>> GetItemsAsync(int maxPriority, bool isDone)
    {
        return db.ToDo.Where(x => x.IsDone == isDone &&
                             x.Priority <= maxPriority).ToListAsync();
    }

    public async Task<IViewComponentResult> InvokeAsync(int maxPriority,bool isDone)
    {
        string MyView = "Default";
        if(maxPriority > 3 && isDone == true)
        {
            MyView = "PVC";
        }
        var items = await GetItemsAsync(maxPriority, isDone);
        return View(MyView,items);
    }
}
