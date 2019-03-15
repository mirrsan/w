using ZadatakNeki.Models;

namespace ZadatakNeki.Repositorys.IRepository
{
    public interface IUredjajRepository : IRepository<Uredjaj>
    {
        Uredjaj PretragaPoImenu(string naziv);
    }
}
