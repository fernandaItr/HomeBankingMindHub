using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        Account FindByIdAndClientEmail(long id, string email);
        IEnumerable<Account> GetAccountsByClient(long cliendId);
        bool ExistsByNumber(string number);
        Account FindByNumber(string number);
    }
}
