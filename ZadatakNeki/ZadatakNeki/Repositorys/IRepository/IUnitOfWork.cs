using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZadatakNeki.Repositorys.IRepository
{
    public interface IUnitOfWork
    {
        void PocniTransakciju();
        void Sacuvaj();
        void ZavrsiTransakciju();
    }
}
