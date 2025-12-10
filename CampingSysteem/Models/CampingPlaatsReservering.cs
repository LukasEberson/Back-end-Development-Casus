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

        public void PrettyPrint()
        {
            this.PrettyPrint("", "");
        }

        public void PrettyPrint(string prefix, string name)
        {
            Console.WriteLine($"{prefix}{name}[{this.Id}]");
            prefix += "  ";
            this.Plaats?.PrettyPrint(prefix, "Plaats: ");
            Console.WriteLine($"{prefix}AantalVolwassenen: {this.AantalVolwassenen}");
            Console.WriteLine($"{prefix}AantalKinderenOnder7: {this.AantalKinderenOnder7}");
            Console.WriteLine($"{prefix}AantalKinderenOnder12: {this.AantalKinderenOnder12}");
            Console.WriteLine($"{prefix}AantalHonden: {this.AantalHonden}");
        }
    }
}
