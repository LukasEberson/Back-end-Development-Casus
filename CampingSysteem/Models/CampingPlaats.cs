namespace CampingSystem
{
    public class CampingPlaats
    {
        public int Id { get; set; }
        public CampingPlaatsType? Type { get; set; }
        public List<CampingPlaatsReservering> Reserveringen { get; set; } = [];
        public int Nummer {  get; set; }

        public override string ToString()
        {
            return $"Campingplaats #{Nummer} - Type: {Type}";
        }
    }
}
