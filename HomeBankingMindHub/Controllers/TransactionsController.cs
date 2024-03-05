using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Dtos;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IAccountRepository _accountRepository;
        private IClientRepository _clientRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IAccountRepository accountRepository, IClientRepository clientRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
            _transactionRepository = transactionRepository;
        }

        //Crear transaccion
        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post(TransferDTO transfer)
        {
            try
            {
                //Parametros no vacios
                if (transfer.Amount < 1 || string.IsNullOrEmpty(transfer.Description) || string.IsNullOrEmpty(transfer.FromAccountNumber) || string.IsNullOrEmpty(transfer.ToAccountNumber)) { 
                    return StatusCode(403, "Campos vacios");
                }

                //Numero de cuentas distintos
                if (transfer.FromAccountNumber == transfer.ToAccountNumber)
                {
                    return StatusCode(403, "Numeros de cuentas iguales");
                }

                //Verificar que cuenta origen y destino exista
                var accountFrom = _accountRepository.FindByNumber(transfer.FromAccountNumber);
                var accountTo = _accountRepository.FindByNumber(transfer.ToAccountNumber);
                if (accountFrom == null || accountTo == null)
                {
                    return StatusCode(403, "Cuenta origen y/o destino inexistente.");
                }

                //Verificar que la cuenta origen pertenezca al cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if(email == string.Empty)
                {
                    return Forbid();
                }
                Client currentClient = _clientRepository.FindByEmail(email);

                bool flag = false;
                foreach(Account account in currentClient.Accounts)
                {
                    if(account.Number == transfer.FromAccountNumber)
                    {                        
                        flag = true;
                    }
                }
                if(!flag)
                {
                    return StatusCode(403, "Cuenta distinta a usuario autenticado.");
                }

                //Verificar que la cuenta origen tenga el monto disponible
                if (accountFrom.Balance < transfer.Amount)
                {
                    return StatusCode(403, "Fondos insuficientes");
                }

                //Creacion de transacciones

                var TransactionDTOFrom = new Transaction
                {
                    Type = TransactionType.DEBIT,
                    Amount = -transfer.Amount,
                    Description = transfer.Description + " " + transfer.FromAccountNumber,
                    Date = DateTime.Now,
                    AccountId = accountFrom.Id,
                };
                var TransactionDTOTo = new Transaction
                {
                    Type = TransactionType.CREDIT,
                    Amount = transfer.Amount,
                    Description = transfer.Description + " " + transfer.ToAccountNumber,
                    Date = DateTime.Now,
                    AccountId = accountTo.Id,
                };
                
                _transactionRepository.Save(TransactionDTOFrom);
                _transactionRepository.Save(TransactionDTOTo);

                //Actualizacion de cuentas
                accountFrom.Balance -= transfer.Amount;
                accountTo.Balance += transfer.Amount;

                _accountRepository.Save(accountFrom);
                _accountRepository.Save(accountTo);

                return StatusCode(201, "exito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
