using HomeBankingMindHub.DTOS;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IClientService
    {
        public List<ClientDTO> Get();
        public ClientDTO GetById(long id);
        public ClientDTO GetCurrent(string email);
        public string PostClient(RegisterClientDTO client);
        public string CreateAccount(string email);
        public string CreateCard(string email, CreateCardDTO card);
        public List<AccountDTO> GetAccounts();
        public List<CardDTO> GetCards();
    }
}
