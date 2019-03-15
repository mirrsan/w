using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;

namespace ZadatakNeki.Repositorys.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ToDoContext _context;
        private IDbContextTransaction transaction;

        public UnitOfWork(ToDoContext context)
        {
            _context = context;
        }

        public void PocniTransakciju()
        {
            transaction = _context.Database.BeginTransaction();
        }

        public void Sacuvaj()
        {
            _context.SaveChanges();
        }

        public void ZavrsiTransakciju()
        {
            transaction.Commit();
        }
    }
}
