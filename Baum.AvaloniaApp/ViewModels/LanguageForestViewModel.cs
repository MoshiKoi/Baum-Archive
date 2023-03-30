using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class LanguageForestViewModel : ViewModelBase
{
    IProjectDatabase Database { get; }

    ObservableCollection<LanguageTreeViewModel> LanguageTrees { get; set; }

    ReactiveCommand<LanguageModel, Unit> OpenLanguageCommand { get; }
    ReactiveCommand<Unit, Unit> AddLanguageCommand { get; }

    public LanguageForestViewModel(ReactiveCommand<LanguageModel, Unit> openLanguageCommand, IProjectDatabase database)
    {
        Database = database;

        OpenLanguageCommand = openLanguageCommand;
        LanguageTrees = new();
        AddLanguageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await database.AddAsync(new LanguageModel { Name = "Unnamed Language" });
            await LoadAsync();
        });
    }

    public async Task LoadAsync()
    {
        var trees =
            from tree in await Database.GetLanguagesAsync()
            where tree.ParentId == null
            select tree;

        LanguageTrees.Clear();
        foreach (var tree in trees)
        {
            LanguageTrees.Add(new(tree, OpenLanguageCommand, Database));
        }
    }
}