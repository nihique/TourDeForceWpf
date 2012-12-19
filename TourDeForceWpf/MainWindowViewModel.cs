using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaBlade.Core;

namespace TourDeForceWpf
{
    public class MainWindowViewModel
    {
        public ObservableCollection<Employee> Employees { get; private set; }

        public MainWindowViewModel()
        {
            MainWindowViewModelAsync();
        }

        private async void MainWindowViewModelAsync()
        {
            Employees = new ObservableCollection<Employee>();
            var mgr = new NorthwindIBEntities();
            var query = mgr.Employee;
            var results = await mgr.ExecuteQueryAsync(query);
            results.ForEach(Employees.Add);
        }
    }
}
