using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JobRecruitment.Entities
{
    public partial class JobRecruitmentContext : DbContext
    {
        public JobRecruitmentContext()
        {
        }

        public JobRecruitmentContext(DbContextOptions<JobRecruitmentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Companys> Companys { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<Requirements> Requirements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-7JOLG2I;database=JobRecruitment;uid=sa;pwd=118Root");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cities>(entity =>
            {
                entity.Property(e => e.City).HasMaxLength(50);
            });

            modelBuilder.Entity<Companys>(entity =>
            {
                entity.Property(e => e.CompanyAddress).HasMaxLength(100);

                entity.Property(e => e.CompanyIntroduce).HasMaxLength(500);

                entity.Property(e => e.CompanyName).HasMaxLength(50);

                entity.Property(e => e.CompanyNature).HasMaxLength(50);

                entity.Property(e => e.CompanySize).HasMaxLength(50);

                entity.Property(e => e.IndustryType).HasMaxLength(50);
            });

            modelBuilder.Entity<Jobs>(entity =>
            {
                entity.Property(e => e.Education).HasMaxLength(50);

                entity.Property(e => e.JobName).HasMaxLength(50);

                entity.Property(e => e.JobPay).HasMaxLength(50);

                entity.Property(e => e.PublishTime).HasColumnType("datetime");

                entity.Property(e => e.Welfare).HasMaxLength(500);

                entity.Property(e => e.WorkArea)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.WorkExperience).HasMaxLength(50);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Jobs_Companys");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.WorkPlace)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Jobs_Jobs1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
