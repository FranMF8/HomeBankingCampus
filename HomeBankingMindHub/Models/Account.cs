using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBankingMindHub.Models
{
    public class Account
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Balance { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
}
