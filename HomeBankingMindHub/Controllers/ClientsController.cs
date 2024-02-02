using HomeBankingMindHub.Models.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
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

                    }).ToList()

                };

                return Ok(clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
