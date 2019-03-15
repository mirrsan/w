using System.Linq;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;
using ZadatakNeki.Repositorys.Repository;

namespace ZadatakNeki.Repository
{
    public class UredjajRepository : Repository<Uredjaj>, IUredjajRepository
    {
        private readonly ToDoContext _context;

        public UredjajRepository(ToDoContext context) : base(context)
        {
            _context = context;
        }
        
        public Uredjaj PretragaPoImenu(string naziv)
        {
            var poImenu = _context.Uredjaji.Where(c => c.Naziv == naziv).FirstOrDefault();
            return poImenu;
        }
    }
}
