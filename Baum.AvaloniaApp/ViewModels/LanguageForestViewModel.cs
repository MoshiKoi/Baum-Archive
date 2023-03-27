using System.IO;
using System.Collections.Generic;
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

    ReactiveCommand<Language, Unit> OpenLanguageCommand { get; }
    ReactiveCommand<Unit, Unit> AddLanguageCommand { get; }

    public LanguageForestViewModel(ReactiveCommand<Language, Unit> openLanguageCommand, IProjectDatabase database)
    {
        Database = database;

        OpenLanguageCommand = openLanguageCommand;
        LanguageTrees = new();
        AddLanguageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await database.SaveAsync(new Language { Name = "Unnamed Language" });
            await LoadAsync();
        });
    }

    public async Task LoadAsync()
    {
        var trees =
            from tree in await Database.GetLanguagesAsync()
            where tree.Parent == null
            select tree;

        LanguageTrees.Clear();
        foreach (var tree in trees)
        {
            LanguageTrees.Add(new(tree, OpenLanguageCommand, Database));
        }
    }
}