using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EF.GraphQL.Demo
{
    public class Query
    {
        // 获取所有员工列表，并支持分页、排序和过滤
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Employee> GetEmployees([Service] CustomDbContext dbContext) =>
            dbContext.Employees.Include(e => e.Addresses).OrderBy(e=>e.DateOfBirth);

        // 根据 ID 获取单个员工及其地址
        public Employee GetEmployee(int id, [Service] CustomDbContext dbContext) =>
            dbContext.Employees.Include(e => e.Addresses).FirstOrDefault(e => e.Id == id);

        // 获取员工的某个特定地址
        public Address GetAddress(int id, [Service] CustomDbContext dbContext) =>
            dbContext.Addresses.FirstOrDefault(a => a.Id == id);

    }
}
/*
 query{
query {
 employees{
 edges {
      node {
        id
        name
        email
        addresses {
          street
          city
          state
          zip
        }
      }
 }
}
}
}
 */