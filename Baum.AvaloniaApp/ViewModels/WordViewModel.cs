using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class WordViewModel : ViewModelBase
{
    [Reactive]
    public Word Word { get; set; }

    ReactiveCommand<Word, Unit> SaveCommand { get; }

    public WordViewModel(Word word, IProjectDatabase database)
    {
        Word = word;
        Database = database;

        SaveCommand = ReactiveCommand.CreateFromTask((Word word) => Database.SaveAsync(word));

        this.WhenAnyValue(
            _ => _.Word,
            _ => _.Word.Name,
            _ => _.Word.IPA,
            (w, _, _) => w)
            .Skip(1)
            .InvokeCommand(SaveCommand);
    }

    IProjectDatabase Database { get; }
}