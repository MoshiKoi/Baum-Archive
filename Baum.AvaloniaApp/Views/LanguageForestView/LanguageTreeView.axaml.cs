using Avalonia.ReactiveUI;
using ReactiveUI;

using Baum.AvaloniaApp.ViewModels;

namespace Baum.AvaloniaApp.Views;

public partial class LanguageTreeView : ReactiveUserControl<LanguageTreeViewModel>
{
    public LanguageTreeView()
    {
        InitializeComponent();

        this.WhenActivated(async d => await ViewModel!.LoadAsync());
    }
}