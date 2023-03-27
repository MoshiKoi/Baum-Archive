using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class ProjectViewModel : ViewModelBase
{
    IProjectDatabase Database { get; }
    FileInfo? SaveFileInfo { get; set; }

    [Reactive]
    public ViewModelBase? Content { get; set; }

    public ReactiveCommand<Unit, Unit> OpenLanguageForestCommand { get; }
    public ReactiveCommand<Language, Unit> OpenLanguageCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public Interaction<Unit, FileInfo> RequestSaveFileInteraction { get; }

    public ProjectViewModel(IProjectDatabase database, FileInfo? file)
    {
        Database = database;
        SaveFileInfo = file;
        OpenLanguageCommand = ReactiveCommand.Create<Language>(OpenLanguage);
        OpenLanguageForestCommand = ReactiveCommand.Create(() =>
        {
            Content = new LanguageForestViewModel(OpenLanguageCommand, Database);
        });
        SaveCommand = ReactiveCommand.CreateFromTask(SaveToFile);
        RequestSaveFileInteraction = new();
        Content = new LanguageForestViewModel(OpenLanguageCommand, Database);
    }

    async Task SaveToFile()
    {
        if (SaveFileInfo == null)
        {
            SaveFileInfo = await RequestSaveFileInteraction.Handle(Unit.Default);
        }
        if (SaveFileInfo != null)
        {
            Database.SaveToFile(SaveFileInfo);
        }
    }

    void OpenLanguage(Language language) => Content = new LanguageViewModel(language, Database);
}