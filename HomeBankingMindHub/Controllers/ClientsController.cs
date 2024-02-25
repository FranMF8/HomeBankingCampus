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
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace HomeBankingMindHub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IEmailService _emailService;
        public ClientsController(IClientService clientService, IEmailService emailService) 
        {
            _clientService = clientService;
            _emailService = emailService;
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
        public async Task<IActionResult> PostAsync([FromBody] RegisterClientDTO client)
        {
            try
            {
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");

                bool emailIsValid = _emailService.ValidateEmail(client.Email);

                if (!emailIsValid)
                    return StatusCode(403, "Email invalido");

                bool emailSent = await _emailService.SendEmail(client.Email);

                if (!emailSent)
                    return StatusCode(403, "Error al crear cuenta");

                string message = _clientService.PostClient(client);

                if (message != "ok")
                    return StatusCode(403, message);

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
                    return StatusCode(403, "Sesion invalida");

                string message = _clientService.CreateAccount(email);

                if (message != "ok")
                    return StatusCode(403, message);

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
                    return StatusCode(403, "Sesion invalida");

                string message = _clientService.CreateCard(email, card);

                if (message != "ok")
                    return StatusCode(403, message);

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
                    return StatusCode(403, "Sesion invalida");

                List<AccountDTO> result = _clientService.GetAccounts(email);

                if (result == null)
                    return StatusCode(404, "Email invalido");

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
                    return StatusCode(403, "Sesion invalida");

                List<CardDTO> result = _clientService.GetCards(email);

                if (result == null)
                    return StatusCode(404, "Error al solicitar las tarjetas");

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet("verify/{email}")]
        public IActionResult VerifyAccount(string email)
        {
            try
            {
                string message = _clientService.VerifyUser(email);

                if(message != "ok")
                    return StatusCode(403, message);

                return Ok("Cuenta verificada");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
