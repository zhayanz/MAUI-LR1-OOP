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
        public MainPage()
        {
            InitializeComponent();
            CreateGrid();
        }
        //створення таблиці
        private void CreateGrid()
        {

            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
        }
        private void AddColumnsAndColumnLabels()
        {
            grid.RowDefinitions.Add(new RowDefinition());
            // Додати стовпці та підписи для стовпців
            for (int col = 0; col < CountColumn + 1; col++)
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
            for (int row = 0; row < CountRow; row++)
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
                for (int col = 0; col < CountColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",

                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center

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
            // Додайте додаткову логіку, яка виконується при виході зі зміненої клітинки
        }
        private void CalculateButton_Clicked(object? sender, EventArgs e)
        {
            // Обробка кнопки "Порахувати"
        }
        private void ModeButton_Clicked(object? sender, EventArgs e)
        {
            _isValueMode = !_isValueMode;
            modeButton.Text = _isValueMode ? "Режим: ЗНАЧЕННЯ" : "Режим: ВИРАЗ";
            modeMenuItem.Text = modeButton.Text;
        }
        private void SaveButton_Clicked(object? sender, EventArgs e)
        {
            // Обробка кнопки "Зберегти"
        }
        private void ReadButton_Clicked(object? sender, EventArgs e)
        {
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
                "Адреси приклад: A1 ; B12 ; AA1 (великі латинські!)\n" +
                "Назви функцій пишуться малими літерами\n" +
                "Числа вводяться цілими а для від'ємного значеня: (0-5)\n" +
                "Приклад: max(A1, 10) ^ 2 - inc(A2)", "Гаразд");
        }
        private void DeleteRowButton_Clicked(object? sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;
                grid.RowDefinitions.RemoveAt(lastRowIndex);
                grid.Children.RemoveAt(lastRowIndex * (CountColumn + 1)); // Remove label
                for (int col = 0; col < CountColumn; col++)
                {
                    grid.Children.RemoveAt((lastRowIndex * CountColumn) + col + 1); //Remove entry
                }
            }
        }
        private void DeleteColumnButton_Clicked(object? sender, EventArgs e)
        {
            if (grid.ColumnDefinitions.Count > 1)
            {
                int lastColumnIndex = grid.ColumnDefinitions.Count - 1;
                grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
                grid.Children.RemoveAt(lastColumnIndex); // Remove label
                for (int row = 0; row < CountRow; row++)
                {

                    grid.Children.RemoveAt(row * (CountColumn + 1) + lastColumnIndex +
                    1); // Remove entry
                }
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
            for (int col = 0; col < CountColumn; col++)
            {
                var entry = new Entry
                {
                    Text = "",

                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center

                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
            }
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
            for (int row = 0; row < CountRow; row++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center

                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, row + 1);

                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);
            }
        }
    }
}