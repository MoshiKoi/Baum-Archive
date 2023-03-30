using Avalonia.ReactiveUI;
using ReactiveUI;

using Baum.AvaloniaApp.ViewModels;

namespace Baum.AvaloniaApp.Views;

public partial class LanguageForestView : ReactiveUserControl<LanguageForestViewModel>
{
    public LanguageForestView()
    {
        InitializeComponent();

        this.WhenActivated(async d => await ViewModel!.LoadAsync());
    }
}