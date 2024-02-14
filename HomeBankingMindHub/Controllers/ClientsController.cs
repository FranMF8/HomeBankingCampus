using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Handlers.Implementations;
using HomeBankingMindHub.Repositories.Classes;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private IEncryptionHandler _encryptionHandler;

        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _encryptionHandler = new EncryptionHandler();
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();

                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients) { 

                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreatedDate,
                            Number = ac.Number

                        }).ToList(),
                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),
                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color.ToString(),
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type.ToString()
                        }).ToList()
                    };

                    clientsDTO.Add(newClientDTO);
                }

                return Ok(clientsDTO);
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
                var client = _clientRepository.FindById(id);

                if (client == null)
                {
                    return NotFound();
                }

                var clientDTO = new ClientDTO

                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreatedDate,
                        Number = ac.Number

                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()

                };

                return Ok(clientDTO);
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
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreatedDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()
                };

                return Ok(clientDTO);
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

                //buscamos si ya existe el usuario
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

        }
    }
}
