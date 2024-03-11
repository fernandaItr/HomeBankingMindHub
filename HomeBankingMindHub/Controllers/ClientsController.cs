//using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Dtos;
using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeBankingMindHub.Utils;
using System.Security.Principal;
using HomeBankingMindHub.Services;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientService _clientService;
        private IAccountService _accountService;
        private ICardService _cardService;

        public ClientsController(IClientService clientService, IAccountService accountService, ICardService cardService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _cardService = cardService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientService.GetAllClients();

                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    ClientDTO newClientDTO = new ClientDTO(client);

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
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                ClientDTO client = _clientService.FindClientDTOById(id);         

                return Ok(client);
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

                ClientDTO client = _clientService.getClientDTOByEmail(email);

                return Ok(client);
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
                if(_clientService.ExistsByEmail(client.Email))
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

                _clientService.Save(newClient);

                //creamos num de cuenta y buscamos si el numero de cuenta ya existe     
                string accountNumber;
                do
                {
                    var random = RandomNumbers.GenerateRandomInt(0, 99999999);

                    accountNumber = "VIN-" + random.ToString();

                } while (_accountService.ExistsByNumber(accountNumber));

                Account newAccount = new Account
                {
                    Number = accountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = newClient.Id,
                };

                _accountService.Save(newAccount);
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
                Client client = _clientService.getClientByEmail(email);

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

                Client client = _clientService.getClientByEmail(email);

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

                } while (_accountService.ExistsByNumber(accountNumber));

                Account newAccount = new Account
                {
                    Number = accountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id,
                };

                _accountService.Save(newAccount);
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
                Client client = _clientService.getClientByEmail(email);

                var cardslist = client.Cards.ToList();

                var cardsDTO = new List<CardDto>();

                foreach (Card card in cardslist)
                {
                    CardDto newCardDTO = new CardDto(card);

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

                Client client = _clientService.getClientByEmail(email);

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
                    var random1 = RandomNumbers.GenerateRandomLong(1000, 9999);
                    var random2 = RandomNumbers.GenerateRandomLong(1000, 9999);
                    var random3 = RandomNumbers.GenerateRandomLong(1000, 9999);
                    var random4 = RandomNumbers.GenerateRandomLong(1000, 9999);
                    cardNumber = (random1 + "-" + random2 + "-" + random3 + "-" + random4).ToString();

                } while (_cardService.ExistsByNumber(cardNumber));

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

                    _cardService.Save(newCard);
                    return StatusCode(201, newCard);
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
