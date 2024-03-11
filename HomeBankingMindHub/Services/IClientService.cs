using HomeBankingMindHub.Dtos;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        ClientDTO getClientDTOByEmail(string email);
        Client getClientByEmail(string email);
        IEnumerable<Client> GetAllClients();
        Client FindClientById(long id);
        ClientDTO FindClientDTOById(long id);
        bool ExistsByEmail(string email);
        void Save(Client client);
    }
}
