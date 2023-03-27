using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class LanguageViewModel : ViewModelBase
{
    IProjectDatabase Database { get; set; }

    [Reactive]
    public Language Language { get; set; }

    public ObservableCollection<WordViewModel> Words { get; }

    [Reactive]
    public WordViewModel? CurrentWord { get; set; }

    public ReactiveCommand<Unit, Unit> AddWordCommand { get; }
    public ReactiveCommand<Language, Unit> SaveCommand { get; }

    public LanguageViewModel(Language language, IProjectDatabase database)
    {
        Language = language;
        Words = new();
        Database = database;
        AddWordCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Database.SaveAsync(new Word { Name = "New Word", IPA = "", Language = Language });
            await LoadAsync();
        });

        SaveCommand = ReactiveCommand.CreateFromTask(async (Language language) =>
        {
            await Database.SaveAsync(language);
            await LoadAsync();
        });

        this.WhenAnyValue(
            _ => _.Language,
            _ => _.Language.Name,
            _ => _.Language.SoundChange,
            (l, _, _) => l)
            .InvokeCommand(SaveCommand);
    }

    public async Task LoadAsync()
    {
        Words.Clear();
        foreach (var word in await Database.GetWordsAsync(Language))
        {
            Words.Add(new(word, Database));
        }
    }
}