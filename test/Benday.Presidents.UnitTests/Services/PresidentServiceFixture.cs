using System;
using Benday.DataAccess;
using Benday.Presidents.Api.DataAccess;
using Benday.Presidents.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Benday.Presidents.UnitTests.Services
{
    [TestClass]
    public class PresidentServiceFixture
    {
        [TestInitialize]
        public void OnTestInitialize()
        {
            _SystemUnderTest = null;
            _RepositoryInstance = null;
            _AdapterInstance = null;
        }

        private PresidentService _SystemUnderTest;
        public PresidentService SystemUnderTest
        {
            get
            {
                if (_SystemUnderTest == null)
                {
                    _SystemUnderTest = new PresidentService(
                        RepositoryInstance, AdapterInstance
                    );
                }

                return _SystemUnderTest;
            }
        }

        private IRepository<Person> _RepositoryInstance;

        public IRepository<Person> RepositoryInstance
        {
            get
            {
                if (_RepositoryInstance == null)
                {
                    _RepositoryInstance = new InMemoryRepository<Person>();
                }

                return _RepositoryInstance;
            }
        }

        private IPersonToPresidentAdapter _AdapterInstance;

        public IPersonToPresidentAdapter AdapterInstance
        {
            get
            {
                if (_AdapterInstance == null)
                {
                    _AdapterInstance = new PersonToPresidentAdapter();
                }

                return _AdapterInstance;
            }
        }

        [TestMethod]
        public void PresidentServiceThrowsExceptionOnDuplicatePresident()
        {
            // arrange
            var person1 = UnitTestUtility.GetThomasJeffersonAsPerson();

            RepositoryInstance.Save(person1);
            Assert.AreNotEqual<int>(0, person1.Id, "Person wasn't saved.");

            var duplicatePresident = UnitTestUtility.GetThomasJeffersonAsPresident();

            // act

            bool gotException = false;

            try
            {
                SystemUnderTest.Save(duplicatePresident);
            }
            catch (InvalidOperationException ex)
            {
                gotException = true;

                // assert
                Assert.AreEqual<string>("Cannot save duplicate president.", ex.Message,
                    "Exception message was wrong.");
            }

            Assert.IsTrue(gotException, "Didn't get exception.");
        }

        [TestMethod]
        public void PresidentServiceSavesUniquePresident()
        {
            // arrange
            var person1 = UnitTestUtility.GetThomasJeffersonAsPerson();

            RepositoryInstance.Save(person1);
            Assert.AreNotEqual<int>(0, person1.Id, "Person wasn't saved.");

            var uniquePresident = UnitTestUtility.GetGroverClevelandAsPresident();

            // act

            SystemUnderTest.Save(uniquePresident);

            // assert

            Assert.AreNotEqual<int>(0, uniquePresident.Id, "President wasn't saved.");

            var fromRepository = RepositoryInstance.GetById(
                uniquePresident.Id);

            Assert.IsNotNull(fromRepository, "Could not reload from repository.");
        }


    }
}
