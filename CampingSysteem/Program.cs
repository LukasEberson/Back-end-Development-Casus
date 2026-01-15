namespace CampingSystem
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            CampingSystemCLI.Run();
            DAL dal = new DAL();

            dal.GetCampingPlaatsReserveringen();
            dal.GetCampingPlaatsen();
            dal.GetCampingPlaatsTypen();

            CampingPlaatsType type = new CampingPlaatsType()
            {
                Id = 1
            };
            CampingPlaats plaats = new CampingPlaats()
            {
                Id = 1,
                Type = type,
                Nummer = 1
            };
            CampingPlaatsReservering reservering = new CampingPlaatsReservering()
            {
                Id = 1,
                Plaats = plaats,
                AantalVolwassenen = 2,
                AantalKinderenOnder7 = 0,
                AantalKinderenOnder12 = 2,
                AantalHonden = 1
            };

            dal.CreateCampingPlaatsType(type);
            dal.CreateCampingPlaats(plaats);
            dal.CreateCampingPlaatsReservering(reservering);

            plaats.Nummer = 2;
            reservering.AantalKinderenOnder7 = 1;

            dal.UpdateCampingPlaatsType(type);
            dal.UpdateCampingPlaats(plaats);
            dal.UpdateCampingPlaatsReservering(reservering);

            dal.GetCampingPlaatsType(1);
            dal.GetCampingPlaats(1);
            dal.GetCampingPlaatsReservering(1);

            dal.DeleteCampingPlaatsType(type);
            dal.DeleteCampingPlaats(plaats);
            dal.DeleteCampingPlaatsReservering(reservering);
        }
    }    
}
