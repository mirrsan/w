using System.Collections.Generic;

namespace ZadatakNeki.Repositorys.IRepository
{
    public interface IRepository<TEntitet> where TEntitet : class
    {
        List<TEntitet> DajSveEntitete();
        TEntitet EntitetPoId(long id);
        void DodajEntitet(TEntitet entitet);
        void Izmeni(TEntitet entitet);
        void ObrisiEntitet(TEntitet entitet);
    }
}
