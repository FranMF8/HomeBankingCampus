using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();

                var accountsDTO = new List<AccountDTO>();

                foreach (var account in accounts)
                {
                    var newAccountDTO = new AccountDTO()
                    {
                        Id = account.Id,
                        Number = account.Number,
                        CreationDate = account.CreatedDate,
                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(tr => new TransactionDTO
                        {
                            Id = tr.Id,
                            Description = tr.Description,
                            Date = tr.DateTime,
                            Amount = tr.Amount
                        }).ToList()
                    };

                    accountsDTO.Add(newAccountDTO);
                }

                return Ok(accountsDTO);
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
