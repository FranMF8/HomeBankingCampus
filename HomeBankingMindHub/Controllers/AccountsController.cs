using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HomeBankingMindHub.Controllers
{
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
                var account = _accountRepository.FindById(id);

                if (account == null)
                {
                    return NotFound();
                }

                var accountDTO = new AccountDTO()
                {
                    Id = account.Id,
                    Balance = account.Balance,
                    CreationDate = account.CreatedDate,
                    Number = account.Number,

                    Transactions = account.Transactions.Select(tr => new TransactionDTO
                    {
                        Id = tr.Id,
                        Type = tr.Type.ToString(),
                        Description = tr.Description,
                        Date = tr.DateTime,
                        Amount = tr.Amount
                    }).ToList()
                };

                return Ok(accountDTO);
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e);
            }
        }
    }
}
