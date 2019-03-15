using System.Linq;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;
using ZadatakNeki.Repositorys.Repository;

namespace ZadatakNeki.Repository
{
    public class KancelarijaRepository : Repository<Kancelarija>, IKancelarijaRepository
    {
        private readonly ToDoContext _context;

        public KancelarijaRepository(ToDoContext context) : base(context)
        {
            _context = context;
        }

        public Kancelarija PretragaPoNazivu(string opis)
        {
            var poImenu = _context.Kancelarije.Where(e => e.Opis == opis).FirstOrDefault();
            return poImenu;
        }
    }
}
