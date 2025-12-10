using CampingSysteem;

namespace CampingSystem
{
    public class MainMenuScreen : SelectionScreen
    {
        private List<CampingPlaats> plaatsen = new List<CampingPlaats>();
        
        public MainMenuScreen(CommandLineProgram program) : base(program)
        {
            this.SetCloseKeyAndDescription("s", "Afsluiten.");
//            this.AddOpenScreenOption("Maak reservering.", new MakeReservationScreen(program));
            this.AddOption("Voeg campingplaats toe.", _ => this.VoegPlaatsToe());
            this.AddOption("Toon campingplaatsen.", _ => this.ToonPlaatsen());
        }

        private void VoegPlaatsToe()
        {
            Console.Clear();
            Console.Write("Plaatsnummer: ");
            int nummer = int.Parse(Console.ReadLine());

            Console.Write("Type: ");
            string? type = Console.ReadLine();

            if (string.IsNullOrEmpty(type))
            {
                Console.WriteLine("Type mag niet leeg zijn");
                Console.ReadKey();
                return;
            }

            plaatsen.Add(new CampingPlaats(nummer, type));

            Console.WriteLine("\nCampingplaats toegevoegd!");
            Console.ReadKey();
        }

        private void ToonPlaatsen()
        {
            Console.Clear();
            Console.WriteLine("=== Alle Campingplaatsen ===\n");

            if (plaatsen.Count == 0)
            {
                Console.WriteLine("Geen campingplaatsen beschikbaar.");
            }
            else
            {
                foreach (var plaats in plaatsen)
                {
                    Console.WriteLine(plaats);
                }
            }

            Console.ReadKey();
        }
    }
}
