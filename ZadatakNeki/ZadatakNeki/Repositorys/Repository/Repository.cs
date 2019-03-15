using System;
using System.Collections.Generic;
using System.Linq;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;

namespace ZadatakNeki.Repositorys.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private ToDoContext _context;

        public Repository(ToDoContext context)
        {
            _context = context;
        }

        public List<TEntity> DajSveEntitete()
        {
            return _context.Set<TEntity>().ToList();
        }

        public TEntity EntitetPoId(long id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public void DodajEntitet(TEntity entitet)
        {
            _context.Set<TEntity>().Add(entitet);
        }

        public void Izmeni(TEntity entitet)
        {
            _context.Set<TEntity>().Update(entitet);
        }

        public void ObrisiEntitet(TEntity entitet)
        {
            _context.Set<TEntity>().Remove(entitet);
        }
    }
}
