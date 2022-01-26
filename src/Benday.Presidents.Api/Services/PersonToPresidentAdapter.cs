using System;
using System.Collections.Generic;
using System.Linq;
using Benday.Presidents.Api.Models;
using Benday.Presidents.Api.DataAccess;

namespace Benday.Presidents.Api.Services
{
    public class PersonToPresidentAdapter : IPersonToPresidentAdapter
    {
        public void Adapt(President fromValue, Person toValue)
        {
            if (fromValue == null)
            {
                throw new ArgumentNullException("fromValue", "Argument cannot be null.");
            }

            if (toValue == null)
            {
                throw new ArgumentNullException("toValue", "Argument cannot be null.");
            }

            toValue.Id = fromValue.Id;
            toValue.FirstName = fromValue.FirstName;
            toValue.LastName = fromValue.LastName;

            if (fromValue.Id == 0)
            {
                toValue.Facts.Clear();
            }

            AdaptPresidentInfoToFacts(fromValue, toValue);

            AdaptPresidentTermsToFacts(fromValue, toValue);
        }

        private void AdaptPresidentInfoToFacts(President fromValue, Person toValue)
        {
            AdaptValueToPersonFact(fromValue.ImageFilename,
                            toValue,
                            PresidentsConstants.ImageFilename);

            AdaptValueToPersonFact(fromValue.BirthCity,
                toValue,
                PresidentsConstants.BirthCity);

            AdaptValueToPersonFact(fromValue.BirthDate,
                toValue,
                PresidentsConstants.BirthDate);

            AdaptValueToPersonFact(fromValue.BirthState,
                toValue,
                PresidentsConstants.BirthState);

            AdaptValueToPersonFact(fromValue.DeathCity,
                toValue,
                PresidentsConstants.DeathCity);

            AdaptValueToPersonFact(fromValue.DeathDate,
                toValue,
                PresidentsConstants.DeathDate);

            AdaptValueToPersonFact(fromValue.DeathState,
                toValue,
                PresidentsConstants.DeathState);
        }

        private void AdaptPresidentTermsToFacts(President fromValue, Person toValue)
        {
            foreach (var fromTerm in fromValue.Terms)
            {
                if (fromTerm.IsDeleted == false)
                {
                    toValue.AddFact(fromTerm.Id,
                        fromTerm.Role,
                        fromTerm.Number.ToString(),
                        fromTerm.Start,
                        fromTerm.End
                        );
                }
                else if (fromTerm.IsDeleted == true && fromTerm.Id > 0)
                {
                    toValue.RemoveFact(fromTerm.Id);
                }
            }
        }

        public void AdaptValueToPersonFact(string fromValue,
        Person toPerson,
            string toPersonFactType)
        {
            toPerson.AddFact(toPersonFactType, fromValue);
        }

        public void AdaptValueToPersonFact(DateTime fromValue,
            Person toPerson,
            string toPersonFactType)
        {
            toPerson.AddFact(toPersonFactType, fromValue);
        }

        public void Adapt(Person fromValue, President toValue)
        {
            if (fromValue == null)
            {
                throw new ArgumentNullException("fromValue", "Argument cannot be null.");
            }

            if (toValue == null)
            {
                throw new ArgumentNullException("toValue", "Argument cannot be null.");
            }

            toValue.Id = fromValue.Id;
            toValue.FirstName = fromValue.FirstName;
            toValue.LastName = fromValue.LastName;

            AdaptFactsToTerms(fromValue, toValue);

            toValue.ImageFilename = fromValue.Facts.GetFactValueAsString(
                PresidentsConstants.ImageFilename);

            toValue.BirthCity = fromValue.Facts.GetFactValueAsString(
                PresidentsConstants.BirthCity);

            toValue.BirthDate = fromValue.Facts.GetFactValueAsDateTime(
                PresidentsConstants.BirthDate);

            toValue.BirthState = fromValue.Facts.GetFactValueAsString(
                PresidentsConstants.BirthState);

            toValue.DeathCity = fromValue.Facts.GetFactValueAsString(
                PresidentsConstants.DeathCity);

            toValue.DeathDate = fromValue.Facts.GetFactValueAsDateTime(
                PresidentsConstants.DeathDate);

            toValue.DeathState = fromValue.Facts.GetFactValueAsString(
                PresidentsConstants.DeathState);
        }

        private void AdaptFactsToTerms(Person fromValue, President toValue)
        {
            foreach (PersonFact fromFact in
                fromValue.Facts.GetFacts(PresidentsConstants.President))
            {
                var toTerm = new Term();

                AdaptFactToTerm(fromFact, toTerm);

                toValue.Terms.Add(toTerm);
            }
        }

        private void AdaptFactToTerm(PersonFact fromValue, Term toValue)
        {
            toValue.Id = fromValue.Id;

            toValue.Role = fromValue.FactType;
            toValue.Start = fromValue.StartDate;
            toValue.End = fromValue.EndDate;

            toValue.Number = SafeToInt32(fromValue.FactValue, -1);
        }

        private int SafeToInt32(string valueAsString, int defaultInt32Value)
        {
            int temp;

            if (Int32.TryParse(valueAsString, out temp) == false)
            {
                return defaultInt32Value;
            }
            else
            {
                return temp;
            }
        }
    }
}
