using HomeBankingMindHub.Dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public Client getClientByEmail(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if(client == null)
            {
                return null;
            }
            return client;
        }
        public ClientDTO getClientDTOByEmail(string email)
        {
            Client client = getClientByEmail(email);
            if (client == null)
            {
                return null;
            }
            return new ClientDTO(client);
        }

        public IEnumerable<Client> GetAllClients()
        {
            return _clientRepository.GetAllClients();
        }

        public Client FindClientById(long id)
        {
            Client client = _clientRepository.FindById(id);

            if(client == null)
            {
                return null;
            }
             return  client;
        }
        public ClientDTO FindClientDTOById(long id)
        {
            Client client = FindClientById(id);
            if (client == null)
            {
                return null;
            }
            return new ClientDTO(client);
        }

        public bool ExistsByEmail(string email)
        {
            return _clientRepository.ExistsByEmail(email);
        }

        public void Save(Client client)
        {
            _clientRepository.Save(client);
        }

    }
}
