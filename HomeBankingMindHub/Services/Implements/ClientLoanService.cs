using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class ClientLoanService : IClientLoanService
    {
        public readonly IClientLoanRepository _clientLoanRepository;
        public ClientLoanService(IClientLoanRepository clientLoanRepository)
        {
            _clientLoanRepository = clientLoanRepository;
        }
        public void Save(ClientLoan clientLoan)
        {
            _clientLoanRepository.Save(clientLoan);
        }
    }
}
