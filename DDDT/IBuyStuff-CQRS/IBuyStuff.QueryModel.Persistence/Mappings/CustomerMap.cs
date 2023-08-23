
using IBuyStuff.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.QueryModel.Persistence.Mappings
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

            // Properties			
            //builder.Property(t => t.Address.Street)
            //    .HasMaxLength(30)
            //    .HasColumnName("Address_Street");
            //builder.Property(t => t.Address.City)
            //    .HasMaxLength(15)
            //    .HasColumnName("Address_City");
            //builder.Property(t => t.Address.Number)
            //    .HasColumnName("Address_Number");
            //builder.Property(t => t.Address.Zip)
            //    .HasMaxLength(15)
            //    .HasColumnName("Address_Zip");
            //builder.Property(t => t.Payment.Number)
            //    ;
            //builder.Property(t => t.Payment.Owner)
            //    ;
            //builder.Property(t => t.Payment.Type)
            //    ;
            //builder.Property(t => t.Payment.Expires.Month)
            //    ;
            //builder.Property(t => t.Payment.Expires.Year)
            //    ;

            // Table and relationships 
            //builder.ToTable("Customers");
            //builder.HasMany(c => c.Orders);
        }
    }

}