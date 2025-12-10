namespace CampingSystem
{
    public class CampingPlek
    {
        public int Id { get; set; }
        public string Nummer { get; set; }  // bijv. "A1", "B5"
        public string Type { get; set; }    // bijv. "Tent", "Camper", "Caravan"
        public decimal PrijsPerNacht { get; set; }
    }
}
