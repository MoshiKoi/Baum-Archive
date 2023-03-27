using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Baum.AvaloniaApp.Services;
using Baum.AvaloniaApp.ViewModels;
using Baum.AvaloniaApp.Views;

namespace Baum.AvaloniaApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Dependency stuff

            ProjectDatabaseFactory databaseFactory = new();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(databaseFactory),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}