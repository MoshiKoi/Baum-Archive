using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class LanguageTreeViewModel : ViewModelBase
{
    [Reactive]
    LanguageModel Language { get; set; }

    [Reactive]
    ObservableCollection<LanguageTreeViewModel> Children { get; set; }

    string DecendedFromMessage { get; } = "Decended From:";

    ReactiveCommand<LanguageModel, Unit> OpenLanguageCommand { get; }
    ReactiveCommand<Unit, Unit> AddChildCommand { get; }

    IProjectDatabase Database { get; }

    public LanguageTreeViewModel(
        LanguageModel language,
        ReactiveCommand<LanguageModel, Unit> openLanguageCommand,
        IProjectDatabase database)
    {
        Language = language;
        Children = new();

        Database = database;

        OpenLanguageCommand = openLanguageCommand;
        AddChildCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Database.AddAsync(new LanguageModel
            {
                ParentId = Language.Id,
                Name = "Unnamed Language"
            });
            await LoadAsync();
        });
    }

    public async Task LoadAsync()
    {
        Children.Clear();
        foreach (var child in await Database.GetChildrenAsync(Language.Id))
        {
            Children.Add(new(child, OpenLanguageCommand, Database));
        }
    }
}