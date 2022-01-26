using Benday.Presidents.Api.DataAccess;
using Benday.Presidents.Api.Models;

namespace Benday.Presidents.Api.Services
{
    public interface IPersonToPresidentAdapter
    {
        void Adapt(President fromValue, Person toValue);
        void Adapt(Person fromValue, President toValue);
    }
}