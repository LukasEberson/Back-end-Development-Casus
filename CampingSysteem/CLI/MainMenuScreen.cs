namespace CommandLineInterface
{
    public class MainMenuScreen : SelectionScreen
    {
        public MainMenuScreen(CommandLineProgram program) : base(program)
        {
            this.SetCloseKeyAndDescription("s", "Afsluiten.");
            this.AddOpenScreenOption("Maak reservering.", new MakeReservationScreen(program));
        }
    }
}
