using HomeBankingMindHub.DTOS;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface ILoanService
    {
        public string ApplicateForLoan(LoanApplicationDTO loan, string email);
        public List<ClientLoanDTO> GetLoans();
    }
}
