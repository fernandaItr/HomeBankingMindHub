using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Dtos;
using HomeBankingMindHub.Services;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IAccountService _accountService;
        private IClientService _clientService;
        private ITransactionService _transactionService;

        public TransactionsController(IAccountService accountService, IClientService clientService, ITransactionService transactionService)
        {
            _accountService = accountService;
            _clientService = clientService;
            _transactionService = transactionService;
        }

        //Crear transaccion
        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post(TransferDTO transfer)
        {
            //using (var scope = new TransactionScope())
            //{
                try
                {
                    //Parametros no vacios
                    if (transfer.Amount < 1 || string.IsNullOrEmpty(transfer.Description) || string.IsNullOrEmpty(transfer.FromAccountNumber) || string.IsNullOrEmpty(transfer.ToAccountNumber))
                    {
                        return StatusCode(403, "Campos vacios");
                    }

                    //Numero de cuentas distintos
                    if (transfer.FromAccountNumber == transfer.ToAccountNumber)
                    {
                        return StatusCode(403, "Numeros de cuentas iguales");
                    }

                    //Verificar que cuenta origen y destino exista
                    var accountFrom = _accountService.FindByNumber(transfer.FromAccountNumber);
                    var accountTo = _accountService.FindByNumber(transfer.ToAccountNumber);
                    if (accountFrom == null || accountTo == null)
                    {
                        return StatusCode(403, "Cuenta origen y/o destino inexistente.");
                    }

                    //Verificar que la cuenta origen pertenezca al cliente autenticado
                    string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                    if (email == string.Empty)
                    {
                        return Forbid();
                    }
                    Client currentClient = _clientService.getClientByEmail(email);

                    var account = currentClient.Accounts.FirstOrDefault(value => value.Number == transfer.FromAccountNumber);
                    if (account == null) { return Forbid(); }

                    //Verificar que la cuenta origen tenga el monto disponible
                    if (accountFrom.Balance < transfer.Amount)
                    {
                        return StatusCode(403, "Fondos insuficientes");
                    }

                    //Creacion de transacciones

                    var TransactionDTOFrom = new Models.Transaction
                    {
                        Type = TransactionType.DEBIT,
                        Amount = -transfer.Amount,
                        Description = transfer.Description + " " + transfer.FromAccountNumber,
                        Date = DateTime.Now,
                        AccountId = accountFrom.Id,
                        //Account = accountFrom
                    };
                    var TransactionDTOTo = new Models.Transaction
                    {
                        Type = TransactionType.CREDIT,
                        Amount = transfer.Amount,
                        Description = transfer.Description + " " + transfer.ToAccountNumber,
                        Date = DateTime.Now,
                        AccountId = accountTo.Id,
                        //Account = accountTo
                    };

                    _transactionService.Save(TransactionDTOFrom);
                    _transactionService.Save(TransactionDTOTo);

                    //Actualizacion de cuentas
                    accountFrom.Balance -= transfer.Amount;
                    accountTo.Balance += transfer.Amount;

                    _accountService.Save(accountFrom);
                    _accountService.Save(accountTo);

                    //scope.Complete();
                    return StatusCode(201, "exito");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            //}
        }
    }
}
