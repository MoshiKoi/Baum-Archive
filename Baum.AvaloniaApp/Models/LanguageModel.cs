using System.Diagnostics.CodeAnalysis;
using ReactiveUI;

namespace Baum.AvaloniaApp.Models;

public class LanguageModel : ReactiveObject
{
    public LanguageModel(string? name, int? parentId = null, string soundChange = "")
        => (_name, _parentId, _soundChange) = (name, parentId, soundChange);

    public int Id { get; set; }

    string? _name;
    public string? Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }

    int? _parentId;
    public int? ParentId { get => _parentId; set => this.RaiseAndSetIfChanged(ref _parentId, value); }

    string _soundChange;
    public string SoundChange { get => _soundChange; set => this.RaiseAndSetIfChanged(ref _soundChange, value); }
}