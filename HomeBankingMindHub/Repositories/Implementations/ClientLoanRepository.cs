using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Repositories.Implementations
{
    public class ClientLoanRepository : RepositoryBase<ClientLoan>, IClientLoanRepository
    {
        public ClientLoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public void Save(ClientLoan loan)
        {
            Create(loan);
            SaveChanges();
        }
    }
}
