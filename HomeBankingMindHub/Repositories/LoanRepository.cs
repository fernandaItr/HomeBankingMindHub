using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll()
                .ToList();
        }

        public Loan FindById(long id)
        {
            return FindByCondition(Loan=>Loan.Id == id)
                .FirstOrDefault();
        }
        public void Save(Loan loan)
        {
            Create(loan);
            SaveChanges();
        }
    }
}
