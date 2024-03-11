using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public bool ExistsByNumber(string number)
        {
            return _accountRepository.ExistsByNumber(number);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _accountRepository.GetAllAccounts();
        }
        public void Save(Account account)
        {
            _accountRepository.Save(account);
        }
        public Account FindByIdAndClientEmail(long id, string email)
        {
            Account account = _accountRepository.FindByIdAndClientEmail(id, email);
            if(account == null)
            {
                return null;
            }
            return account;
        }
        public Account FindByNumber(string number)
        {
            Account account = _accountRepository.FindByNumber(number);
            if(account == null)
            {
                return null;
            }
            return account;
        }
    }
}
