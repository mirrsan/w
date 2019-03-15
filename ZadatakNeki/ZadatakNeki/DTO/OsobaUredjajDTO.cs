using System;

namespace ZadatakNeki.DTO
{
    public class OsobaUredjajDTO
    {
        public DateTime PocetakKoriscenja { get; set; }
        public DateTime? KrajKoriscenja { get; set; }
        public OsobaDTO Osoba { get; set; }
        public UredjajDTO Uredjaj { get; set; }
    }
}
