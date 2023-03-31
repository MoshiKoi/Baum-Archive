using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using ReactiveUI;

using Baum.Phonology;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class WordViewModel : ViewModelBase
{
    IProjectDatabase Database { get; }

    WordModel _wordModel;
    public WordModel Word { get => _wordModel; set => this.RaiseAndSetIfChanged(ref _wordModel, value); }

    public ObservableCollection<WordModel> Ancestry { get; set; }

    ReactiveCommand<WordModel, Unit> SaveCommand { get; }

    public WordViewModel(WordModel word, IProjectDatabase database, PhonologyData data)
    {
        _wordModel = word;
        Ancestry = new();
        Database = database;

        SaveCommand = ReactiveCommand.CreateFromTask(async (WordModel word) =>
        {
            if (word.Transient)
                Word = await Database.AddAsync(word);
            else
                await Database.UpdateAsync(word);
        });

        this.WhenAnyValue(
            _ => _.Word,
            _ => _.Word.Name,
            _ => _.Word.IPA,
            (w, _, _) => w)
            .Skip(1)
            .InvokeCommand(SaveCommand);

        this.WhenAnyValue(_ => _.Word)
            .InvokeCommand(ReactiveCommand.CreateFromTask(async (WordModel word) =>
            {
                Ancestry.Clear();
                foreach (var ancestor in await Database.GetAncestryAsync(word, data))
                {
                    Ancestry.Add(ancestor);
                }
            }));
    }
}