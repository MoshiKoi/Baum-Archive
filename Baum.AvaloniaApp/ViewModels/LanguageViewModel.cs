using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.Phonology;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class LanguageViewModel : ViewModelBase
{
    PhonologyData Data { get; set; }
    IProjectDatabase Database { get; set; }

    [Reactive]
    public LanguageModel Language { get; set; }

    public ObservableCollection<WordViewModel> Words { get; }

    [Reactive]
    public WordViewModel? CurrentWord { get; set; }

    public ReactiveCommand<Unit, Unit> AddWordCommand { get; }
    public ReactiveCommand<LanguageModel, Unit> SaveCommand { get; }

    public LanguageViewModel(LanguageModel language, IProjectDatabase database, PhonologyData data)
    {
        Language = language;
        Words = new();
        Database = database;
        Data = data;
        AddWordCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Database.AddAsync(new WordModel
            {
                Transient = false,
                Name = "New Word",
                IPA = "",
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