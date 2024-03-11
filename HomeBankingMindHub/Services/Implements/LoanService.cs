using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }
        public IEnumerable<Loan> GetAll()
        {
            return _loanRepository.GetAll();
        }
        public Loan FindById(long id)
        {
            Loan loan = _loanRepository.FindById(id);
            if (loan == null)
            {
                return null;
            }
            return loan;
        }
    }
}
