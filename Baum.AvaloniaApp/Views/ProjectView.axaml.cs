using Avalonia.ReactiveUI;

using Baum.AvaloniaApp.ViewModels;

namespace Baum.AvaloniaApp.Views;

public partial class ProjectView : ReactiveUserControl<ProjectViewModel>
{
    public ProjectView()
    {
        InitializeComponent();
    }
}