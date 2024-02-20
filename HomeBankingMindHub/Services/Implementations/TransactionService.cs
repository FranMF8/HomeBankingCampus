using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Implementations;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionService(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        public string MakeTransaction(MakeTransactionDTO transaction, string email)
        {
            var client = _clientRepository.FindByEmail(email);

            if (client == null)
                return "Email invalido";

            if (transaction.Amount <= 0 || transaction.Description.IsNullOrEmpty() || transaction.FromAccountNumber.IsNullOrEmpty() || transaction.ToAccountNumber.IsNullOrEmpty())
                return "Campos invalidos";

            if (transaction.FromAccountNumber == transaction.ToAccountNumber)
                return "Las cuentas de origen y destino son identicas";

            var originAcc = _accountRepository.FindByVIN(transaction.FromAccountNumber);

            if (originAcc == null)
                return "La cuenta de origen no existe";

            if (!(client.Id == originAcc.ClientId))
                return "La cuenta no pertenece al cliente autenticado";

            var destinationAcc = _accountRepository.FindByVIN(transaction.ToAccountNumber);

            if (destinationAcc == null)
                return "La cuenta de destino no existe";

            if (originAcc.Balance < transaction.Amount)
                return "Fondos insuficientes";

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

            return "ok";
        }
    }
}
