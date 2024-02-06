﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
namespace HomeBankingMindHub.Models
{
    public class HomeBankingContext : DbContext
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<Loan>().ToTable("Loan");
            modelBuilder.Entity<ClientLoan>().ToTable("ClientLoan");
            modelBuilder.Entity<Card>().ToTable("Card");
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<ClientLoan> ClientLoans { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}
