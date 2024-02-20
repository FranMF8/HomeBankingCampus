using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Handlers.Implementations;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Services.Interfaces;
using HomeBankingMindHub.Services.Implementations;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        public ClientsController(ClientService clientService) 
        {
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<ClientDTO> result = _clientService.Get();

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                ClientDTO result = _clientService.GetById(id);

                if (result == null)
                    return StatusCode(404, "Id invalido");

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //[HttpPost]
        /*public IActionResult Add([FromBody] CreateClientDTO model)
        {
            if (model.Email.IsNullOrEmpty() || model.FirstName.IsNullOrEmpty() || model.LastName.IsNullOrEmpty())
            {
                return BadRequest("Se requieren todos los campos");
            }

            try
            {
                var client = new Client();

                client.Email = model.Email;
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.Password = "123456";
                _clientRepository.Save(client);

                return Created();
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e);
            }
        }*/

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return StatusCode(403, "Sesion invalida");

                ClientDTO result = _clientService.GetCurrent(email);

                if (result == null)
                    return StatusCode(404, "Cliente invalido");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] RegisterClientDTO client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");

                Client user = _clientRepository.FindByEmail(client.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
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
                    return StatusCode(400, "Error al crear la cuenta");

                Account account = new Account
                {
                    ClientId = dbUser.Id,
                    CreatedDate = DateTime.Now,
                    Balance = 0
                };

                _accountRepository.Save(account);

                return StatusCode(201, "Cuenta creada con exito");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        public IActionResult Post()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return Forbid();

                var client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return NotFound();

                if (client.Accounts.Count() > 3)
                    return StatusCode(401, "Limite de cuentas alcanzado");

                Account account = new Account
                {
                    ClientId = client.Id,
                    CreatedDate = DateTime.Now,
                    Balance = 0
                };

                _accountRepository.Save(account);

                return StatusCode(201, "Cuenta creada con exito");
            }
            catch (Exception e)
            {

                return StatusCode(500, e);
            }          
        }

        [HttpPost("current/cards")]
        public IActionResult PostCard(CreateCardDTO card)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return Forbid();

                var client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return NotFound();

                if (client.Cards.Count() >= 6)
                    return Forbid();

                foreach (var crd in client.Cards)
                {
                    if (card.Type == crd.Type.ToString())
                    {
                        if (card.Color == crd.Color.ToString())
                        {
                            return Forbid();
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

                return StatusCode(201, "Tarjeta creada con exito");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet("current/accounts")]  
        public IActionResult GetAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return StatusCode(403, "Error en la autenticacion");

                var client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return NotFound();

                List<AccountDTO> result = new List<AccountDTO>();

                foreach (var account in client.Accounts)
                {
                    AccountDTO acc = new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        CreationDate = account.CreatedDate,
                        Balance = account.Balance
                    };
                    result.Add(acc);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet("current/cards")]
        public IActionResult GetCards()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return Forbid();

                var client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return NotFound();

                var cards = _cardRepository.GetCardsByClient(client.Id);

                if (cards == null)
                    return NotFound();

                return Ok(cards);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
