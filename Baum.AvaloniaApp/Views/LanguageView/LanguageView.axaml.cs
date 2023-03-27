using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

using Baum.AvaloniaApp.ViewModels;

namespace Baum.AvaloniaApp.Views;

public partial class LanguageView : ReactiveUserControl<LanguageViewModel>
{
    public LanguageView()
    {
        InitializeComponent();

        this.WhenActivated(async d => await ViewModel!.LoadAsync());
    }
}