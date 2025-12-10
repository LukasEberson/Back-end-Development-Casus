using Microsoft.IdentityModel.Tokens;

namespace CampingSystem
{
    public class CommandLineProgram
    {
        private readonly string title;
        private readonly Stack<CommandLineScreen> screens = [];

        public CommandLineProgram(string title)
        {
            this.title = title;
        }

        public void OpenScreen(CommandLineScreen screen)
        {
            if (this.screens.Peek() != screen)
            {
                this.screens.Push(screen);
                screen.Opened();
            }
        }

        public void CloseScreen(CommandLineScreen screen)
        {
            if (this.screens.Peek() == screen)
            {
                this.screens.Pop();
                screen.Closed();
            }
        }

        public void Run(CommandLineScreen startScreen)
        {
            this.screens.Push(startScreen);

            while (true)
            {
                if (this.screens.IsNullOrEmpty())
                {
                    break;
                }
                else
                {
                    this.DisplayClear();
                    this.DisplayTitle();

                    this.screens.Peek().Run();
                }
            }
        }

        private void DisplayClear()
        {
            Console.Clear();
        }

        private void DisplayTitle()
        {
            int lpad = (Console.WindowWidth - this.title.Length) / 2;
            int rpad = Console.WindowWidth - this.title.Length - lpad;

            Console.Write(new string('=', lpad - 1));
            Console.Write(' ');
            Console.Write(this.title);
            Console.Write(' ');
            Console.Write(new string('=', rpad - 1));
            Console.Write('\n');
        }
    }
}
