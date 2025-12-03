namespace CommandLineInterface
{
    public class CampingSystemCLI
    {
        public static void Run()
        {
            CommandLineProgram program = new CommandLineProgram("Camping Systeem");
            MainMenuScreen mainMenu = new MainMenuScreen(program);

            program.Run(mainMenu);
        }
    }
}
