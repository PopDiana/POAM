using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace POAM.Models
{
    public partial class POAMDbContext : DbContext
    {
        public POAMDbContext()
        {
        }

        public POAMDbContext(DbContextOptions<POAMDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apartment> Apartment { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<Receipt> Receipt { get; set; }
        public virtual DbSet<WaterConsumption> WaterConsumption { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-3A7DF0HD\\SQLEXPRESS;Database=POAMDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.HasKey(e => e.IdApartment);

                entity.HasIndex(e => e.IdOwner);

                entity.Property(e => e.Building)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FlatNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdOwnerNavigation)
                    .WithMany(p => p.Apartment)
                    .HasForeignKey(d => d.IdOwner)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Apartment_Owner");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(e => e.IdContract);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.IdEmployee);

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Employment)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Pid)
                    .IsRequired()
                    .HasColumnName("PID")
                    .HasMaxLength(50);

                entity.Property(e => e.Telephone)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(e => e.IdOwner);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Telephone)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username).IsRequired();
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.HasKey(e => e.IdReceipt);

                entity.HasIndex(e => e.IdApartment);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.IdApartmentNavigation)
                    .WithMany(p => p.Receipt)
                    .HasForeignKey(d => d.IdApartment)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Receipt_Apartment");
            });

            modelBuilder.Entity<WaterConsumption>(entity =>
            {
                entity.HasKey(e => e.IdWaterConsumption);

                entity.HasIndex(e => e.IdApartment);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.IdApartmentNavigation)
                    .WithMany(p => p.WaterConsumption)
                    .HasForeignKey(d => d.IdApartment)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WaterConsumption_Apartment");
            });
        }
    }
}
