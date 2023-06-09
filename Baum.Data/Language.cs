using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baum.Data;

public class Language
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public List<Word> Words { get; set; } = new();
    public string SoundChange { get; set; } = "";

    public int? ParentId { get; set; }
    public Language? Parent { get; set; }


    [InverseProperty(nameof(Parent))]
    public List<Language> Children { get; set; } = new();
}