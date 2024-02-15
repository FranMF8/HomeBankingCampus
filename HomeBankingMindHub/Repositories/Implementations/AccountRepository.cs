using HomeBankingMindHub.Handlers.Implementations;
using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories.Classes
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {

        }

        public Account FindById(long id)
        {
            return FindByCondition( acc => acc.Id == id)
                .Include(acc => acc.Transactions)
                .FirstOrDefault();
        }
        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(acc => acc.Transactions)
                .ToList();
        }
        public void Save(Account account)
        {
            bool condition = true;
            string vin = string.Empty;

            while (condition)
            {
                vin = NumbersHandler.GenerateVIN();

                var acc = FindByVIN(vin);

                if (acc == null)
                    condition = false;
            }
            account.Number = vin;
            Create(account);
            SaveChanges();
        }
        public IEnumerable<Account> GetAccountsByClient(long clientId)

        {
            return FindByCondition(account => account.ClientId == clientId)
            .Include(account => account.Transactions)
            .ToList();
        }
        public Account FindByVIN(string VIN)
        {
            return FindByCondition(acc => acc.Number == VIN)
                .Include(acc => acc.Transactions)
                .FirstOrDefault();
        }
    }
}
