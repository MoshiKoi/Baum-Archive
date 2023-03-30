using Avalonia.ReactiveUI;

using Baum.AvaloniaApp.ViewModels;

namespace Baum.AvaloniaApp.Views;

public partial class HomeView : ReactiveUserControl<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
    }
}