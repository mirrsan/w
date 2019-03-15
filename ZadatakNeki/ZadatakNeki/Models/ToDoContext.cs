using Microsoft.EntityFrameworkCore;

namespace ZadatakNeki.Models
{
    public class ToDoContext : DbContext
    {
        
        public ToDoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Osoba> Osobe { get; set; }
        public DbSet<Uredjaj> Uredjaji { get; set; }
        public DbSet<Kancelarija> Kancelarije { get; set; }
        public DbSet<OsobaUredjaj> OsobaUredjaj { get; set; }
    }
}
