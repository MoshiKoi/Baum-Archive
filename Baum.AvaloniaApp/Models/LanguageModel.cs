using System.Diagnostics.CodeAnalysis;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Baum.AvaloniaApp.Models;

public class LanguageModel : ReactiveObject
{
    public int Id { get; set; }

    [Reactive]
    public string? Name { get; set; }

    public int? ParentId { get; set; }

    [Reactive]
    public string SoundChange { get; set; } = "";
}