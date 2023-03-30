using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Baum.Phonology;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services;

namespace Baum.AvaloniaApp.ViewModels;

public class ProjectViewModel : ViewModelBase
{
    PhonologyData Data { get; set; }
    IProjectDatabase Database { get; }
    FileInfo? SaveFileInfo { get; set; }

    [Reactive]
    public ViewModelBase? Content { get; set; }

    public ReactiveCommand<Unit, Unit> OpenLanguageForestCommand { get; }
    public ReactiveCommand<LanguageModel, Unit> OpenLanguageCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public Interaction<Unit, FileInfo> RequestSaveFileInteraction { get; }

    public ProjectViewModel(IProjectDatabase database, FileInfo? file, PhonologyData data)
    {
        Data = data;
        Database = database;
        SaveFileInfo = file;
        OpenLanguageCommand = ReactiveCommand.Create<LanguageModel>(OpenLanguage);
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

    void OpenLanguage(LanguageModel language) => Content = new LanguageViewModel(language, Database, Data);
}