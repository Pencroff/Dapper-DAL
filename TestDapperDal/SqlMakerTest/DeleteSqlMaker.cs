using Dapper_DAL.SqlMaker;
using NUnit.Framework;

namespace TestDapperDal.SqlMakerTest
{
    [TestFixture]
    public class DeleteSqlMaker
    {
        [SetUp]
        public void SetUp()
        {
            QueryMaker.DbScheme = "dbo";
        }
        
        [Test]
        public void InitDeleteTest()
        {
            var maker = QueryMaker.New()
                .DELETE("Customer");
            var sql = maker.RawSql();
            var example = "DELETE FROM [dbo].[Customer];";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereDeleteTest()
        {
            var maker = QueryMaker.New()
                .DELETE("Customer")
                .WHERE("Zip = @zip AND Id >= @id");
            var sql = maker.RawSql();
            var example = "DELETE FROM [dbo].[Customer]"
                        + "\nWHERE Zip = @zip AND Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereAndDeleteTest()
        {
            var maker = QueryMaker.New()
                .DELETE("Customer")
                .WHERE("Zip = @zip")
                .WhereAnd("Id >= @id");
            var sql = maker.RawSql();
            var example = "DELETE FROM [dbo].[Customer]"
                        + "\nWHERE Zip = @zip\n\tAND Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereOrDeleteTest()
        {
            var maker = QueryMaker.New()
                .DELETE("Customer")
                .WHERE("Zip = @zip")
                .WhereOr("Id >= @id");
            var sql = maker.RawSql();
            var example = "DELETE FROM [dbo].[Customer]"
                        + "\nWHERE Zip = @zip\n\tOR Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }
    }
}