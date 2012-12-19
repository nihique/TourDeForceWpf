using System.Windows;

namespace TourDeForceWpf
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = new MainWindowViewModel();
            DataContext = _vm;
            _vm.PropertyChanged += delegate { EnableDisableButtons(); };
            _vm.Employees.CollectionChanged += delegate { EnableDisableButtons(); };
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            _vm.MoveCurrentToFirst();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            _vm.MoveCurrentToPrevious();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _vm.MoveCurrentToNext();
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            _vm.MoveCurrentToLast();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            _vm.AddNew();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.CurrentPosition > -1) _vm.RemoveAt(_vm.CurrentPosition);
        }

        private void EnableDisableButtons()
        {
            First.IsEnabled = _vm.CurrentPosition > 0;
            Previous.IsEnabled = _vm.CurrentPosition > 0;
            Next.IsEnabled = _vm.CurrentPosition < _vm.Employees.Count - 1;
            Last.IsEnabled = _vm.CurrentPosition < _vm.Employees.Count - 1;
            Add.IsEnabled = _vm.Employees != null;
            Delete.IsEnabled = _vm.CurrentPosition > -1;
            Ok.IsEnabled = _vm.CanSave;
            Cancel.IsEnabled = _vm.CanSave;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            _vm.Save();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _vm.RollbackChanges();
        }

    }
}
