namespace CampingSystem
{
    public class CampingPlaats
    {
        public int Id { get; set; }
        public int Nummer {  get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"Campingplaats #{Nummer} - Type: {Type}";
        }
    }
}
