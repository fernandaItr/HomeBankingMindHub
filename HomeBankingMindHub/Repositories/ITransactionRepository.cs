using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ITransactionRepository
    {
        void Save(Transaction transaction);
    }
}
