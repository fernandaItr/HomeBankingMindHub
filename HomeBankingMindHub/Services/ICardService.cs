using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        void Save(Card card);
        bool ExistsByNumber(string number);
    }
}
