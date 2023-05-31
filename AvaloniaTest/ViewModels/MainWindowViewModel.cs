using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Timers;
using Avalonia.Threading;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Random _random = new();

    private readonly Stack<Person> _stack = new();
    private ObservableCollection<Person> _people = new();

    public MainWindowViewModel()
    {
        var mode = RunMode.CorrectObservableCollection;

        Observable.Interval(TimeSpan.FromSeconds(1))
            .ObserveOn(AvaloniaScheduler.Instance)
            .Subscribe(_ =>
            {
                switch (mode)
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

        var timer = new Timer();
        timer.Interval = 500;
        timer.AutoReset = true;
        timer.Elapsed += (_, _) =>
        {
            switch (mode)
            {
                case RunMode.TimerTest:
                    _stack.Push(new Person
                    {
                        FirstName = _random.Next(1000).ToString(),
                        LastName = _random.Next(1000).ToString()
                    });
                    break;
                case RunMode.VanillaObservableCollection:
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
        timer.Start();
    }

    public ObservableCollection<Person> People
    {
        get => _people;
        set => this.RaiseAndSetIfChanged(ref _people, value);
    }

    private enum RunMode
    {
        Observable = 0,
        TimerTest = 1,
        VanillaObservableCollection = 2,
        CorrectObservableCollection = 3
    }

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
}