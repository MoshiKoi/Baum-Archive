using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;

using Baum.AvaloniaApp.ViewModels;

namespace Baum.AvaloniaApp.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(ViewModel!.RequestFileInteraction.RegisterHandler(ShowOpenFileDialog));
            d(ViewModel.RequestSaveFileInteraction.RegisterHandler(ShowSaveFileDialog));
            d(ViewModel.RequestTemporaryFileInteraction.RegisterHandler(GetTemporaryFile));
            d(ViewModel.ConfirmMigrationInteraction.RegisterHandler(ShowMigrationConfirmationDialog));
        });
    }

    async Task ShowMigrationConfirmationDialog(InteractionContext<Unit, bool> interaction)
    {
        var dialog = new MigrationConfirmationWindow();
        dialog.DataContext = new MigrationConfirmationWindowViewModel();

        var result = await dialog.ShowDialog<bool?>(this);
        interaction.SetOutput(result ?? false);
    }

    public void GetTemporaryFile(InteractionContext<Unit, FileInfo> interaction)
    {
        // TODO: I should probably clean up temp files after usage
        // https://stackoverflow.com/questions/400140/how-do-i-automatically-delete-temp-files-in-c
        interaction.SetOutput(new FileInfo(Path.GetTempFileName()));
    }

    public async Task ShowOpenFileDialog(InteractionContext<Unit, FileInfo?> interaction)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new() {
                new FileDialogFilter {
                    Extensions = new() { "db", "baum" }
                }
            }
        };
        var selections = await dialog.ShowAsync(this);

        if (selections is [var filePath])
        {
            interaction.SetOutput(new FileInfo(filePath));
        }
        else
        {
            interaction.SetOutput(null);
        }
    }

    public async Task ShowSaveFileDialog(InteractionContext<Unit, FileInfo?> interaction)
    {
        var dialog = new SaveFileDialog
        {
            Filters = new() {
                new FileDialogFilter {
                    Extensions = new() { "baum" }
                }
            }
        };

        var saveFile = await dialog.ShowAsync(this);

        if (saveFile != null)
        {
            interaction.SetOutput(new FileInfo(saveFile));
        }
        else
        {
            interaction.SetOutput(null);
        }
    }
}