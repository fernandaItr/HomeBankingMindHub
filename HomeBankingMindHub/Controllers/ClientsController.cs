//using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeBankingMindHub.Utils;
using System.Security.Principal;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;

        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();

                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number,
                        }).ToList(),
                        Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),
                        Cards = client.Cards.Select(c => new CardDto
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color.ToString(),
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type.ToString()
                        }).ToList()
                    };

                    clientsDTO.Add(newClientDTO);
                }

                return Ok(clientsDTO);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number,
                    }).ToList(),
                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDto
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()
                };

                return Ok(clientDTO);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")] 
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
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

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number,
                    }).ToList(),
                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDto
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Crear cliente con una cuenta asociada
        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email))
                    return StatusCode(403, "Email invalido");

                if (String.IsNullOrEmpty(client.Password))
                    return StatusCode(403, "Contraseña invalida");

                if (String.IsNullOrEmpty(client.FirstName))
                    return StatusCode(403, "Nombre invalido");

                if (String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "Apellido invalido");

                //buscamos si el usuario ya existe
                if (_clientRepository.ExistsByEmail(client.Email))
                {
                    return StatusCode(403, "Email esta en uso");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);

                //creamos num de cuenta y buscamos si el numero de cuenta ya existe     
                string accountNumber;
                do
                {
                    var random = RandomNumbers.GenerateRandomInt(0, 99999999);

                    accountNumber = "VIN-" + random.ToString();

                } while (_accountRepository.ExistsByNumber(accountNumber));

                Account newAccount = new Account
                {
                    Number = accountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = newClient.Id,
                };

                _accountRepository.Save(newAccount);
                return Created("", "Cliente y cuenta creado con exito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET accounts
        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);

                var accountslist = client.Accounts.ToList();

                var accountsDTO = new List<AccountDTO>();

                foreach (Account account in accountslist)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,
                        Balance = account.Balance,
                        CreationDate = account.CreationDate,
                        Number = account.Number,
                    };

                    accountsDTO.Add(newAccountDTO);
                }

                return Ok(accountslist);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //POST account
        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostAccount()
        {
            try
            {
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

                //validamos datos antes

                //Verificar que el cliente no tenga mas de 3 cuentas registradas
                if (client.Accounts.Count > 2)
                {
                    return StatusCode(403, "Prohibido tener mas de 3 cuentas");
                }

                //creamos num de cuenta y buscamos si el numero de cuenta ya existe     
                string accountNumber;
                do
                {
                    var random = RandomNumbers.GenerateRandomInt(0, 99999999);

                    accountNumber = "VIN-" + random.ToString();

                } while (_accountRepository.ExistsByNumber(accountNumber));

                Account newAccount = new Account
                {
                    Number = accountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id,
                };

                _accountRepository.Save(newAccount);
                return StatusCode(201, "Creada");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET cards
        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCards()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);

                var cardslist = client.Cards.ToList();

                var cardsDTO = new List<CardDto>();

                foreach (Card card in cardslist)
                {
                    var newCardDTO = new CardDto
                    {                        
                            Id = card.Id,
                            CardHolder = card.CardHolder,
                            Color = card.Color.ToString(),
                            Cvv = card.Cvv,
                            FromDate = card.FromDate,
                            Number = card.Number,
                            ThruDate = card.ThruDate,
                            Type = card.Type.ToString()
                    };

                    cardsDTO.Add(newCardDTO);
                }

                return Ok(cardslist);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //POST cards
        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostCard(CardPreferenceDTO cardPreferenceDTO)
        {
            try
            {
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

                foreach(var card in client.Cards)
                {
                    if(card.Type.ToString() == cardPreferenceDTO.Type && card.Color.ToString() == cardPreferenceDTO.Color)
                    {
                        return StatusCode(403, "Prohibido, limite de tarjetas");
                    }
                }

                //Generar numero de tarjeta
                string cardNumber;
                do
                {
                    var random = RandomNumbers.GenerateRandomLong(1111111111111111, 9999999999999999);
                    cardNumber = random.ToString();

                } while (_cardRepository.ExistsByNumber(cardNumber));

                //Generar cvv
                var randomCvv = RandomNumbers.GenerateRandomInt(0, 999);
                    
                Card newCard = new Card
                {
                        CardHolder = client.FirstName + " " + client.LastName,
                        Type = (CardType)Enum.Parse(typeof(CardType), cardPreferenceDTO.Type),
                        Color = (CardColor)Enum.Parse(typeof(CardColor), cardPreferenceDTO.Color),
                        Number = cardNumber,
                        Cvv = randomCvv,
                        FromDate = DateTime.Now,
                        ThruDate = (DateTime.Now).AddYears(5),
                        ClientId = client.Id
                };

                    _cardRepository.Save(newCard);
                    return StatusCode(201, newCard);
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
