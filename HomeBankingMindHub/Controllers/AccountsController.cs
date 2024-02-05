using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {

                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest("Error: " + e);
            }
            
        }
    }
}
