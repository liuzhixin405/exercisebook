using Microsoft.EntityFrameworkCore;

namespace EF.GraphQL.Demo
{
    public class CustomDbContext: DbContext
    {
        public CustomDbContext(DbContextOptions<CustomDbContext> options)
            : base(options)
        {
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Employee> Employees { get; set; }
        //override protected void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    modelBuilder.Entity<Address>().ToTable("Address");
        //    modelBuilder.Entity<Employee>().ToTable("Employee");

        //    modelBuilder.Entity<Employee>().Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        //    modelBuilder.Entity<Employee>().Property(e => e.Name).HasColumnName("name");
        //    modelBuilder.Entity<Employee>().Property(e => e.Email).HasColumnName("email");

        //    modelBuilder.Entity<Address>().Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        //    modelBuilder.Entity<Address>().Property(e => e.Street).HasColumnName("street");
        //    modelBuilder.Entity<Address>().Property(e => e.City).HasColumnName("city");
        //    modelBuilder.Entity<Address>().Property(e => e.State).HasColumnName("state");
        //    modelBuilder.Entity<Address>().Property(e => e.Zip).HasColumnName("zip");

        //    modelBuilder.Entity<Address>().HasOne<Employee>().WithMany().HasForeignKey("employeeid");
        //    // 配置外键和导航属性
        //    modelBuilder.Entity<Address>()
        //        .HasOne(a => a.Employee)
        //        .WithMany(e => e.Addresses)
        //        .HasForeignKey(a => a.Employeeid)
        //        .HasConstraintName("FK_Address_Employee");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee")
                .Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

            modelBuilder.Entity<Employee>().Property(e => e.Name).HasColumnName("name");
            modelBuilder.Entity<Employee>().Property(e => e.Email).HasColumnName("email");
            modelBuilder.Entity<Employee>().Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            modelBuilder.Entity<Employee>().Property(e => e.Phone).HasColumnName("phone");

            modelBuilder.Entity<Address>().ToTable("Address")
                .Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();

            modelBuilder.Entity<Address>().Property(a => a.Street).HasColumnName("street");
            modelBuilder.Entity<Address>().Property(a => a.City).HasColumnName("city");
            modelBuilder.Entity<Address>().Property(a => a.State).HasColumnName("state");
            modelBuilder.Entity<Address>().Property(a => a.Zip).HasColumnName("zip");

            // 正确的外键配置
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Addresses)
                .HasForeignKey(a => a.Employeeid) // 使用实体的 EmployeeId 属性
                .HasConstraintName("FK_Address_Employee");
        }

    }
}
