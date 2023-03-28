using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class WordViewModel : ViewModelBase
{
    [Reactive]
    public WordModel Word { get; set; }

    public ObservableCollection<WordModel> Ancestry { get; set; }

    ReactiveCommand<WordModel, Unit> SaveCommand { get; }

    public WordViewModel(WordModel word, IProjectDatabase database)
    {
        Word = word;
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
                foreach (var ancestor in await Database.GetAncestryAsync(word))
                {
                    Ancestry.Add(ancestor);
                }
            }));
    }

    IProjectDatabase Database { get; }
}