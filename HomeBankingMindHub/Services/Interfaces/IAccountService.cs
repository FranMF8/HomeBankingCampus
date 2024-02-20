using HomeBankingMindHub.DTOS;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IAccountService
    {
        public List<AccountDTO> GetAll();
    }
}
