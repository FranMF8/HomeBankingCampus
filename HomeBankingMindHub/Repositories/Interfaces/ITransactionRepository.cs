using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        void Save(Transaction card);
        Transaction FindById(long id);
    }
}
