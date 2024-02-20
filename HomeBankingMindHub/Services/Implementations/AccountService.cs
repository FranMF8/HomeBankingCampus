using HomeBankingMindHub.DTOS;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;

namespace HomeBankingMindHub.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public List<AccountDTO> GetAll()
        {
            List<Account> accounts = _accountRepository.GetAllAccounts().ToList();

            List<AccountDTO> accountsDTO = new List<AccountDTO>();

            foreach (var account in accounts)
            {
                var newAccountDTO = new AccountDTO(account);

                accountsDTO.Add(newAccountDTO);
            }

            return accountsDTO;
        }

        public AccountDTO GetById(long id)
        {
            var account = _accountRepository.FindById(id);

            if (account == null)
                return null;

            AccountDTO accountDTO = new AccountDTO(account);

            return accountDTO;
        }
    }
}
