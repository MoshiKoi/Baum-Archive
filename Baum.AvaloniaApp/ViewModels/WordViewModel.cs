using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using Avalonia;
using Avalonia.Platform;
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
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                List<Phonology.Sound> sounds = new();
                using (var stream = assets.Open(new Uri("avares://Baum.AvaloniaApp/Assets/ipa-consonants.csv")))
                {
                    using var reader = new StreamReader(stream);
                    sounds.AddRange(await Phonology.Utils.CsvLoader.LoadAsync(reader));
                }

                using (var stream = assets.Open(new Uri("avares://Baum.AvaloniaApp/Assets/ipa-vowels.csv")))
                {
                    using var reader = new StreamReader(stream);
                    sounds.AddRange(await Phonology.Utils.CsvLoader.LoadAsync(reader));
                }
                Ancestry.Clear();
                foreach (var ancestor in await Database.GetAncestryAsync(word, new(sounds)))
                {
                    Ancestry.Add(ancestor);
                }
            }));
    }

    IProjectDatabase Database { get; }
}