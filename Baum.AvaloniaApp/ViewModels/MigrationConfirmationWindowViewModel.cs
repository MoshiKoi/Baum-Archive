using System.Reactive;
using ReactiveUI;

namespace Baum.AvaloniaApp.ViewModels;

public class MigrationConfirmationWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, bool> ConfirmCommand { get; }
    public ReactiveCommand<Unit, bool> RejectCommand { get; }

    public MigrationConfirmationWindowViewModel()
    {
        ConfirmCommand = ReactiveCommand.Create(() => true);
        RejectCommand = ReactiveCommand.Create(() => false);
    }
}