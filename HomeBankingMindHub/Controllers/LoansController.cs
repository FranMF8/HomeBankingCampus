using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public IActionResult ApplicateForLoan(LoanApplicationDTO loan)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return StatusCode(403, "Error en la autenticacion");

                var client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return NotFound();

                if (loan.LoanId == 0 || loan.Amount <= 0 || loan.Payments.IsNullOrEmpty() || int.Parse(loan.Payments) <= 0 || loan.ToAccountNumber.IsNullOrEmpty())
                    return StatusCode(403, "Datos invalidos");

                var dbLoan = _loanRepository.FindById(loan.LoanId);

                if(dbLoan == null)
                    return StatusCode(403, "Datos invalidos");

                if (dbLoan.MaxAmount < loan.Amount)
                    return StatusCode(403, "Limite alcanzado");

                var account = _accountRepository.FindByVIN(loan.ToAccountNumber);

                if (account == null)
                    return StatusCode(403, "Error en la cuenta");

                if (account.ClientId != client.Id)
                    return StatusCode(403, "La cuenta no pertenece al cliente autenticado");

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


                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet]
        public IActionResult GetLoans()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return StatusCode(403, "Error en la autenticacion");

                var client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return NotFound();

                var loans = _loanRepository.GetAll();

                List<LoanDTO> result = new List<LoanDTO>();

                foreach (var loan in loans)
                {
                    LoanDTO loanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments
                    };
                    result.Add(loanDTO);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
