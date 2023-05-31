using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Timers;
using Avalonia.Threading;

namespace AvaloniaTest.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Random _random = new();
    private readonly RunMode _runMode = RunMode.CorrectObservableCollection;
    private readonly Stack<Person> _stack = new();

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
                            FirstName = _random.Next(1000).ToString(),
                            LastName = _random.Next(1000).ToString()
                        });
                        break;
                    case RunMode.TimerTest:
                    {
                        if (_stack.TryPop(out var newPerson))
                            People.Add(newPerson);
                        break;
                    }
                }
            });

        _timer.Elapsed += (_, _) =>
        {
            switch (_runMode)
            {
                case RunMode.TimerTest:
                    _stack.Push(new Person
                    {
                        FirstName = _random.Next(1000).ToString(),
                        LastName = _random.Next(1000).ToString()
                    });
                    break;
                case RunMode.WrongObservableCollection:
                    People.Add(new Person
                    {
                        FirstName = _random.Next(1000).ToString(),
                        LastName = _random.Next(1000).ToString()
                    });
                    break;
                case RunMode.CorrectObservableCollection:
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        People.Add(new Person
                        {
                            FirstName = _random.Next(1000).ToString(),
                            LastName = _random.Next(1000).ToString()
                        });
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
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
#pragma warning restore CS8618

    #endregion

    private enum RunMode
    {
        Observable = 0,
        TimerTest = 1,
        WrongObservableCollection = 2,
        CorrectObservableCollection = 3
    }
}