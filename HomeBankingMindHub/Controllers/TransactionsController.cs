using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        TransactionsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost]
        public IActionResult MakeTransaction(MakeTransactionDTO transaction)
        {
            try
            {
                if (transaction.Amount == 0 || transaction.Description.IsNullOrEmpty() || transaction.FromAccountNumber.IsNullOrEmpty() || transaction.ToAccountNumber.IsNullOrEmpty())
                    return StatusCode(403, "Campos vacios");

                if (transaction.FromAccountNumber == transaction.ToAccountNumber)
                    return StatusCode(403, "Las cuentas de origen y destino son identicas");


                return StatusCode(201, "Transaccion realizada");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
