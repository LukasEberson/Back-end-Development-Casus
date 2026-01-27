using CampingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingSystem
{
    public class CampingPlaatsTarieven
    {
        public int Id { get; set; }
        public CampingPlaatsType? Type { get; set; }

        public DateTime? GeldigVan { get; set; }
        public DateTime? GeldigTot { get; set; }

        public int TariefVolwassenen { get; set; }
        public int TariefKinderenOnder7 { get; set; }
        public int TariefKinderenOnder12 { get; set; }
        public int TariefHonden { get; set; }
        public int TariefElectriciteit { get; set; }
    }
}
