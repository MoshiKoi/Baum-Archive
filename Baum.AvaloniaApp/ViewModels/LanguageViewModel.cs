using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

using Baum.Phonology;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class LanguageViewModel : ViewModelBase
{
    PhonologyData Data { get; set; }
    IProjectDatabase Database { get; set; }

    LanguageModel _languageModel;
    public LanguageModel Language { get => _languageModel; set => this.RaiseAndSetIfChanged(ref _languageModel, value); }

    public ObservableCollection<WordViewModel> Words { get; }

    WordViewModel? _currentWord;
    public WordViewModel? CurrentWord { get => _currentWord; set => this.RaiseAndSetIfChanged(ref _currentWord, value); }

    public ReactiveCommand<Unit, Unit> AddWordCommand { get; }
    public ReactiveCommand<LanguageModel, Unit> SaveCommand { get; }

    public LanguageViewModel(LanguageModel language, IProjectDatabase database, PhonologyData data)
    {
        _languageModel = language;
        Words = new();
        Database = database;
        Data = data;
        AddWordCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Database.AddAsync(new WordModel("New Word", "")
            {
                Transient = false,
                LanguageId = Language.Id
            });
            await LoadAsync();
        });

        SaveCommand = ReactiveCommand.CreateFromTask(async (LanguageModel language) =>
        {
            await Database.UpdateAsync(language);
            await LoadAsync();
        });

        this.WhenAnyValue(
            _ => _.Language,
            _ => _.Language.Name,
            _ => _.Language.SoundChange,
            (l, _, _) => l)
            .InvokeCommand(SaveCommand);

        this.WhenAnyValue(_ => _.CurrentWord)
            .IgnoreElements()
            .InvokeCommand(ReactiveCommand.CreateFromTask(_ => LoadAsync()));
    }

    public async Task LoadAsync()
    {
        Words.Clear();
        foreach (var word in await Database.GetWordsAsync(Language.Id, Data))
        {
            Words.Add(new(word, Database, Data));
        }
    }
}