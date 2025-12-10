namespace CampingSystem
{
    /// <summary>
    /// The base class for a CLI screen. A screen takes up the whole console window.
    /// </summary>
    public abstract class CommandLineScreen
    {

        protected CommandLineProgram program;

        protected CommandLineScreen(CommandLineProgram program)
        {
            this.program = program;
        }

        public abstract void Opened();

        public abstract void Closed();

        public abstract void Run();

    }
}
