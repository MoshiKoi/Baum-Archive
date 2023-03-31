using ReactiveUI;

namespace Baum.AvaloniaApp.Models;

public class WordModel : ReactiveObject
{
    public WordModel(string name, string ipa)
        => (_name, _ipa) = (name, ipa);

    public required bool Transient { get; set; }
    public int Id { get; set; }
    public int? AncestorId { get; set; }
    public required int LanguageId { get; set; }

    string _name;
    public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }

    string _ipa;
    public string IPA { get => _ipa; set => this.RaiseAndSetIfChanged(ref _ipa, value); }
}