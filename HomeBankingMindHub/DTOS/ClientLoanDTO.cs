using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DTOS
{
    public class ClientLoanDTO
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int Payments { get; set; }


        //ARREGLAR POR FAVOR
        public ClientLoanDTO(ClientLoan loan) 
        {
            Id = loan.Id;
            LoanId = loan.LoanId;      
        }
    }
}
