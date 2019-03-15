using ZadatakNeki.Models;

namespace ZadatakNeki.Repositorys.IRepository
{
    public interface IOsobaRepository : IRepository<Osoba>
    {
        Osoba PretragaPoImenu(string ime);
    }
}
