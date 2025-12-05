namespace CommandLineInterface
{
    public class MakeReservationScreen : CommandLineScreen
    {
        public MakeReservationScreen(CommandLineProgram program) : base(program)
        {
        }

        public override void Opened()
        {
        }

        public override void Closed()
        {
        }

        public override void Run()
        {
            Console.ReadKey();
            this.program.CloseScreen(this);
        }
    }
}
