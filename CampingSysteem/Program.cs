/*using CommandLineInterface;

namespace CampingSysteem
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            CampingSystemCLI.Run();
        }
    }    
}*/

﻿namespace CampingSysteem
{
    internal class Program
    {
        static List<CampingPlaats> plaatsen = new List<CampingPlaats>();

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Camping Systeem ===");
                Console.WriteLine("1. Campingplaats toevoegen");
                Console.WriteLine("2. Campingplaatsen tonen");
                Console.WriteLine("3. Stoppen");
                Console.Write("Keuze: ");

                string keuze = Console.ReadLine();

              
                if (keuze == "1")
                {
                    VoegPlaatsToe();
                }
                else if (keuze == "2")
                {
                    ToonPlaatsen();
                }
                else if (keuze == "3")
                {
                    return; 
                }
                else
                {
                    Console.WriteLine("Ongeldige keuze!");
                    Console.ReadKey();
                }
            }
        }

        static void VoegPlaatsToe()
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

        static void ToonPlaatsen()
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
