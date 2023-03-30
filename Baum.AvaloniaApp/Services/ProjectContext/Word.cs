using System.ComponentModel.DataAnnotations.Schema;

namespace Baum.AvaloniaApp.Services.Database;

public class Word
{
    public int Id { get; set; }

    public int? AncestorId { get; set; }

    [ForeignKey(nameof(AncestorId))]
    public Word? Ancestor { get; set; }

    public required string Name { get; set; }

    public required string IPA { get; set; }

    public int LanguageId { get; set; }
    public required Language Language { get; set; }
}