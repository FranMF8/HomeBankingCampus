﻿using HomeBankingMindHub.Models;
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
        public async Task<ActionResult> LoginJWT([FromBody] LoginClientDTO client)
        {
            try
            {
                Client user = _clientRepository.FindByEmail(client.Email);

                if (user == null || !(_encryptionHandler.ValidatePassword(client.Password, user.Hash, user.Salt)))
                    return StatusCode(401, "Credenciales invalidas");

                var claims = new[]
                {
                    new Claim("Client", client.Email),
                    new Claim("AuthType", "JWT")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var securityToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                    );

                string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

                return Ok( new { token = token });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}

