using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }

        public bool ExistsByNumber(string number)
        {
            return FindByCondition(card => card.Number == number).Any();
        }
    }
}
