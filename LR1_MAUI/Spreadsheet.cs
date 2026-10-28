using System.Globalization;
using System.Text.Json;

namespace LR1_MAUI
{
    public class Spreadsheet
    {
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public Dictionary<string, Cell> Cells { get; private set; } = new();

        public Spreadsheet(int rows, int cols)
        {
            RowCount = rows;
            ColumnCount = cols;
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            Cells.Clear();
            for (int r = 1; r <= RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    string address = GetColumnName(c) + r;
                    Cells.Add(address, new Cell(address));
                }
            }
        }

        public void RecalculateAll()
        {
            foreach (var cell in Cells.Values)
            {
                if (string.IsNullOrWhiteSpace(cell.Expression))
                {
                    cell.Value = "";
                    continue;
                }

                try
                {
                    cell.Value = Calculator.Evaluate(cell.Expression, Cells, new HashSet<string>()).ToString(CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    cell.Value = "Помилка: " + ex.Message;
                }
            }
        }

        public void Save(string path)
        {
            var expressions = new Dictionary<string, string>();
            foreach (var cell in Cells.Values)
            {
                expressions.Add(cell.Address, cell.Expression);
            }
            string json = JsonSerializer.Serialize(new { RowCount, ColumnCount, Cells = expressions });
            File.WriteAllText(path, json);
        }

        public void Load(string path)
        {
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            int rows = document.RootElement.GetProperty("RowCount").GetInt32();
            int cols = document.RootElement.GetProperty("ColumnCount").GetInt32();
            var expressions = document.RootElement.GetProperty("Cells");
            if (rows < 0 || cols < 0 || expressions.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException("Некоректний формат файла");
            }

            var loaded = new Spreadsheet(rows, cols);
            foreach (var cell in loaded.Cells.Values)
            {
                var expression = expressions.GetProperty(cell.Address).GetString();
                if (expression != null)
                {
                    cell.Expression = expression;
                }
                else
                {
                    throw new FormatException("Некоректний формат файла");
                }
            }
            RowCount = rows;
            ColumnCount = cols;
            Cells = loaded.Cells;
            RecalculateAll();
        }

        public static string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string value = "";
            if (index >= letters.Length) value += letters[index / letters.Length - 1];
            value += letters[index % letters.Length];
            return value;
        }
    }
}
