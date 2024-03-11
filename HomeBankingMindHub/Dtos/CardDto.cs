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

        public CardDto() { }
        public CardDto(Card card)
        {
            Id = card.Id;
            CardHolder = card.CardHolder;
            Type = card.Type.ToString();
            Color = card.Color.ToString();
            Number = card.Number;
            Cvv = card.Cvv;
            FromDate = card.FromDate;
            ThruDate = card.ThruDate;
        }
    }
}
