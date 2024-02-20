using HomeBankingMindHub.DTOS;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface ITransactionService
    {
        public string MakeTransaction(MakeTransactionDTO transaction, string email);
    }
}
