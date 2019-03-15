using System;
using System.Collections.Generic;

namespace ZadatakNeki.Models
{
    public class VratiRezultat
    {
        public object Data { get; set; }
        public bool IsError { get; set; }
        public Error Error { get; set; }
    }
}
