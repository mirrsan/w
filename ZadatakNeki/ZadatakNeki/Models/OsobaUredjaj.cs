using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZadatakNeki.Models
{
    public class OsobaUredjaj
    {
        public long Id { get; set; }
        public DateTime PocetakKoriscenja { get; set; }
        public DateTime? KrajKoriscenja { get; set; }

        public long OsobaId { get; set; }
        [ForeignKey("OsobaId")]
        public Osoba Osoba { get; set; }

        public long UredjajId { get; set; }
        [ForeignKey("UredjajId")]
        public Uredjaj Uredjaj { get; set; }

    }
}
