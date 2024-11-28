using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EF.GraphQL.Demo
{

        public class Mutation
        {
            // 创建员工并添加地址
            public async Task<Employee> AddEmployee(EmployeeInput input, [Service] CustomDbContext context)
            {
                var employee = new Employee
                {
                    Name = input.Name,
                    Email = input.Email,
                    DateOfBirth = input.DateOfBirth,
                    Phone = input.Phone
                };

                // 添加员工并保存
                context.Employees.Add(employee);
                await context.SaveChangesAsync();

                // 创建地址
                var address = new Address
                {
                    Street = input.Street,
                    City = input.City,
                    State = input.State,
                    Zip = input.Zip,
                    Employeeid = employee.Id
                };

                // 添加地址并保存
                context.Addresses.Add(address);
                await context.SaveChangesAsync();

                return employee;
            }

            // 更新员工信息
            public async Task<Employee> UpdateEmployee(int id, UpdateEmployeeInput input, [Service] CustomDbContext context)
            {
            var employee = await context.Employees
                           .FirstOrDefaultAsync(e => e.Id == input.Id);

            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            // 更新 employee 属性
            employee.Name = input.Name;
            employee.Email = input.Email;
            employee.DateOfBirth = input.DateOfBirth;
            // 如果需要更新电话字段
            if (!string.IsNullOrEmpty(input.Phone))
            {
                employee.Phone = input.Phone; // 假设你在 Employee 实体中有一个 Phone 属性
            }
            // 更新地址（如果需要）
            var address = await context.Addresses
                                        .FirstOrDefaultAsync(a => a.Employeeid == input.Id);

            if (address != null)
            {
                address.City = input.City;
                address.State = input.State;
                address.Street = input.Street;
                address.Zip = input.Zip;
            }
            // 保存更新后的数据
            await context.SaveChangesAsync();

            return employee;  // 确保返回有效的 employee 对象
        }

            // 删除员工及其所有地址
            public async Task<bool> DeleteEmployee(int id, [Service] CustomDbContext context)
            {
                var employee = await context.Employees.Include(e => e.Addresses).FirstOrDefaultAsync(e => e.Id == id);
                if (employee == null) return false;

                context.Employees.Remove(employee);
                context.Addresses.RemoveRange(employee.Addresses);

                await context.SaveChangesAsync();
                return true;
            }

            // 删除指定地址
            public async Task<bool> DeleteAddress(int id, [Service] CustomDbContext context)
            {
                var address = await context.Addresses.FirstOrDefaultAsync(a => a.Id == id);
                if (address == null) return false;

                context.Addresses.Remove(address);
                await context.SaveChangesAsync();

                return true;
            }
        }

        public record EmployeeInput(string Name, string Email, DateTime DateOfBirth, string Phone, string Street, string City, string State, string Zip);
    public record UpdateEmployeeInput(
    int Id,          // 员工ID，唯一标识员工
    string Name,     // 员工的名字
    string Email,    // 员工的电子邮件
    DateTime DateOfBirth, // 员工的出生日期
    string City,     // 员工的城市
    string State,    // 员工的州或省
    string Street,   // 员工的街道地址
    string Zip,       // 员工的邮政编码
    string Phone     // 员工的电话号码
);

}
/*
# 创建员工并添加地址
mutation {
  addEmployee(input: {
    name: "John Doe",
    email: "johndoe@example.com",
     dateOfBirth: "1990-01-01T00:00:00Z",  # 确保是 ISO 8601 格式
    phone: "123-456-7890",
    street: "123 Main St",
    city: "New York",
    state: "NY",
    zip: "10001"
  }) {
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


# 更新员工信息
mutation {
  updateEmployee(id: 5, input: {
    id:5,
    name: "John Doe Updated",
    email: "johnupdated@example.com",
    dateOfBirth: "1990-01-01T00:00:00Z",
    phone: "987-654-3210",
    street: "123 Updated St",
    city: "Los Angeles",
    state: "CA",
    zip: "90001"
  }) {
    id
    name
    email
    addresses {
      street
      city
    }
  }
}
 */