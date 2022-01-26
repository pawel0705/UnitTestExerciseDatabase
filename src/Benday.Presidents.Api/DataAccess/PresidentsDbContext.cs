using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Benday.Presidents.Api.DataAccess
{
    public class PresidentsDbContext : DbContext
    {
        public PresidentsDbContext(DbContextOptions options):
            base(options)
        {

        }

        public DbSet<Person> Persons { get; set; }

        public DbSet<PersonFact> PersonFacts { get; set; }

        public override int SaveChanges()
        {
            CleanupOrphanedPersonFacts();

            return base.SaveChanges();
        }

        private void CleanupOrphanedPersonFacts()
        {
            var deleteThese = new List<PersonFact>();

            foreach (var deleteThis in PersonFacts.Local.Where(pf => pf.Person == null))
            {
                deleteThese.Add(deleteThis);
            }

            foreach (var deleteThis in deleteThese)
            {
                PersonFacts.Remove(deleteThis);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");
            });

            modelBuilder.Entity<PersonFact>().ToTable("PersonFact");
        }
    }
}
