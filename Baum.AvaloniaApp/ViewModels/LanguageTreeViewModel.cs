using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class LanguageTreeViewModel : ViewModelBase
{
    [Reactive]
    Language Language { get; set; }

    [Reactive]
    ObservableCollection<LanguageTreeViewModel> Children { get; set; }

    string DecendedFromMessage { get; } = "Decended From:";

    ReactiveCommand<Language, Unit> OpenLanguageCommand { get; }
    ReactiveCommand<Unit, Unit> AddChildCommand { get; }

    IProjectDatabase Database { get; }

    public LanguageTreeViewModel(
        Language language,
        ReactiveCommand<Language, Unit> openLanguageCommand,
        IProjectDatabase database)
    {
        Language = language;
        Children = new();

        Database = database;

        OpenLanguageCommand = openLanguageCommand;
        AddChildCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Database.SaveAsync(new Language
            {
                Parent = Language,
                Name = "Unnamed Language"
            });
            await LoadAsync();
        });
    }

    public async Task LoadAsync()
    {
        Children.Clear();
        foreach (var child in await Database.GetChildrenAsync(Language))
        {
            Children.Add(new(child, OpenLanguageCommand, Database));
        }
    }
}