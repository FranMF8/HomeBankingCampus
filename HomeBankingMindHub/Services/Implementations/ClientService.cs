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
            var client = _clientRepository.FindByEmail(email);

            if (client == null)
                return "Email invalido";

            if (client.Accounts.Count() > 3)
                return "Limite de cuentas alcanzado";

            Account account = new Account
            {
                ClientId = client.Id,
                CreatedDate = DateTime.Now,
                Balance = 0
            };

            _accountRepository.Save(account);

            return "ok";
        }

        public string CreateCard(string email, CreateCardDTO card)
        {
            var client = _clientRepository.FindByEmail(email);

            if (client == null)
                return "Email invalido";

            if (client.Cards.Count() >= 6)
                return "Limite de tarjetas alcanzado";

            foreach (var crd in client.Cards)
            {
                if (card.Type == crd.Type.ToString())
                {
                    if (card.Color == crd.Color.ToString())
                    {
                        return "Error al crear la tarjeta";
                    }
                }
            }

            Card newCard = new Card
            {
                ClientId = client.Id,
                CardHolder = client.FirstName + " " + client.LastName,
                Type = (CardType)Enum.Parse(typeof(CardType), card.Type, true),
                Color = (CardColor)Enum.Parse(typeof(CardColor), card.Color, true),
                FromDate = DateTime.Now,
                ThruDate = (card.Type == CardType.DEBIT.ToString() ? DateTime.Now.AddYears(5) : DateTime.Now.AddYears(4)),
            };

            _cardRepository.Save(newCard);

            return "ok";
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

        public List<AccountDTO> GetAccounts(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
                return null;

            List<AccountDTO> result = new List<AccountDTO>();

            foreach (var account in client.Accounts)
            {
                AccountDTO acc = new AccountDTO(account);
                result.Add(acc);
            }
            return result;
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

        public List<CardDTO> GetCards(string email)
        {
            throw new NotImplementedException();
        }

        public ClientDTO GetCurrent(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
                return null;

            ClientDTO clientDTO = new ClientDTO(client);

            return clientDTO;         
        }

        public string PostClient(RegisterClientDTO client)
        {
            Client user = _clientRepository.FindByEmail(client.Email);

            if (user != null)
            {
                return "Email en uso";
            }

            _encryptionHandler.EncryptPassword(client.Password, out byte[] hash, out byte[] salt);

            Client newClient = new Client
            {
                Email = client.Email,
                Hash = hash,
                Salt = salt,
                FirstName = client.FirstName,
                LastName = client.LastName,
            };

            _clientRepository.Save(newClient);

            var dbUser = _clientRepository.FindByEmail(newClient.Email);

            if (dbUser == null)
                return "Error al crear la cuenta";

            Account account = new Account
            {
                ClientId = dbUser.Id,
                CreatedDate = DateTime.Now,
                Balance = 0
            };

            _accountRepository.Save(account);

            return "ok";
        }
    }
}
