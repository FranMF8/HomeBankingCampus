using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Implementations;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private IClientRepository _clientRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;
        public LoanService(
            ILoanRepository loanRepository,
            IClientLoanRepository clientLoanRepository,
            IClientRepository clientRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository
            )
        {
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public string ApplicateForLoan(LoanApplicationDTO loan, string email)
        {
            var client = _clientRepository.FindByEmail(email);

            if (client == null)
                return "Cliente no encontrado";

            var dbLoan = _loanRepository.FindById(loan.LoanId);

            if (dbLoan == null)
                return "Credito invalido";

            var paymentsCondition = dbLoan.Payments.Split(",").Contains(loan.Payments);

            if (loan.LoanId == 0 || loan.Amount <= 0 || loan.Payments.IsNullOrEmpty() || int.Parse(loan.Payments) <= 0 || loan.ToAccountNumber.IsNullOrEmpty() || !paymentsCondition)
                return "Datos invalidos";

            if (dbLoan.MaxAmount < loan.Amount)
                return "Limite alcanzado";

            var account = _accountRepository.FindByVIN(loan.ToAccountNumber);

            if (account == null)
                return "Error en la cuenta";

            if (account.ClientId != client.Id)
                return "La cuenta no pertenece al cliente autenticado";

            ClientLoan clientLoan = new ClientLoan
            {
                Amount = loan.Amount * 1.2,
                Payments = loan.Payments,
                ClientId = client.Id,
                LoanId = loan.LoanId
            };

            Transaction transaction = new Transaction
            {
                Type = TransactionType.CREDIT,
                Amount = loan.Amount,
                Description = "Credit",
                DateTime = DateTime.Now,
                AccountId = account.Id
            };

            account.Balance += loan.Amount;

            _clientLoanRepository.Save(clientLoan);
            _transactionRepository.Save(transaction);
            _accountRepository.Save(account);

            return "ok";
        }

        public List<LoanDTO> GetLoans(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
                return null;

            List<Loan> loans = _loanRepository.GetAll().ToList();

            List<LoanDTO> result = new List<LoanDTO>();

            foreach (var loan in loans)
            {
                LoanDTO loanDTO = new LoanDTO(loan);
                result.Add(loanDTO);
            }

            return result;
        }
    }
}
