using CampingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingSystem
{
    public class CampingReservering
    {
        public int Id { get; set; }

        public CampingRekening? Rekening { get; set; }

        public string Naam { get; set; } = "";
        public string Emailadres { get; set; } = "";
        public string Telefoonnummer { get; set; } = "";

        public DateTime? BeginDatum { get; set; }
        public DateTime? EindDatum { get; set; }
    }
}
