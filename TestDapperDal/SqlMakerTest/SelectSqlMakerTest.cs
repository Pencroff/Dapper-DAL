using Dapper_DAL.SqlMaker;
using NUnit.Framework;

namespace TestDapperDal.SqlMakerTest
{
    [TestFixture]
    public class SelectSqlMakerTest
    {
        private string _dbScheme;

        [SetUp]
        public void SetUp()
        {
            _dbScheme = "dbo"; // default MS SQl scheme
        }

        [Test]
        public void InitSelectDistinctTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SelectDistinct("Name, Description, Address");
            var sql = maker.GetRaw();
            var example = "SELECT DISTINCT\n\tName\n\t, Description\n\t, Address";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void SelectUnionTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                .UNION();
            var sql = maker.GetRaw();
            var example = "SELECT\nUNION\nSELECT";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);

            maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id")
                .UNION(IsALL: true)
                    .Col("Id");
            sql = maker.GetRaw();
            example = "SELECT\n\tId\nUNION ALL\nSELECT\n\tId";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void InitSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT("Name, t.Description, t.Address AS adr");
            var sql = maker.GetRaw();
            var example = "SELECT\n\tName\n\t, t.Description\n\t, t.Address AS adr";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase); 
        }

        [Test]
        public void AddColSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT("Id")
                    .Col("Name")
                    .Col("Description", "Desc")
                    .Col("t.Address", "Addr");
            var sql = maker.GetRaw();
            var example = "SELECT\n\tId\n\t, Name\n\t, Description AS Desc\n\t, t.Address AS Addr";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }
    }
}