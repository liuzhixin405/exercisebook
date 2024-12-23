﻿using mfc.Core;
using sample.Model;

namespace sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //await ExcecuteOrder();

           // await InMemoryUser();
           await Logger();
        }

        private static async Task Logger()
        {
            var dbProvider = DatabaseProviderFactory.CreateProvider("inmemory");
            var dbContext = new MiniDbContext(dbProvider);
            // 订阅 SavingChanges 事件
            dbContext.SavingChanges += (sender, e) =>
            {
                Console.WriteLine("Saving changes...");
            };

            // 订阅 SavedChanges 事件
            dbContext.SavedChanges += (sender, e) =>
            {
                Console.WriteLine($"Saved changes successfully. Affected rows: {e.AffectedRows}");
            };
            dbContext.Set<User>().Add(new User { Id = 1, Name = "Alice" });
            await dbContext.SaveChangesAsync();
            var users = await dbContext.Set<User>().FindAsync(user => user.Id == 1);
        }

        private static async Task InMemoryUser()
        {
            var dbProvider = DatabaseProviderFactory.CreateProvider("inmemory");
            var dbContext = new MiniDbContext(dbProvider);
            dbContext.Set<User>().Add(new User { Id = 1, Name = "Alice" });
           await dbContext.SaveChangesAsync();
           var users =await dbContext.Set<User>().FindAsync(user => user.Id == 1);
        }
        private static async Task ExcecuteOrder()
        {
            var dbProvider = DatabaseProviderFactory.CreateProvider("mysql", "server=127.0.0.1;port=3307;user=root;password=123456;database=testdb;SslMode = none;AllowLoadLocalInfile=true");
            var dbContext = new MiniDbContext(dbProvider);

            //await AddOrder(dbContext);

            // 使用扩展方法进行查询
            var discountedOrders =await dbContext.Where<Orders>(order => order.Discount > 0);
            var firstOrder =await dbContext.FirstOrDefault<Orders>(order => order.Id == 1);
            var orderCount = await dbContext.Count<Orders>();
            var pagedOrders =await dbContext.SkipTake<Orders>(0, 10);

            // 打印查询结果
            Console.WriteLine($"Discounted Orders: {discountedOrders.Count()}");
            Console.WriteLine($"First Order: {firstOrder?.ProductName}");
            Console.WriteLine($"Total Orders: {orderCount}");
            Console.WriteLine($"Paged Orders: {pagedOrders.Count()}");
        }

        private static async Task AddOrder(MiniDbContext dbContext)
        {
            dbContext.Set<Orders>().Add(new Orders { Id = 5, ProductName = "Product1", Discount = 0.1, CreateTime = DateTime.Now });
            await dbContext.SaveChangesAsync();
        }

        private static async Task ExecutePerson()
        {
            // 创建模拟的数据库提供程序（例如：InMemory 数据库提供程序）
            var dbProvider = DatabaseProviderFactory.CreateProvider("inmemory");
            var context = new MiniDbContext(dbProvider);

            // 获取 DbSet
            var personSet = context.Set<Person>();

            // 添加新实体
            var person1 = new Person { Id = 1, Name = "Alice" };
            personSet.Add(person1);

            // 更新实体
            person1.Name = "Alice Updated";
            personSet.Update(person1);

            // 删除实体
            personSet.Remove(person1);

            // 保存更改（生成并执行 SQL）
            await context.SaveChangesAsync();

            // 执行查询
            var people =await personSet.ToListAsync();
            foreach (var person in people)
            {
                Console.WriteLine($"Person: {person.Name}");
            }

            // 在事务中执行操作
            await context.ExecuteInTransactionAsync(async () =>
            {
                var anotherPerson = new Person { Id = 2, Name = "Bob" };
                personSet.Add(anotherPerson);
                await context.SaveChangesAsync();
            });

            // 查看事务提交后的结果
            var peopleAfterTransaction =await personSet.ToListAsync();
            foreach (var person in peopleAfterTransaction)
            {
                Console.WriteLine($"Person after transaction: {person.Name}");
            }
        }

    }
}
