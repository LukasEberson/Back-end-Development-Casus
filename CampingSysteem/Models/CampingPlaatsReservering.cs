namespace CampingSystem
{
    public class CampingPlaatsReservering
    {
        public int Id { get; set; }
        public CampingPlaats? Plaats { get; set; }
        public int AantalVolwassenen { get; set; }
        public int AantalKinderenOnder7 { get; set; }
        public int AantalKinderenOnder12 { get; set; }
        public int AantalHonden { get; set; }
    }
}
