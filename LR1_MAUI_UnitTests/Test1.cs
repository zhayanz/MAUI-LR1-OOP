using LR1_MAUI;

namespace LR1_MAUI_UnitTests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        [DataRow("2+3", 5.0)]
        [DataRow("10-4", 6.0)]
        [DataRow("3*4", 12.0)]
        [DataRow("5/2", 2.5)]
        [DataRow("2^3", 8.0)]
        [DataRow("inc(5)", 6.0)]
        [DataRow("dec(0)", -1.0)]
        [DataRow("max(3,8)", 8.0)]
        [DataRow("min(3,8)", 3.0)]
        public void TestOperations(string expression, double expected)
        {
            var table = new Spreadsheet(1, 1);
            double result = Calculator.Evaluate(expression, table.Cells, new HashSet<string>());
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("2+3*4", 14.0)]
        [DataRow("(2+3)*4", 20.0)]
        [DataRow("max(5,min(8,3))+2*3^2", 23.0)]
        public void TestPriority(string expression, double expected)
        {
            var table = new Spreadsheet(1, 1);
            double result = Calculator.Evaluate(expression, table.Cells, new HashSet<string>());
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("A1+3", 10.0)]
        [DataRow("B1*2", 20.0)]
        [DataRow("A1+A1", 14.0)]
        [DataRow("AA10+1", 6.0)]
        public void TestCalculateAndReferences(string expression, double expected)
        {
            var table = new Spreadsheet(10, 27);
            table.Cells["A1"].Expression = "7";
            table.Cells["B1"].Expression = "A1+3";
            table.Cells["AA10"].Expression = "5";

            double result = Calculator.Evaluate(expression, table.Cells, new HashSet<string>());
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("2+*3", 3)]
        [DataRow("max(1)", 6)]
        [DataRow("(2+3", 5)]
        [DataRow("2@3", 2)]
        public void TestSyntaxError(string expression, int position)
        {
            var table = new Spreadsheet(1, 1);
            string message = "Помилка виразу в рядку 1, позиція " + position;
            try
            {
                Calculator.Evaluate(expression, table.Cells, new HashSet<string>());
                Assert.Fail("Помилка виразу не виникла");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(message, ex.Message);
            }

        }

        [TestMethod]
        [DataRow("A1", "")]
        [DataRow("B1", "A1")]
        public void TestCycle(string expression, string otherExpression)
        {
            var table = new Spreadsheet(1, 2);
            table.Cells["A1"].Expression = expression;
            table.Cells["B1"].Expression = otherExpression;
            table.RecalculateAll();

            Assert.AreEqual("Помилка: Циклічне посилання", table.Cells["A1"].Value);
            if (otherExpression != "")
            {
                Assert.AreEqual("Помилка: Циклічне посилання", table.Cells["B1"].Value);
            }
        }

        [TestMethod]
        public void TestSaveAndLoad()
        {
            string path = Path.GetTempFileName();
            try
            {
                var table = new Spreadsheet(2, 3);
                table.Cells["A1"].Expression = "7";
                table.Cells["C2"].Expression = "A1+3";
                table.Save(path);

                var loaded = new Spreadsheet(1, 1);
                loaded.Load(path);
                Assert.AreEqual(2, loaded.RowCount);
                Assert.AreEqual(3, loaded.ColumnCount);
                Assert.AreEqual("7", loaded.Cells["A1"].Expression);
                Assert.AreEqual("A1+3", loaded.Cells["C2"].Expression);
                Assert.AreEqual("10", loaded.Cells["C2"].Value);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void TestEmptyCell()
        {
            var table = new Spreadsheet(1, 1);
            table.RecalculateAll();
            Assert.AreEqual("", table.Cells["A1"].Value);
        }

        [TestMethod]
        public void TestLoadInvalidFile()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "{");
            var table = new Spreadsheet(1, 1);
            bool error = false;
            try
            {
                table.Load(path);
            }
            catch (Exception)
            {
                error = true;
            }
            finally
            {
                File.Delete(path);
            }
            Assert.IsTrue(error);
        }
    }
}
