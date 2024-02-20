using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Handlers.Implementations;
using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;

namespace HomeBankingMindHub.Services.Implementations
{
    public class ClientService : IClientService
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;
        private IEncryptionHandler _encryptionHandler;
        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository) 
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            _encryptionHandler = new EncryptionHandler();
        }

        public string CreateAccount(string email)
        {
            throw new NotImplementedException();
        }

        public string CreateCard(string email)
        {
            throw new NotImplementedException();
        }

        public List<ClientDTO> Get()
        {
            var clients = _clientRepository.GetAllClients();

            var clientsDTO = new List<ClientDTO>();

            foreach (Client client in clients)
            {
                ClientDTO clientDTO = new ClientDTO(client);
                clientsDTO.Add(clientDTO);
            }

            return clientsDTO;
        }

        public List<AccountDTO> GetAccounts()
        {
            throw new NotImplementedException();
        }

        public ClientDTO GetById(long id)
        {
            Client client = _clientRepository.FindById(id);

            if (client == null)
            {
                return null;
            }

            ClientDTO clientDTO = new ClientDTO(client);
            return clientDTO;
        }

        public List<CardDTO> GetCards()
        {
            throw new NotImplementedException();
        }

        public ClientDTO GetCurrent(string email)
        {
            throw new NotImplementedException();
        }

        public string PostClient(RegisterClientDTO client)
        {
            throw new NotImplementedException();
        }
    }
}
