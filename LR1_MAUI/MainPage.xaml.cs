using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using Grid = Microsoft.Maui.Controls.Grid;

namespace LR1_MAUI
{
    public partial class MainPage : ContentPage
    {
        const int CountColumn = 8; // кількість стовпчиків (A to Z)
        const int CountRow = 10; // кількість рядків
        private bool _isValueMode = true;
        private Spreadsheet _spreadsheet = new Spreadsheet(CountRow, CountColumn);
        public MainPage()
        {
            InitializeComponent();
            CreateGrid();
        }
        //створення таблиці
        private void CreateGrid()
        {
            foreach (var view in grid.Children)
            {
                if (view is Entry entry)
                    entry.Unfocused -= Entry_Unfocused;
            }
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
            UpdateGridDisplay();
        }
        private void AddColumnsAndColumnLabels()
        {
            grid.RowDefinitions.Add(new RowDefinition());
            // Додати стовпці та підписи для стовпців
            for (int col = 0; col < _spreadsheet.ColumnCount + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                if (col > 0)
                {
                    var label = new Label
                    {
                        Text = GetColumnName(col),

                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center

                    };

                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);

                }
            }
        }
        private void AddRowsAndCellEntries()
        {
            // Додати рядки, підписи для рядків та комірки
            for (int row = 0; row < _spreadsheet.RowCount; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                // Додати підпис для номера рядка
                var label = new Label
                {
                    Text = (row + 1).ToString(),

                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center

                };
                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);
                // Додати комірки (Entry) для вмісту
                for (int col = 0; col < _spreadsheet.ColumnCount; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",

                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.Fill

                    };

                    entry.Focused += (s, args) =>
                    {
                        var address = GetColumnName(Grid.GetColumn(entry)) + Grid.GetRow(entry);
                        if (_spreadsheet.Cells.ContainsKey(address))
                            entry.Text = _spreadsheet.Cells[address].Expression;
                    };
                    entry.Unfocused += Entry_Unfocused; // обробник події Unfocused

                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);

                }
            }
        }
        private string GetColumnName(int colIndex)
        {
            int dividend = colIndex;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;

            }
            return columnName;
        }
        // викликається, коли користувач вийде зі зміненої клітинки (втратить фокус)
        private void Entry_Unfocused(object? sender, FocusEventArgs e)
        {
            // щоб програма не впала бонусом -попердження компілятора null
            if (sender is not Entry entry)
                return;

            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;
            var address = GetColumnName(col + 1) + (row + 1);
            if (_spreadsheet.Cells.ContainsKey(address) == false)
            {
                _spreadsheet.Cells.Add(address, new Cell(address));
            }

            if (content != null)
            {
                _spreadsheet.Cells[address].Expression = content;
            }
            else
            {
                _spreadsheet.Cells[address].Expression = "";
            }
            if (_isValueMode)
                entry.Text = _spreadsheet.Cells[address].Value;
            // Додайте додаткову логіку, яка виконується при виході зі зміненої клітинки
        }
        private void CalculateButton_Clicked(object? sender, EventArgs e)
        {
            foreach (var view in grid.Children)
            {
                if (view is Entry entry && entry.IsFocused)
                {
                    var address = GetColumnName(Grid.GetColumn(entry)) + Grid.GetRow(entry);
                    if (_spreadsheet.Cells.ContainsKey(address) == false)
                    {
                        _spreadsheet.Cells.Add(address, new Cell(address));
                    }
                    if (entry.Text != null)
                    {
                        _spreadsheet.Cells[address].Expression = entry.Text;
                    }
                    else
                    {
                        _spreadsheet.Cells[address].Expression = "";
                    }
                }
            }
            _spreadsheet.RecalculateAll();
            UpdateGridDisplay();
            // Обробка кнопки "Порахувати"
        }
        private void ModeButton_Clicked(object? sender, EventArgs e)
        {
            foreach (var view in grid.Children)
            {
                if (view is Entry entry && entry.IsFocused)
                {
                    var address = GetColumnName(Grid.GetColumn(entry)) + Grid.GetRow(entry);
                    if (_spreadsheet.Cells.ContainsKey(address) == false)
                    {
                        _spreadsheet.Cells.Add(address, new Cell(address));
                    }
                    if (entry.Text != null)
                    {
                        _spreadsheet.Cells[address].Expression = entry.Text;
                    }
                    else
                    {
                        _spreadsheet.Cells[address].Expression = "";
                    }
                }
            }
            if (_isValueMode)
            {
                _isValueMode = false;
            }
            else
            {
                _isValueMode = true;
            }
            modeButton.Text = _isValueMode ? "Режим: ЗНАЧЕННЯ" : "Режим: ВИРАЗ";
            modeMenuItem.Text = modeButton.Text;
            UpdateGridDisplay();
        }
        private void UpdateGridDisplay()
        {
            foreach (var view in grid.Children)
            {
                if (view is Entry entry && entry.IsFocused == false)
                {
                    var address = GetColumnName(Grid.GetColumn(entry)) + Grid.GetRow(entry);
                    if (_spreadsheet.Cells.ContainsKey(address))
                    {
                        var cell = _spreadsheet.Cells[address];
                        entry.Text = _isValueMode ? cell.Value : cell.Expression;
                    }
                }
            }
        }
        private async void SaveButton_Clicked(object? sender, EventArgs e)
        {
            try
            {
                CalculateButton_Clicked(sender, e);
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "table.json");
                _spreadsheet.Save(path);
                await DisplayAlertAsync("Збережено", "Файл збережено за шляхом: " + path, "Гаразд");
            }
            catch (Exception)
            {
                await DisplayAlertAsync("Помилка!", "Не вдалося зберегти таблицю", "Гаразд");
            }
            // Обробка кнопки "Зберегти"
        }
        private async void ReadButton_Clicked(object? sender, EventArgs e)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "table.json");
                if (File.Exists(path) == false)
                {
                    await DisplayAlertAsync("Увага!", "Файл " + path + " не знайдено", "Гаразд");
                    return;
                }
                _spreadsheet.Load(path);
                CreateGrid();
                await DisplayAlertAsync("Успіх", "Таблицю успішно завантажено", "Гаразд");
            }
            catch (Exception)
            {
                await DisplayAlertAsync("Помилка!", "Не вдалося прочитати таблицю, Перевірте файл!", "Гаразд");
            }
            // Обробка кнопки "Прочитати"
        }
        private async void ExitButton_Clicked(object? sender, EventArgs e)
        {
            bool answer = await DisplayAlertAsync("Підтвердження", "Ви дійсно хочете вийти?",
            "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }
        private async void HelpButton_Clicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Довідка",
                "Електронна таблиця (ЛР1) з ООП\n" +
                "Власов Влад Олександрович К-25\n" +
                "Варіант 11:\n" +
                "Операції: +  -  *  /  ^\n" +
                "Функції: inc(x) ; dec(x) ; max(x,y) ; min(x,y)\n\n" +
                "Додатково:\n" +
                "Адреси приклад: A1 ; B12 ; AA1\n" +
                "Назви функцій пишуться малими літерами\n" +
                "Дробові числа вводяться через крапку (9.9), через кому не працює! (9,9)\n" +
                "Для від'ємного значення: (0-5)\n" +
                "Приклад: max(A1, 10) ^ 2 - inc(A2)", "Гаразд");
        }
        private void DeleteRowButton_Clicked(object? sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;
                for (int i = grid.Children.Count - 1; i >= 0; i--)
                {
                    var view = (View)grid.Children[i];
                    if (Grid.GetRow(view) == lastRowIndex)
                    {
                        if (view is Entry entry)
                        {
                            entry.Unfocused -= Entry_Unfocused;
                            var address = GetColumnName(Grid.GetColumn(entry)) + lastRowIndex;
                            _spreadsheet.Cells.Remove(address);
                        }
                        grid.Children.RemoveAt(i);
                    }
                }
                grid.RowDefinitions.RemoveAt(lastRowIndex);
                _spreadsheet.RowCount--;
            }
        }
        private void DeleteColumnButton_Clicked(object? sender, EventArgs e)
        {
            if (grid.ColumnDefinitions.Count > 1)
            {
                int lastColumnIndex = grid.ColumnDefinitions.Count - 1;
                for (int i = grid.Children.Count - 1; i >= 0; i--)
                {
                    var view = (View)grid.Children[i];
                    if (Grid.GetColumn(view) == lastColumnIndex)
                    {
                        if (view is Entry entry)
                        {
                            entry.Unfocused -= Entry_Unfocused;
                            var address = GetColumnName(lastColumnIndex) + Grid.GetRow(entry);
                            _spreadsheet.Cells.Remove(address);
                        }
                        grid.Children.RemoveAt(i);
                    }
                }
                grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
                _spreadsheet.ColumnCount--;
            }
        }
        private void AddRowButton_Clicked(object? sender, EventArgs e)
        {
            int newRow = grid.RowDefinitions.Count;
            // Add a new row definition
            grid.RowDefinitions.Add(new RowDefinition());
            // Add label for the row number
            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);
            // Add entry cells for the new row
            for (int col = 0; col < grid.ColumnDefinitions.Count - 1; col++)
            {
                var cellAddress = GetColumnName(col + 1) + newRow;
                _spreadsheet.Cells.Add(cellAddress, new Cell(cellAddress));
                var entry = new Entry
                {
                    Text = "",

                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill

                };
                entry.Focused += (s, args) =>
                {
                    var address = GetColumnName(Grid.GetColumn(entry)) + Grid.GetRow(entry);
                    if (_spreadsheet.Cells.ContainsKey(address))
                        entry.Text = _spreadsheet.Cells[address].Expression;
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
            }
            _spreadsheet.RowCount++;
        }
        private void AddColumnButton_Clicked(object? sender, EventArgs e)
        {
            int newColumn = grid.ColumnDefinitions.Count;
            // Add a new column definition
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            // Add label for the column name
            var label = new Label
            {
                Text = GetColumnName(newColumn),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, newColumn);
            grid.Children.Add(label);
            // Add entry cells for the new column
            for (int row = 0; row < grid.RowDefinitions.Count - 1; row++)
            {
                var cellAddress = GetColumnName(newColumn) + (row + 1);
                _spreadsheet.Cells.Add(cellAddress, new Cell(cellAddress));
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill

                };
                entry.Focused += (s, args) =>
                {
                    var address = GetColumnName(Grid.GetColumn(entry)) + Grid.GetRow(entry);
                    if (_spreadsheet.Cells.ContainsKey(address))
                        entry.Text = _spreadsheet.Cells[address].Expression;
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, row + 1);

                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);
            }
            _spreadsheet.ColumnCount++;
        }
    }
}