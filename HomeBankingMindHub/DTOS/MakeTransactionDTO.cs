namespace HomeBankingMindHub.DTOS
{
    public class MakeTransactionDTO
    {
        public double Amount { get; set; }
        public string Description { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
    }
}
