using System;
using Dapper_DAL.SqlMaker;
using Dapper_DAL.SqlMaker.Interfaces;
using NUnit.Framework;

namespace TestDapperDal.SqlMakerTest
{
    [TestFixture]
    public class InitSqlMakerTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void InitSqlMaker()
        {
            var current = QueryMaker.Current;
            Assert.That(current, Is.InstanceOf<ISqlMaker>());
            var maker = QueryMaker.New();
            Assert.That(maker, Is.InstanceOf<ISqlMaker>());
            Assert.That(maker, Is.InstanceOf<ISqlFirst>());
            Assert.That(maker, Is.Not.EqualTo(current));
        }

        [Test]
        public void MakeEmptyQueryTest()
        {
            var newQuery = QueryMaker.Current;
            try
            {
                newQuery.RawSql();
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.InstanceOf<Exception>()
                        .With.Property("Message").EqualTo("Empty query"));
            }
        }
    }
}