using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories.Implementations
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {

        }
        public Transaction FindById(long id)
        {
            return FindByCondition( tr => tr.Id == id ).FirstOrDefault();
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return FindAll().ToList();
        }

        public IEnumerable<Transaction> GetTransactionsByAccount(long accountId)
        {
            return FindByCondition(tr => tr.AccountId == accountId).ToList();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}
