using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;
        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public IActionResult MakeTransaction(MakeTransactionDTO transaction)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return StatusCode(403, "Error en la autenticacion");

                var client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return NotFound();

                if (transaction.Amount <= 0 || transaction.Description.IsNullOrEmpty() || transaction.FromAccountNumber.IsNullOrEmpty() || transaction.ToAccountNumber.IsNullOrEmpty())
                    return StatusCode(403, "Campos invalidos");

                if (transaction.FromAccountNumber == transaction.ToAccountNumber)
                    return StatusCode(403, "Las cuentas de origen y destino son identicas");

                var originAcc = _accountRepository.FindByVIN(transaction.FromAccountNumber);

                if (originAcc == null)
                    return StatusCode(403, "La cuenta de origen no existe");

                if ( !(client.Id == originAcc.ClientId) )
                    return StatusCode(403, "La cuenta no pertenece al cliente autenticado");

                var destinationAcc = _accountRepository.FindByVIN(transaction.ToAccountNumber);

                if (destinationAcc == null)
                    return StatusCode(403, "La cuenta de destino no existe");

                if (originAcc.Balance < transaction.Amount)
                    return StatusCode(403, "Fondos insuficientes");

                Transaction debitTransaction = new Transaction
                {
                    AccountId = originAcc.Id,
                    Type = TransactionType.DEBIT,
                    Amount = transaction.Amount * -1,
                    Description = transaction.Description + " " + destinationAcc.Number,
                    DateTime = DateTime.Now
                };

                Transaction creditTransaction = new Transaction
                {
                    AccountId = destinationAcc.Id,
                    Type = TransactionType.CREDIT,
                    Amount = transaction.Amount,
                    Description = transaction.Description + " " + originAcc.Number,
                    DateTime = DateTime.Now
                };

                originAcc.Balance -= transaction.Amount;
                destinationAcc.Balance += transaction.Amount;

                _transactionRepository.Save(debitTransaction);
                _transactionRepository.Save(creditTransaction);

                _accountRepository.Save(originAcc);
                _accountRepository.Save(destinationAcc);

                return StatusCode(201, "Transferencia realizada");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
