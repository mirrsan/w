using ZadatakNeki.Models;

namespace ZadatakNeki.Repositorys.IRepository
{
    public interface IKancelarijaRepository : IRepository<Kancelarija>
    {
        Kancelarija PretragaPoNazivu(string opis);
    }
}
