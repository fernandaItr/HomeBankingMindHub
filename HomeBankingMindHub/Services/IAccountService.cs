using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAllAccounts();
        bool ExistsByNumber(string number);
        void Save(Account account);
        Account FindByIdAndClientEmail(long id, string email);
        Account FindByNumber(string number);
    }
}
