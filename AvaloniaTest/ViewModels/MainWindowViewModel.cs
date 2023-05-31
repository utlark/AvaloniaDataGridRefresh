using System.Collections.Generic;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private List<Person> _people = new();

    public List<Person> People
    {
        get => _people;
        set => this.RaiseAndSetIfChanged(ref _people, value);
    }

    public MainWindowViewModel()
    {
        People = new List<Person>
        {
            new()
            {
                FirstName = "1",
                LastName = "2"
            },
            new()
            {
                FirstName = "3",
                LastName = "4"
            },
            new()
            {
                FirstName = "5",
                LastName = "6"
            },
        };
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}