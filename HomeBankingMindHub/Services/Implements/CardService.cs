using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        public CardService(ICardRepository cardRepository) 
        {
            _cardRepository = cardRepository;        
        }
        public bool ExistsByNumber(string number)
        {
            return _cardRepository.ExistsByNumber(number);
        }
        public void Save(Card card)
        {
            _cardRepository.Save(card); 
        }
    }
}
