using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Baum.AvaloniaApp.Models;

namespace Baum.AvaloniaApp.Services;

public interface IProjectDatabase
{
    bool HasMigrations();
    Task MigrateAsync();

    void SaveToFile(FileInfo fileInfo);

    Task SaveAsync(Language language);
    Task<IEnumerable<Language>> GetLanguagesAsync();
    Task<IEnumerable<Language>> GetChildrenAsync(Language language);

    Task SaveAsync(Word word);
    Task<IEnumerable<Word>> GetWordsAsync(Language language);
}