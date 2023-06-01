using Avalonia.Controls;
using AvaloniaTest.ViewModels;
using JetBrains.Annotations;

namespace AvaloniaTest.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MyDataGrid.BeginningEdit += (_, e) => _original = ((MainWindowViewModel.Person)e.Column.GetCellContent(e.Row).DataContext)?.Clone();

            MyDataGrid.CellEditEnded += (_, e) => e.Column.GetCellContent(e.Row).DataContext = _original;
        }

        [CanBeNull] private MainWindowViewModel.Person _original;
    }
}