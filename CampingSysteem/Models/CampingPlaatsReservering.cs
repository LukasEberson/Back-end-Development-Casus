namespace CampingSystem
{
    public class Reservering
    {
        public int Id { get; set; }
        public int CampingPlekId { get; set; }
        public string KlantNaam { get; set; }
        public string KlantEmail { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }
        public decimal TotaalPrijs { get; set; }
    }
}
