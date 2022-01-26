using Benday.Presidents.Common;
using Benday.Presidents.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Benday.DataAccess;
using Benday.DataAccess.SqlServer;

namespace Benday.Presidents.Api.DataAccess.SqlServer
{
    public class SqlEntityFrameworkPersonRepository :
        SqlEntityFrameworkCrudRepositoryBase<Person, PresidentsDbContext>
    {
        public SqlEntityFrameworkPersonRepository(
            PresidentsDbContext context) : base(context)
        {

        }

        protected override DbSet<Person> EntityDbSet
        {
            get
            {
                return Context.Persons;
            }
        }

        public override IList<Person> GetAll()
        {
            return (
                from temp in EntityDbSet
                    .Include(p => p.Facts)
                orderby temp.LastName, temp.FirstName
                select temp
                ).ToList();
        }

        public override Person GetById(int id)
        {
            return (
                from temp in EntityDbSet
                    .Include(p => p.Facts)
                where temp.Id == id
                select temp
                ).FirstOrDefault();
        }
    }
}