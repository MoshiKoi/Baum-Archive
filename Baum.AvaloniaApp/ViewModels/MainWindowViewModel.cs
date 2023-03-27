using System.IO;
using System.Threading.Tasks;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive]
    public ViewModelBase Content { get; set; }

    public Interaction<Unit, FileInfo?> RequestFileInteraction { get; }
    public Interaction<Unit, FileInfo?> RequestSaveFileInteraction { get; }
    public Interaction<Unit, FileInfo> RequestTemporaryFileInteraction { get; }

    IProjectDatabaseFactory DatabaseFactory { get; }

    public MainWindowViewModel(IProjectDatabaseFactory databaseFactory)
    {
        Content = new HomeViewModel(
            ReactiveCommand.CreateFromTask(OpenFileAsync),
            ReactiveCommand.CreateFromTask(NewFileAsync));

        RequestFileInteraction = new();
        RequestSaveFileInteraction = new();
        RequestTemporaryFileInteraction = new();
        DatabaseFactory = databaseFactory;
    }

    async Task OpenFileAsync()
    {
        var file = await RequestFileInteraction.Handle(Unit.Default);

        if (file != null)
        {
            // TODO: Implement dirty tracking and stuff
            // var tempFile = await RequestTemporaryFileInteraction.Handle(Unit.Default);
            // file.CopyTo(tempFile.FullName, true);
            var tempFile = file;
            Open(await GetDatabase(tempFile), file);
        }
    }

    async Task NewFileAsync()
    {
        var file = await RequestTemporaryFileInteraction.Handle(Unit.Default);
        Open(await GetDatabase(file), null);
    }

    void Open(IProjectDatabase database, FileInfo? file)
    {
        var vm = new ProjectViewModel(
            database,
            file);

        vm.RequestSaveFileInteraction.RegisterHandler(
            async i => i.SetOutput(await RequestSaveFileInteraction.Handle(Unit.Default)));

        Content = vm;
    }

    async Task<IProjectDatabase> GetDatabase(FileInfo file)
    {
        var database = DatabaseFactory.Create(file);
        if (database.HasMigrations())
        {
            await database.MigrateAsync();
        }
        return database;
    }
}
