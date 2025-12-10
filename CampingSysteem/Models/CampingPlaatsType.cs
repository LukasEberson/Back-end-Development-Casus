namespace CampingSystem
{
    public class CampingPlaatsType
    {
        public int Id { get; set; }
        public List<CampingPlaats> Plaatsen { get; set; } = [];

        public void PrettyPrint()
        {
            this.PrettyPrint("", "");
        }

        public void PrettyPrint(string prefix, string name)
        {
            Console.WriteLine($"{prefix}{name}[{this.Id}]");
        }
    }
}
