using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace csharp_sprint1_stories.Models;

public partial class CsharpSprint1StoriesContext : DbContext
{
    public CsharpSprint1StoriesContext(DbContextOptions<CsharpSprint1StoriesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<CheckingAccount> CheckingAccounts { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<SavingsAccount> SavingsAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.HasIndex(e => e.CustomerId, "FK_Accounts_Customers");

            entity.Property(e => e.AccountType).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Accounts_Customers");
        });

        modelBuilder.Entity<CheckingAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.Property(e => e.NextCheckNumber).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.Account).WithOne(p => p.CheckingAccount)
                .HasForeignKey<CheckingAccount>(d => d.AccountId)
                .HasConstraintName("FK_CheckingAccounts_Accounts");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.HasIndex(e => e.Abn, "UQ_Companies_ABN").IsUnique();

            entity.HasIndex(e => e.Acn, "UQ_Companies_ACN").IsUnique();

            entity.Property(e => e.Abn)
                .HasMaxLength(20)
                .HasColumnName("ABN");
            entity.Property(e => e.Acn)
                .HasMaxLength(20)
                .HasColumnName("ACN");
            entity.Property(e => e.ContactPersonEmail).HasMaxLength(100);
            entity.Property(e => e.ContactPersonName).HasMaxLength(100);
            entity.Property(e => e.ContactPersonPhone).HasMaxLength(30);
            entity.Property(e => e.Industry).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithOne(p => p.Company)
                .HasForeignKey<Company>(d => d.CustomerId)
                .HasConstraintName("FK_Companies_Customers");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.HasIndex(e => e.Email, "UQ_Customers_Email").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerType).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.Occupation).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithOne(p => p.Person)
                .HasForeignKey<Person>(d => d.CustomerId)
                .HasConstraintName("FK_Persons_Customers");
        });

        modelBuilder.Entity<SavingsAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.Property(e => e.InterestRate).HasPrecision(5);

            entity.HasOne(d => d.Account).WithOne(p => p.SavingsAccount)
                .HasForeignKey<SavingsAccount>(d => d.AccountId)
                .HasConstraintName("FK_SavingsAccounts_Accounts");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
