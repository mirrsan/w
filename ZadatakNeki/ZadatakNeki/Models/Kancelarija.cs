using System.Collections.Generic;

namespace ZadatakNeki.Models
{
    public class Kancelarija
    {
        public long Id { get; set; }
        public string Opis { get; set; }

        public List<Osoba> Osobe { get; set; }
    }
}
