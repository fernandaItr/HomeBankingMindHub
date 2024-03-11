using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        void Save(Transaction transaction);
    }
}
