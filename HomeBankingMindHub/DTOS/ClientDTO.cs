using HomeBankingMindHub.Models;
using System.Text.Json.Serialization;

namespace HomeBankingMindHub.DTOS
{
    public class ClientDTO
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Credits { get; set; }
        public ICollection<CardDTO> Cards { get; set; }

        public ClientDTO(Client client)
        {
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;

            List<AccountDTO> accountsDTO = new List<AccountDTO>();
            foreach (var account in client.Accounts)
            {
                AccountDTO accountDTO = new AccountDTO(account);
                accountsDTO.Add(accountDTO);
            }

            Accounts = accountsDTO;

            List<ClientLoanDTO> loans = new List<ClientLoanDTO>();

            foreach (var loan in client.ClientLoans)
            {
                ClientLoanDTO loanDTO = new ClientLoanDTO(loan);
                loans.Add(loanDTO);
            }
        }
    }
}
