using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;
using ZadatakNeki.Repositorys.Repository;

namespace ZadatakNeki.Repository
{
    public class OsobaRepository : Repository<Osoba>, IOsobaRepository
    {
        private readonly ToDoContext _context;

        public OsobaRepository(ToDoContext context) : base(context)
        {
            _context = context;
        }
        
        public List<Osoba> DajSveEntitete()
        {
            return _context.Osobe.Include(p => p.Kancelarija).ToList();
        }

        public Osoba EntitetPoId(long id)
        {
            return _context.Osobe.Include(k => k.Kancelarija).Where(o => o.Id == id).FirstOrDefault();
        }
        
        public Osoba PretragaPoImenu(string ime)
        {
            return _context.Osobe.Include(k => k.Kancelarija).Where(i => i.Ime == ime).FirstOrDefault();
        }
    }
}
