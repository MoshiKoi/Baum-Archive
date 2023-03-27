using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baum.AvaloniaApp.Models;

public class Word : ReactiveObject
{
    [NotMapped]
    public bool Transient { get; set; }

    public int Id { get; set; }

    public int? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public Word? Parent { get; set; }

    [Reactive]
    public required string Name { get; set; }

    [Reactive]
    public required string IPA { get; set; }

    public int LanguageId { get; set; }
    public required Language Language { get; set; }
}