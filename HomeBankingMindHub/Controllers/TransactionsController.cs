using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult MakeTransaction()
        {
            try
            {
                return StatusCode(201, "Transaccion realizada");
            }
            catch (Exception e)
            {
                StatusCode(500, e);
            }
        }
    }
}
