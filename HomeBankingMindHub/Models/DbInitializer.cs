using System;
using System.Linq;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123456"}
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }
            if(!context.Accounts.Any()) 
            {
                var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");

                if (accountVictor != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreatedDate = DateTime.Now, Number = "V0001", Balance = 0 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();
                }
            }
                

            if (!context.Transactions.Any())
            {
                var transactionsVictor = context.Accounts.FirstOrDefault(a => a.Number == "V0001");

                if (transactionsVictor != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= transactionsVictor.Id, Amount = 10000, DateTime = DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type = TransactionType.CREDIT.ToString() },

                        new Transaction { AccountId= transactionsVictor.Id, Amount = -2000, DateTime = DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT.ToString() },

                        new Transaction { AccountId= transactionsVictor.Id, Amount = -3000, DateTime = DateTime.Now.AddHours(-7), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT.ToString() },
                    };

                    double newBalance = 0;

                    foreach (Transaction transaction in transactions)
                    {
                        newBalance += transaction.Amount;
                        context.Transactions.Add(transaction);
                    }

                    var account = context.Accounts.FirstOrDefault(acc => acc.Number == "V0001");

                    account.Balance = newBalance;

                    context.SaveChanges();

                }
            }
            
        }
    }
}