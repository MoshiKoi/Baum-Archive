using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baum.AvaloniaApp.Services.Database;

public class Word
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public Word? Parent { get; set; }

    public required string Name { get; set; }

    public required string IPA { get; set; }

    public int LanguageId { get; set; }
    public required Language Language { get; set; }
}