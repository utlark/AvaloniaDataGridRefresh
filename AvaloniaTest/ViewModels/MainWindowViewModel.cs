using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using Avalonia.Threading;

namespace AvaloniaTest.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Random _random = new();
    private readonly RunMode _runMode = RunMode.ObservableCollectionPropertyChange;

    private readonly Timer _timer = new()
    {
        Interval = 500,
        AutoReset = true
    };

    public MainWindowViewModel()
    {
        Observable.Interval(TimeSpan.FromSeconds(1))
            .ObserveOn(AvaloniaScheduler.Instance)
            .Subscribe(_ =>
            {
                switch (_runMode)
                {
                    case RunMode.Observable:
                        People.Add(new Person
                        {
                            FirstName = _random.Next(1000),
                            LastName = _random.Next(1000).ToString()
                        });
                        break;
                }
            });

        _timer.Elapsed += (_, _) =>
        {
            switch (_runMode)
            {
                case RunMode.WrongObservableCollection:
                    People.Add(new Person
                    {
                        FirstName = _random.Next(1000),
                        LastName = _random.Next(1000).ToString()
                    });
                    break;
                case RunMode.CorrectObservableCollection:
                case RunMode.ObservableCollectionPropertyChange:
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        People.Add(new Person
                        {
                            FirstName = _random.Next(1000),
                            LastName = _random.Next(1000).ToString()
                        });
                    }, DispatcherPriority.Background);
                    break;
            }
        };
        _timer.Elapsed += (_, _) =>
        {
            switch (_runMode)
            {
                case RunMode.ObservableCollectionPropertyChange:
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var temp = People[0];
                        temp.FirstName = People.Count;
                    }, DispatcherPriority.Background);
                    break;
            }
        };
        _timer.Start();
    }

    public ObservableCollection<Person> People { get; } = new();

    #region PersonClass

#pragma warning disable CS8618
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class Person : INotifyPropertyChanged
    {
        private int _firstName;
        private string _lastName;

        public int FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = "") => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
#pragma warning restore CS8618

    #endregion

    private enum RunMode
    {
        Observable = 0,
        WrongObservableCollection = 1,
        CorrectObservableCollection = 2,
        ObservableCollectionPropertyChange = 3
    }
}