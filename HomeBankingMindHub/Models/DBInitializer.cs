using System;
using System.Linq;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123456"},
                    new Client { Email = "marcosGG@gmail.com", FirstName="Marcos", LastName="Gonzalez", Password="678910"},
                    new Client { Email = "veronuñez@gmail.com", FirstName="Veronica", LastName="Nuñez", Password="345623"}
                };

                context.Clients.AddRange(clients);

                context.SaveChanges();
            }

        }
    }
}
