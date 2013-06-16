using Dapper_DAL.Infrastructure.Enum;
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
            var sql = maker.RawSql();
            var example = "SELECT DISTINCT\n\tName\n\t, Description\n\t, Address;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void SelectUnionTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                .UNION();
            var sql = maker.RawSql();
            var example = "SELECT\nUNION\nSELECT;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);

            maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id")
                .UNION(IsALL: true)
                    .Col("Id");
            sql = maker.RawSql();
            example = "SELECT\n\tId\nUNION ALL\nSELECT\n\tId;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void InitSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT("Name, t.Description, t.Address AS adr");
            var sql = maker.RawSql();
            var example = "SELECT\n\tName\n\t, t.Description\n\t, t.Address AS adr;";
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
            var sql = maker.RawSql();
            var example = "SELECT\n\tId\n\t, Name\n\t, Description AS Desc\n\t, t.Address AS Addr;";
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
            var sql = maker.RawSql();
            var example = "SELECT\n\tId\n\t, Name\n\t, Description AS Desc\n\t, t.Address AS Addr\nFROM"
                        + "\n\ttable\n\t, dbo.table\n\t, [dbo].[Customer] cst;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void EmptySchemeSelectTest()
        {
            var maker = QueryMaker.New()
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[Customer];";
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
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Table]\n\t, [dbo].[Table] AS Tab\n\t, [tbl].[Table] AS Tab;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .WHERE("Zip = @zip AND Id >= @id");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nWHERE\n\tZip = @zip AND Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereAndSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .WHERE("Zip = @zip")
                .WhereAnd("Id >= @id");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nWHERE\n\tZip = @zip\n\tAND Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void WhereOrSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .WHERE("Zip = @zip")
                .WhereOr("Id >= @id");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nWHERE\n\tZip = @zip\n\tOR Id >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void JoinSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .JOIN("Address", "addr");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nINNER JOIN [dbo].[Address] AS addr;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void LeftJoinSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .LeftJoin("Address", "addr");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nLEFT JOIN [dbo].[Address] AS addr;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void RightJoinSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .RightJoin("Address", "addr");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nRIGHT JOIN [dbo].[Address] AS addr;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void FullJoinSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .FullJoin("Address", "addr");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nFULL JOIN [dbo].[Address] AS addr;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void OnJoinSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer", "cst")
                .JOIN("Address", "addr")
                    .ON("cst.Id = addr.Id");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer] AS cst"
                        + "\nINNER JOIN [dbo].[Address] AS addr"
                        + "\n\tON cst.Id = addr.Id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void OnAndJoinSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer", "cst")
                .JOIN("Address", "addr")
                    .ON("cst.Id = addr.Id")
                    .OnAnd("cst.Col = addr.Col");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer] AS cst"
                        + "\nINNER JOIN [dbo].[Address] AS addr"
                        + "\n\tON cst.Id = addr.Id"
                        + "\n\tAND cst.Col = addr.Col;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void OnOrJoinSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer", "cst")
                .JOIN("Address", "addr")
                    .ON("cst.Id = addr.Id")
                    .OnOr("cst.Col = addr.Col");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer] AS cst"
                        + "\nINNER JOIN [dbo].[Address] AS addr"
                        + "\n\tON cst.Id = addr.Id"
                        + "\n\tOR cst.Col = addr.Col;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void OrderBySelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer", "cst")
                .ORDERBY("Id", SortAs.Desc);
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                          + "\n\t[dbo].[Customer] AS cst"
                          + "\nORDER BY Id DESC;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void OrderThenSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer", "cst")
                .ORDERBY("Id", SortAs.Desc)
                .OrderThen("Zip", SortAs.Asc);
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                          + "\n\t[dbo].[Customer] AS cst"
                          + "\nORDER BY Id DESC, Zip ASC;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void GroupBySelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer", "cst")
                .GROUPBY("Id");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                          + "\n\t[dbo].[Customer] AS cst"
                          + "\nGROUP BY Id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void GroupThenSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer", "cst")
                .GROUPBY("Id")
                .GroupThen("Zip");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                          + "\n\t[dbo].[Customer] AS cst"
                          + "\nGROUP BY Id, Zip;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void HavingSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .GROUPBY("Zip")
                .HAVING("COUNT(Id) >= @id");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nGROUP BY Zip"
                        + "\nHAVING COUNT(Id) >= @id;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void HavingAndSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .GROUPBY("Zip")
                .HAVING("COUNT(Id) >= @id")
                .HavingAnd("MIN(Zip) = @zip");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nGROUP BY Zip"
                        + "\nHAVING COUNT(Id) >= @id\n\tAND MIN(Zip) = @zip;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }

        [Test]
        public void HavingOrSelectTest()
        {
            var maker = QueryMaker.New(_dbScheme)
                .SELECT()
                    .Col("Id", "Id")
                .FROM()
                    .Tab("Customer")
                .GROUPBY("Zip")
                .HAVING("COUNT(Id) >= @id")
                .HavingOr("MIN(Zip) = @zip");
            var sql = maker.RawSql();
            var example = "SELECT\n\tId AS Id\nFROM"
                        + "\n\t[dbo].[Customer]\nGROUP BY Zip"
                        + "\nHAVING COUNT(Id) >= @id\n\tOR MIN(Zip) = @zip;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
        }
    }
}