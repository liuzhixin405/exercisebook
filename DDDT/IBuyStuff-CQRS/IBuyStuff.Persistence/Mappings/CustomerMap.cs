
using IBuyStuff.Domain.Customers;
using IBuyStuff.Domain.Shared;
using IBuyStuff.Persistence.Facade;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.Persistence.Mappings
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Primary Key
            //builder.HasKey(t => t.CustomerId);
            //builder.Property(t => t.CustomerId)
            //    .IsRequired()
            //    .HasColumnName("Id");
            //builder.Property(t => t.FirstName)
            //    .IsRequired()
            //    .HasColumnName("FirstName");
            //builder.Property(t => t.LastName)
            //    .IsRequired()
            //    .HasColumnName("LastName");
            //builder.Property(t => t.Email)
            //    .IsRequired()
            //    .HasColumnName("Email");
            //builder.OwnsOne(o => o.Address, a =>
            //{
            //    a.Property<int>("OrderId").UseHiLo("orderseq", CommandModelDatabase.DEFAULT_SCHEMA);
            //    a.WithOwner();
            //});
            //builder.HasOne<CreditCard>().WithMany().HasForeignKey("_paymentId").IsRequired(false).OnDelete(DeleteBehavior.Restrict); 
            // Properties			
                //.HasMaxLength(30)
                //.HasColumnName("Address_Street");
            //builder.Property(t => t.Address.City)
            //    .HasMaxLength(15)
            //    .HasColumnName("Address_City");
            //builder.Property(t => t.Address.Number)
            //    .HasColumnName("Address_Number");
            //builder.Property(t => t.Address.Zip)
            //    .HasMaxLength(15)
            //    .HasColumnName("Address_Zip");
            //builder.Property(t => t.Payment.Number);
            ////.IsOptional();
            //builder.Property(t => t.Payment.Owner);
            ////.IsOptional();
            //builder.Property(t => t.Payment.Type);
            ////.IsOptional();
            //builder.Property(t => t.Payment.Expires.Month);
            ////.IsOptional();
            //builder.Property(t => t.Payment.Expires.Year);
            //.IsOptional();

            // Table and relationships 
            //builder.ToTable("Customers");
            //builder.HasMany(c => c.Orders);
        }

       
      
    }

}