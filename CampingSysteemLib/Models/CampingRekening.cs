using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingSystem
{
    public class CampingRekening
    {
        public int Id { get; set; }
        public int ToeristenBelasting { get; set; }
        public int Korting { get; set; }
        public bool Betaald { get; set; }
    }
}
