using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZadatakNeki.Models
{
    public class Osoba
    {
        public long Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }

        public long KancelarijaId { get; set; }
        [ForeignKey("KancelarijaId")]
        public Kancelarija Kancelarija { get; set; }

        public List<OsobaUredjaj> OsobaUredjaji { get; set; }
        
    }
}
