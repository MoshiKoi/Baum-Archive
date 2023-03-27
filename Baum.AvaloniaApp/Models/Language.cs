using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace Baum.AvaloniaApp.Models;

public class Language : ReactiveObject
{
    public int Id { get; set; }

    [Reactive]
    public string? Name { get; set; }
    public List<Word> Words { get; set; } = new();
    public Language? Parent { get; set; }

    [Reactive]
    public string? SoundChange { get; set; }

    [InverseProperty(nameof(Parent))]
    public List<Language> Children { get; set; } = new();
}