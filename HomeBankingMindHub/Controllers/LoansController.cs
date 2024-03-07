using HomeBankingMindHub.Dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
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
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        //[Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAll();
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
                    var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                    if (loan == null)
                    {
                        return Forbid();
                    }

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

                    Client client = _clientRepository.FindByEmail(email);

                    if (client == null)
                    {
                        return Forbid();
                    }

                    //Cuentas
                    Account account = _accountRepository.FindByNumber(loanApplicationDTO.AccountNumber);

                    if(account == null) { return Forbid(); }

                    if(account.ClientId != client.Id) { return Forbid(); }

                    //Crear solicitud de prestamo
                    ClientLoan newClientLoan = new ClientLoan
                    {
                        Amount = loanApplicationDTO.Amount + (loanApplicationDTO.Amount * 0.20),
                        Payments = loanApplicationDTO.Payments,
                        ClientId = client.Id,
                        LoanId = loanApplicationDTO.LoanId,
                    };

                    _clientLoanRepository.Save(newClientLoan);

                    //Crear una transaccion asociada a la cuenta destino

                    Models.Transaction transaction = new Models.Transaction
                    {
                        Type = TransactionType.CREDIT,
                        Amount = loanApplicationDTO.Amount,
                        Description = loan.Name + " loan approved",
                        Date = DateTime.Now,
                        AccountId = account.Id,
                    };

                    _transactionRepository.Save(transaction);

                    //Actualizar cuenta destino sumando el monton solicitado

                    account.Balance += loanApplicationDTO.Amount;
                    _accountRepository.Save(account);

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
