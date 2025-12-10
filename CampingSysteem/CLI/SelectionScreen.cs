using System.Linq.Expressions;

namespace CampingSystem
{
    /// <summary>
    /// A screen that displays a list of options for the user the choose from.
    /// </summary>
    public class SelectionScreen : CommandLineScreen
    {
        private readonly List<string> descriptions = [];
        private readonly List<SelectionRunner> options = [];

        private string? closeKey = "t"; // t for 'terug'
        private string? closeDescription = "Ga terug.";

        private string? errormessage;

        public SelectionScreen(CommandLineProgram program) : base(program)
        {
        }
        
        protected void SetCloseKeyAndDescription(string key, string description)
        {
            this.closeKey = key;
            this.closeDescription = description;
        }

        protected void AddOption(string description, SelectionRunner option)
        {
            this.descriptions.Add(description);
            this.options.Add(option);
        }

        protected void AddOpenScreenOption(string description, CommandLineScreen screen)
        {
            this.AddOption(description, _ => this.program.OpenScreen(screen));
        }

        public override void Opened()
        {
            this.errormessage = null;
        }

        public override void Closed()
        {
        }

        public override void Run()
        {
            this.DisplayOptions();
            this.HandleInputs();
        }

        private void DisplayOptions()
        {
            for (int i = 0; i < this.descriptions.Count(); i++)
            {
                Console.WriteLine($"[{i + 1}] {this.descriptions[i]}");
            }

            Console.WriteLine($"{this.closeKey}: {this.closeDescription}");
        }

        private void HandleInputs()
        {
            Console.WriteLine(errormessage ?? "");
            Console.Write("> ");

            string? option = Console.ReadLine();

            if (option == null || option == this.closeKey)
            {
                this.program.CloseScreen(this);
            }
            else
            {
                try
                {
                    int selection = int.Parse(option);

                    if (selection < 1 || selection > this.options.Count)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else
                    {
                        this.options[selection - 1].Invoke(selection);
                    }
                }
                catch (Exception)
                {
                    errormessage = $"!! {option} is geen geldige optie!";
                }
            }
        }

        protected delegate void SelectionRunner(int selection);

    }
}
