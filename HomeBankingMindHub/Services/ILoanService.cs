using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        IEnumerable<Loan> GetAll();
        Loan FindById(long id);
    }
}
