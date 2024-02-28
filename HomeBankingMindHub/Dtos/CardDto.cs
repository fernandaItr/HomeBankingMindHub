using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Dtos
{
    public class CardDto
    {
        public long Id { get; set; }
        public string CardHolder { get; set; }
        public String Type { get; set; }
        public String Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ThruDate { get; set;}
    }
}
