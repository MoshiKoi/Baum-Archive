using System.Reactive;
using ReactiveUI;

namespace Baum.AvaloniaApp.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public string OpenButtonText { get; } = "Open File";

    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    public ReactiveCommand<Unit, Unit> NewCommand { get; }

    public HomeViewModel(ReactiveCommand<Unit, Unit> openCommand, ReactiveCommand<Unit, Unit> newCommand)
    {
        OpenCommand = openCommand;
        NewCommand = newCommand;
    }
}