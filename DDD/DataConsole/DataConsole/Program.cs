using DataConsole.Model.BookModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ContextFacade>(options => options.UseInMemoryDatabase("inmdb_01"));
            builder.Services.AddDbContext<QueryModelDatabase>(options => options.UseInMemoryDatabase("inmdb_02"));
            builder.Services.AddDbContext<BookDbContext>(options => options.UseSqlServer("Data Source=PC-202205262203;Initial Catalog=book_app04;Persist Security Info=False;User ID=sa;Password=1230;MultipleActiveResultSets=true"));
            builder.Services.AddControllers();
            var host = builder.Build();
            host.MapGet("/", context =>
            {
                using var scope = host.Services.CreateAsyncScope();
                {
                    var bookContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
                    if (!(bookContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    {
                        bookContext.Database.EnsureDeleted();
                        bookContext.Database.EnsureCreated();
                        if (!bookContext.Books.Any())
                        {
                            var martinFowler = new Author
                            {
                                Name = "Martin Fowler",
                                WebUrl = "http://martinfowler.com/"
                            };

                            var books = new List<Book>
            {
                new Book
                {
                    Title = "Refactoring",
                    Description = "Improving the design of existing code",
                    PublishedOn = new DateTime(1999, 7, 8),
                    Author = martinFowler
                },
                new Book
                {
                    Title = "Patterns of Enterprise Application Architecture",
                    Description = "Written in direct response to the stiff challenges",
                    PublishedOn = new DateTime(2002, 11, 15),
                    Author = martinFowler
                },
                new Book
                {
                    Title = "Domain-Driven Design",
                    Description = "Linking business needs to software design",
                    PublishedOn = new DateTime(2003, 8, 30),
                    Author = new Author {Name = "Eric Evans", WebUrl = "http://domainlanguage.com/"}
                },
                new Book
                {
                    Title = "Quantum Networking",
                    Description = "Entangled quantum networking provides faster-than-light data communications",
                    PublishedOn = new DateTime(2057, 1, 1),
                    Author = new Author {Name = "Future Person"}
                }
            };

                            bookContext.Books.AddRange(books);
                            bookContext.SaveChanges();
                        }
                    }
                }

                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ContextFacade>();
                    dbContext.Add<Customer>(new Customer
                    {
                        Id = 1,
                        Name
                    = "Jack"
                    });
                    dbContext.Order.Add(new Order { Id = 1, CustomerId = 1 });
                    dbContext.SaveChanges();

                    var customer = dbContext.Find<Customer>(new object[] { 1 });
                    return context.Response.WriteAsync($"hello world,{customer?.Name},orderid={dbContext.Order?.FirstOrDefault()?.Id ?? 0}");
                }
            });

            host.Run();
        }
    }
}