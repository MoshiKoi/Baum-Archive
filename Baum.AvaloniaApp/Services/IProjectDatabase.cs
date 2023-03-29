using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Baum.Phonology;
using Baum.AvaloniaApp.Models;

namespace Baum.AvaloniaApp.Services;

public interface IProjectDatabase
{
    bool HasMigrations();
    Task MigrateAsync();

    void SaveToFile(FileInfo fileInfo);

    Task AddAsync(LanguageModel language);
    Task UpdateAsync(LanguageModel language);
    Task<IEnumerable<LanguageModel>> GetLanguagesAsync();
    Task<IEnumerable<LanguageModel>> GetChildrenAsync(int languageId);

    Task<WordModel> AddAsync(WordModel word);
    Task UpdateAsync(WordModel word);
    Task<IEnumerable<WordModel>> GetWordsAsync(int languageId, PhonologyData data);
    Task<IEnumerable<WordModel>> GetAncestryAsync(WordModel word, PhonologyData data);
}