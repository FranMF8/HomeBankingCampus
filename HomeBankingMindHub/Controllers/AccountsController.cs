using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HomeBankingMindHub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<AccountDTO> result = _accountService.GetAll();

                return Ok(result);
            }
            catch (Exception e)
            {

                return BadRequest("Error: " + e);
            }
            
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                AccountDTO result = _accountService.GetById(id);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e);
            }
        }
    }
}
