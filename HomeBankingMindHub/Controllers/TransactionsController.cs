using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private ITransactionService _transactionService;
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public IActionResult MakeTransaction(MakeTransactionDTO transaction)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return StatusCode(403, "Sesion invalida");

                string message = _transactionService.MakeTransaction(transaction, email);

                if (message != "ok")
                    return StatusCode(403, message);

                return StatusCode(201, "Transferencia realizada");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
