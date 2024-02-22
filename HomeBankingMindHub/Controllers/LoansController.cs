using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoansController(ILoanService loanService) {
            _loanService = loanService;
        }

        [HttpPost]
        public IActionResult ApplicateForLoan(LoanApplicationDTO loan)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return StatusCode(403, "Error en la autenticacion");

                string message = _loanService.ApplicateForLoan(loan, email);

                if (message != "ok")
                    return StatusCode(403, message);

                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet]
        public IActionResult GetLoans()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                    return StatusCode(403, "Error en la autenticacion");

                List<LoanDTO> result = _loanService.GetLoans(email);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
