using HomeBankingMindHub.Models;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.DTOS
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDate { get; set; }
        public double Balance { get; set; }
        public ICollection<TransactionDTO> Transactions { get; set; }

        public AccountDTO(Account account)
        {
            Id = account.Id;
            Number = account.Number;
            CreationDate = account.CreatedDate;
            Balance = account.Balance;

            List<TransactionDTO> transactions = new List<TransactionDTO>();

            if (!account.Transactions.IsNullOrEmpty())
            {
                foreach (var transaction in account.Transactions)
                {
                    TransactionDTO transactionDTO = new TransactionDTO(transaction);
                    transactions.Add(transactionDTO);
                }         
            }
            Transactions = transactions;
        }

        public AccountDTO()
        {

        }
    }
}
