using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAllCards();
        void Save(Loan loan);
        Loan FindById(long id);
        Loan FindByNumber(string number);
        IEnumerable<Loan> GetLoansByClient(long clientId);
    }
}
