using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using TourDeForceWpf.Annotations;

namespace TourDeForceWpf
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Employee _currentEmployee;
        private int _currentPosition;
        private NorthwindIBEntities _entities;
        private bool _canSave;
        private EntityQuery<Employee> _query;

        public MainWindowViewModel()
        {
            MainWindowViewModelAsync();
        }

        public ObservableCollection<Employee> Employees { get; private set; }

        public Employee CurrentEmployee
        {
            get { return _currentEmployee; }
            set
            {
                _currentEmployee = value;
                OnPropertyChanged("CurrentEmployee");
            }
        }

        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                _canSave = value;
                OnPropertyChanged("CanSave");
            }
        }

        public int CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                if (Employees != null && Employees.Count > 0)
                {
                    _currentPosition = Math.Min(Math.Max(value, 0), Employees.Count - 1);
                    CurrentEmployee = Employees[_currentPosition];
                }
                _currentPosition = value;
                OnPropertyChanged("CurrentPosition");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private async void MainWindowViewModelAsync()
        {
            Employees = new ObservableCollection<Employee>();
            _entities = new NorthwindIBEntities();
            _query = _entities.Employee;
            IEnumerable<Employee> results = await _entities.ExecuteQueryAsync(_query);
            results.ForEach(Employees.Add);
            MoveCurrentToFirst();
            _entities.EntityChanged += (sender, args) =>
                {
                    CanSave = ((NorthwindIBEntities) sender).HasChanges();
                };
            Employees.CollectionChanged += Employees_CollectionChanged;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Employees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action) 
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Employee>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Employee>());
                    break;
            }
        }

        public void RollbackChanges()
        {
            int privateCurrentPosition = CurrentPosition;
            _entities.RejectChanges();
            Employees.Clear();
            _query.With(QueryStrategy.CacheOnly).Execute().ForEach(Employees.Add);
            CurrentPosition = privateCurrentPosition;
        }

        private void Remove(IEnumerable<Employee> employees)
        {
            employees.ForEach(x => x.EntityAspect.Delete());
            Save();
        }

        private void Add(IEnumerable<Employee> employees)
        {
            _entities.AddEntities(employees);
        }

        public void MoveCurrentToFirst()
        {
            CurrentPosition = 0;
        }

        public void MoveCurrentToPrevious()
        {
            CurrentPosition -= 1;
        }

        public void MoveCurrentToNext()
        {
            CurrentPosition += 1;
        }

        public void MoveCurrentToLast()
        {
            CurrentPosition = Employees != null ? Employees.Count - 1 : -1;
        }

        public void AddNew()
        {
            Employees.Add(new Employee());
            MoveCurrentToLast();
        }

        public void RemoveAt(int position)
        {
            Employees.RemoveAt(position);
            CurrentPosition = position;
        }

        public void Save()
        {
            _entities.SaveChanges();
        }
    }
}