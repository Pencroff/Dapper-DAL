using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper_DAL.SqlMaker;
using NUnit.Framework;

namespace TestDapperDal.SqlMakerTest
{
    [TestFixture]
    class SqlMakerInsertTest
    {
        private string _dbScheme;

        [SetUp]
        public void SetUp()
        {
            _dbScheme = "dbo"; // default MS SQl scheme
        }

        [Test]
        public void InitTest()
        {
            var maker = QueryMaker.New(_dbScheme).INSERT("Customer");
            var sql = maker.GetRaw();
            var example = "INSERT INTO\n\t[dbo].[Customer]";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void AddColumnTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .INSERT("Customer")
                    .Col("Name")
                    .Col("Description")
                    .Col("Address");
            var sql = maker.GetRaw();
            var example = "INSERT INTO\n\t[dbo].[Customer] (\n\t\t[Name]\n\t\t, [Description]\n\t\t, [Address]\n\t)";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }
    }
}
