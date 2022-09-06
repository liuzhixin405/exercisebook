
using Microsoft.EntityFrameworkCore;

namespace TodoComponent.Models;
public class ToDoContext:DbContext
{
    public ToDoContext(DbContextOptions<ToDoContext> contextOptions):base(contextOptions)
    {

    }
    public DbSet<ToDoItem> ToDo { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        for (int i = 0; i < 9; i++)
        {
            modelBuilder.Entity<ToDoItem>().HasData(new ToDoItem {
                Id = i + 1,
                IsDone = i % 3 == 0,
                Name = "Task " + (i + 1),
                Priority = i % 5 + 1
            });
        }
        
    }
}
