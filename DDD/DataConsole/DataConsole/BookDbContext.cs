using DataConsole.Model.BookModel;
using Microsoft.EntityFrameworkCore;

namespace DataConsole
{
    public class BookDbContext:DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options):base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        //虽然没有设置author表，数据库依然会生成,作为bookid外键自动识别
    }
}
