﻿using System.ComponentModel.DataAnnotations;

namespace HomeBankingMindHub.Models
{
    public class Client
    {
        [Key] 
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Salt { get; set; }
        public bool Verified { get; set; }

        public ICollection<Account> Accounts { get; set; }
        public ICollection<ClientLoan> ClientLoans { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
