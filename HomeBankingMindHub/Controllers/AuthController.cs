using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Handlers.Implementations;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IEncryptionHandler _encryptionHandler;
        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            _encryptionHandler = new EncryptionHandler();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginClientDTO client)
        {
            try
            {
                if ( (client.Email == null || client.Email == string.Empty) || (client.Password == null || client.Password == string.Empty))
                {
                    return StatusCode(401, "Campos vacios");
                }

                Client user = _clientRepository.FindByEmail(client.Email);

                if (user == null || !( _encryptionHandler.ValidatePassword(client.Password, user.Hash, user.Salt) ))
                    return StatusCode(401, "Credenciales invalidas");

                var claims = new List<Claim>
                {
                    new Claim("Client", user.Email),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

