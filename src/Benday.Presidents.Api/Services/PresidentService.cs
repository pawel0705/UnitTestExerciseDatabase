using System;
using System.Linq;
using Benday.DataAccess;
using Benday.Presidents.Api.DataAccess;
using Benday.Presidents.Api.Models;

namespace Benday.Presidents.Api.Services
{
    public class PresidentService
    {
        private IRepository<Person> _Repository;
        private IPersonToPresidentAdapter _Adapter;

        public PresidentService(
            IRepository<Person> repository,
            IPersonToPresidentAdapter adapter
        )
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository", "Argument cannot be null.");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter", "Argument cannot be null.");
            }

            _Adapter = adapter;
            _Repository = repository;
        }

        public President GetPresidentById(int id)
        {
            var match = _Repository.GetById(id);

            if (match == null)
            {
                return null;
            }
            else
            {
                var toPresident = new President();

                _Adapter.Adapt(match, toPresident);

                return toPresident;
            }
        }

        public void Save(President saveThis)
        {
            if (saveThis == null)
            {
                throw new ArgumentNullException("saveThis", "Argument cannot be null.");
            }

            var allPersons = _Repository.GetAll();

            var match = (
                from temp in allPersons
                where temp.FirstName == saveThis.FirstName &&
                temp.LastName == saveThis.LastName &&
                temp.Facts.GetFactValueAsDateTime(PresidentsConstants.BirthDate) ==
                    saveThis.BirthDate
                select temp
            ).FirstOrDefault();

            if (match == null)
            {
                var toPerson = new Person();

                _Adapter.Adapt(saveThis, toPerson);

                _Repository.Save(toPerson);

                saveThis.Id = toPerson.Id;
            }
            else
            {
                throw new InvalidOperationException("Cannot save duplicate president.");
            }
        }
    }
}