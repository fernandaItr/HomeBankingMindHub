using HomeBankingMindHub.Dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientService _clientService;
        private IAccountService _accountService;
        private ILoanService _loanService;
        private IClientLoanService _clientLoanService;
        private ITransactionService _transactionService;

        public LoansController(IClientService clientService, IAccountService accountService, ILoanService loanService, IClientLoanService clientLoanService, ITransactionService transactionService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _loanService = loanService;
            _clientLoanService = clientLoanService;
            _transactionService = transactionService;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanService.GetAll();
                
                var loansDTO = new List<LoanDTO>();

                foreach(Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };
                    loansDTO.Add(newLoanDTO);
                }

                return Ok(loansDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Metodo post q reciba LoanApplicationDTO 
        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    //Obtener el prestamo a pedir y verificar que el prestamo exista
                    Loan loan = _loanService.FindById(loanApplicationDTO.LoanId);

                    //Verificar que los datos no esten vacios y monto 
                    if (loanApplicationDTO.Amount < 1 || loanApplicationDTO.Payments.IsNullOrEmpty() || loanApplicationDTO.AccountNumber.IsNullOrEmpty())
                    {
                        return Forbid();
                    }

                    //Verificar cuotas
                    string[] paymentValues = loan.Payments.Split(',');

                    string paymentValue = paymentValues.FirstOrDefault(value => value == loanApplicationDTO.Payments);

                    if(paymentValue.IsNullOrEmpty()) { return  Forbid(); }

                    //Verificar que el monto solicitado no exceda el monto maximo del prestamo
                    if (loanApplicationDTO.Amount > loan.MaxAmount) { return Forbid(); };

                    //Obtener cliente autenticado
                    string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                    if (email == string.Empty)
                    {
                        return Forbid();
                    }

                    Client client = _clientService.getClientByEmail(email);

                    //Cuentas
                    Account account = _accountService.FindByNumber(loanApplicationDTO.AccountNumber);

                    if(account.ClientId != client.Id) { return Forbid(); }

                    //Crear solicitud de prestamo
                    ClientLoan newClientLoan = new ClientLoan
                    {
                        Amount = loanApplicationDTO.Amount + (loanApplicationDTO.Amount * 0.20),
                        Payments = loanApplicationDTO.Payments,
                        ClientId = client.Id,
                        LoanId = loanApplicationDTO.LoanId,
                    };

                    _clientLoanService.Save(newClientLoan);

                    //Crear una transaccion asociada a la cuenta destino

                    Models.Transaction transaction = new Models.Transaction
                    {
                        Type = TransactionType.CREDIT,
                        Amount = loanApplicationDTO.Amount,
                        Description = loan.Name + " loan approved",
                        Date = DateTime.Now,
                        AccountId = account.Id,
                    };

                    _transactionService.Save(transaction);

                    //Actualizar cuenta destino sumando el monton solicitado

                    account.Balance += loanApplicationDTO.Amount;
                    _accountService.Save(account);

                    scope.Complete();
                    return StatusCode(201, "Exito");

                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }

    }
}
