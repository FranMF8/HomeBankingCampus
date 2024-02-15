using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMindHub.DTOS
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }
}
