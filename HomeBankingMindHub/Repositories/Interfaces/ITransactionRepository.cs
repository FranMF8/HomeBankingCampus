using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        void Save(Transaction card);
        Transaction FindById(long id);
        Transaction FindByNumber(string number);
        IEnumerable<Transaction> GetTransactionsByAccount(long accountVIN);
    }
}
