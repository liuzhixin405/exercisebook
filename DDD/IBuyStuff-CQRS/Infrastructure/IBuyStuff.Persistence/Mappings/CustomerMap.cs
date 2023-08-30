
using System.Reflection.Emit;
using IBuyStuff.Domain.Customers;
using IBuyStuff.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.Persistence.Mappings
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {

            // Primary Key
            builder.HasKey(t => t.CustomerId);
            builder.Property(t => t.CustomerId)
                .IsRequired()
                .HasColumnName("Id");
            builder.Property(t => t.FirstName)
                .IsRequired()
                .HasColumnName("FirstName");
            builder.Property(t => t.LastName)
                .IsRequired()
                .HasColumnName("LastName");
            builder.Property(t => t.Email)
                .IsRequired()
                .HasColumnName("Email");

            // Properties			
            builder.OwnsOne(t => t.Address, a =>
            {
                a.Property(p => p.Street).HasMaxLength(30)
                .HasColumnName("Address_Street");
                a.Property(t => t.City)
              .HasMaxLength(15)
              .HasColumnName("Address_City");
                a.Property(t => t.Number)
                    .HasColumnName("Address_Number");
                a.Property(t => t.Zip)
                    .HasMaxLength(15)
                    .HasColumnName("Address_Zip");
            });

            builder.OwnsOne(t => t.Payment, a => {
                a.Property(t => t.Number)
                  .HasColumnName("Payment_Number").IsRequired(false);
                a.Property(t => t.Owner)
                    .HasColumnName("Payment_Owner").IsRequired(false);
               
                a.Property(t => t.Type)
                   .HasColumnName("Payment_Type");
               
                a.OwnsOne(b => b.Expires, c =>
                {
                    c.Property(d => d.Year).HasColumnName("Payment_Expires_Year");
                    c.Property(d => d.Month).HasColumnName("Payment_Expires_Month");
                });

                //a.Property(t => t.Expires.Month)
                //    .IsRequired();
                //a.Property(t => t.Expires.Year)
                //    .IsRequired();
                //a.Property(t => t.Expires).IsRequired();
            });
           

            // Table and relationships 
            builder.ToTable("Customers");
            builder.HasMany(c => c.Orders);
        }
    }
   


}