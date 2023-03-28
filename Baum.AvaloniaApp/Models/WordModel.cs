using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Baum.AvaloniaApp.Models;

public class WordModel : ReactiveObject
{
    public required bool Transient { get; set; }
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public required int LanguageId { get; set; }

    [Reactive]
    public required string Name { get; set; }

    [Reactive]
    public required string IPA { get; set; }
}