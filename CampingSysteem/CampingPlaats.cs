using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingSysteem
{
    internal class CampingPlaats
    {

        public int Nummer {  get; set; }
        public string Type { get; set; }

        public CampingPlaats(int nummer, string type) 
        { 
            Nummer = nummer;
            Type = type;
        }

        public override string ToString()
        {
            return $"Campingplaats #{Nummer} - Type: {Type}";
        }
    }
}
