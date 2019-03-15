using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;
using ZadatakNeki.Repositorys.Repository;

namespace ZadatakNeki.Repository
{
    public class OsobaUredjajRepository : Repository<OsobaUredjaj>, IOsobaUredjajRepository
    {
        private readonly ToDoContext _context;

        public OsobaUredjajRepository(ToDoContext context) : base(context)
        {
            _context = context;
        }
        
        public List<OsobaUredjaj> DajSveEntitete()
        {
            return _context.OsobaUredjaj.Include(o => o.Osoba)
                                        .Include(u => u.Uredjaj)
                                        .Include(k => k.Osoba.Kancelarija).ToList();
        }
        
        public OsobaUredjaj EntitetPoId(long id)
        {
            return _context.OsobaUredjaj.Include(o => o.Osoba)
                                        .Include(u => u.Uredjaj)
                                        .Include(k => k.Osoba.Kancelarija)
                                        .Where(osobaUredjaj => osobaUredjaj.Id == id).FirstOrDefault();
        }
    }
}
