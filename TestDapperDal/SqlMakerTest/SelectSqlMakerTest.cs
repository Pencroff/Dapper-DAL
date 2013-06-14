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

        [Test]
        public void FromSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT("Id")
                    .Col("Name")
                    .Col("Description", "Desc")
                    .Col("t.Address", "Addr")
                .FROM("table, dbo.table, [dbo].[Customer] cst");
            var sql = maker.GetRaw();
            var example = "SELECT\n\tId\n\t, Name\n\t, Description AS Desc\n\t, t.Address AS Addr\nFROM"
                        + "\n\ttable\n\t, dbo.table\n\t, [dbo].[Customer] cst";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void AddTableSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Table")
                    .Tab("Table", "Tab")
                    .Tab("Table", "Tab", "tbl");
            var sql = maker.GetRaw();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Table]\n\t, [dbo].[Table] AS [Tab]\n\t, [tbl].[Table] AS [Tab]";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void SimpleWhereSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .WHERE("Zip = @zip AND Id >= @id");
            var sql = maker.GetRaw();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nWHERE\n\tZip = @zip AND Id >= @id";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }
    }
}