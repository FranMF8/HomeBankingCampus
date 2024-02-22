using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Handlers.Implementations;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IEncryptionHandler _encryptionHandler;
        private IConfiguration config;
        public AuthController(IClientRepository clientRepository, IConfiguration config)
        {
            _clientRepository = clientRepository;
            _encryptionHandler = new EncryptionHandler();
            this.config = config;
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

        [HttpPost("login/mobile")]
        public async Task<ActionResult> LoginJWT([FromBody] LoginClientDTO client)
        {
            try
            {
                Console.WriteLine("Mobile cosa");
                var dbClient = _clientRepository.FindByEmail(client.Email);

                if (dbClient == null)
                    return StatusCode(404, "Email invalido");

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, dbClient.FirstName),
                    new Claim(ClaimTypes.Email , client.Email),
                    new Claim("Client", client.Email)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var securityToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                    );

                string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

                return Ok( new { token = token});
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
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

