using Dapper_DAL.SqlMaker;
using NUnit.Framework;

namespace TestDapperDal.SqlMakerTest
{
    [TestFixture]
    public class UpdateSqlMakerTest
    {
        [SetUp]
        public void SetUp()
        {
            QueryMaker.DbScheme = "dbo";
        }

        [Test]
        public void InitUpdateTest()
        {
            var maker = QueryMaker.New()
                .UPDATE("Customer");
            var sql = maker.RawSql();
            var example = "UPDATE [dbo].[Customer];";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void SetUpdateTest()
        {
            var maker = QueryMaker.New()
                .UPDATE("Customer")
                .SET("Id = @id, Name = @name");
            var sql = maker.RawSql();
            var example = "UPDATE [dbo].[Customer]\nSET\n\tId = @id\n\t, Name = @name;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void ValueSetUpdateTest()
        {
            var maker = QueryMaker.New()
                .UPDATE("Customer")
                .SET()
                    .Val("Id", "id")
                    .Val("Name", "name");
            var sql = maker.RawSql();
            var example = "UPDATE [dbo].[Customer]\nSET\n\tId = @id\n\t, Name = @name;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereUpdateTest()
        {
            var maker = QueryMaker.New()
                .UPDATE("Customer")
                .SET("Id = @id")
                    .Val("Name", "name")
                .WHERE("Zip = @zip AND Id >= @id");
            var sql = maker.RawSql();
            var example = "UPDATE [dbo].[Customer]\nSET\n\tId = @id\n\t, Name = @name"
                        + "\nWHERE Zip = @zip AND Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereAndUpdateTest()
        {
            var maker = QueryMaker.New()
                .UPDATE("Customer")
                .SET("Id = @id")
                    .Val("Name", "name")
                .WHERE("Zip = @zip")
                .WhereAnd("Id >= @id");
            var sql = maker.RawSql();
            var example = "UPDATE [dbo].[Customer]\nSET\n\tId = @id\n\t, Name = @name"
                        + "\nWHERE Zip = @zip\n\tAND Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereOrUpdateTest()
        {
            var maker = QueryMaker.New()
                .UPDATE("Customer")
                .SET("Id = @id")
                    .Val("Name", "name")
                .WHERE("Zip = @zip")
                .WhereOr("Id >= @id");
            var sql = maker.RawSql();
            var example = "UPDATE [dbo].[Customer]\nSET\n\tId = @id\n\t, Name = @name"
                        + "\nWHERE Zip = @zip\n\tOR Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        } 
    }
}