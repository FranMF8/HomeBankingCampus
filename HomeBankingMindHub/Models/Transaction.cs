using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMindHub.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }

        public Account Account { get; set; }
        public long AccountId { get; set; }
    }
}
