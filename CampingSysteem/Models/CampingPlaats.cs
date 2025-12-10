namespace CampingSystem
{
    public class CampingPlaats
    {
        public int Id { get; set; }
        public CampingPlaatsType? Type { get; set; }
        public int Nummer {  get; set; }
        public List<CampingPlaatsReservering> Reserveringen { get; set; } = [];

        public void PrettyPrint()
        {
            this.PrettyPrint("");
        }

        public void PrettyPrint(string prefix)
        {
            Console.WriteLine($"{prefix}[{this.Id}]");
            prefix += "  ";
            this.Type?.PrettyPrint(prefix + "Type: ");
            Console.WriteLine($"{prefix}Nummer: {this.Nummer}");
        }
    }
}
