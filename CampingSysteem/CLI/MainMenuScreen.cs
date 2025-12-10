namespace CampingSystem
{
    public class MainMenuScreen : SelectionScreen
    {
        private List<CampingPlaats> plaatsen = new List<CampingPlaats>();
        
        public MainMenuScreen(CommandLineProgram program) : base(program)
        {
            this.SetCloseKeyAndDescription("s", "Afsluiten.");
//            this.AddOpenScreenOption("Maak reservering.", new MakeReservationScreen(program));
            this.AddOption("Toon campingplaats typen.", _ => this.ToonCampingPlaatsTypen());
            this.AddOption("Toon campingplaatsen.", _ => this.ToonCampingPlaatsen());
            this.AddOption("Toon campingplaats reserveringen.", _ => this.ToonCampingPlaatsReserveringen());
        }

        private void ToonCampingPlaatsTypen()
        {
            DAL dal = new DAL();
            List<CampingPlaatsType> typen = dal.GetCampingPlaatsTypen();

            foreach (CampingPlaatsType type in typen)
            {
                type.PrettyPrint();
            }

            Console.ReadKey();
        }

        private void ToonCampingPlaatsen()
        {
            DAL dal = new DAL();
            List<CampingPlaats> plaatsen = dal.GetCampingPlaatsen();

            foreach (CampingPlaats plaats in plaatsen)
            {
                plaats.PrettyPrint();
            }

            Console.ReadKey();
        }

        private void ToonCampingPlaatsReserveringen()
        {
            DAL dal = new DAL();
            List<CampingPlaatsReservering> reserveringen = dal.GetCampingPlaatsReserveringen();

            foreach (CampingPlaatsReservering reservering in reserveringen)
            {
                reservering.PrettyPrint();
            }

            Console.ReadKey();
        }
    }
}
