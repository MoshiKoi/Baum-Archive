using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Baum.AvaloniaApp.Models;

namespace Baum.AvaloniaApp.Services;

class ProjectDatabase : IProjectDatabase
{
    FileInfo File { get; }

    public ProjectDatabase(FileInfo fileInfo) => File = fileInfo;

    public async Task SaveAsync(Language language)
    {
        using var context = new ProjectContext(File);
        context.Languages.Update(language);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Language>> GetChildrenAsync(Language language)
    {
        using var context = new ProjectContext(File);

        var entity = await context.Languages.FindAsync(language.Id);
        if (entity == null) throw new InvalidOperationException();

        return await context.Entry(entity)
            .Collection(l => l.Children)
            .Query()
            .Include(l => l.Parent)
            .AsNoTracking()
            .ToArrayAsync();
    }

    public async Task<IEnumerable<Language>> GetLanguagesAsync()
    {
        using var context = new ProjectContext(File);

        return await context.Languages.Include(l => l.Parent).ToArrayAsync();
    }

    public async Task<IEnumerable<Word>> GetWordsAsync(Language language)
    {
        using var context = new ProjectContext(File);

        List<Word> words = new();

        await foreach (var word in context.Words.Where(w => w.LanguageId == language.Id).Include(w => w.Parent).AsAsyncEnumerable())
        {
            word.Transient = false;
            words.Add(word);
        }

        if (language.Parent != null)
        {
            var parentWords = await GetWordsAsync(language.Parent);
            foreach (var parentWord in parentWords)
            {
                // Skip automatic generation if there exists an existing word inherited
                if (words.Exists(w => w.ParentId == parentWord.Id))
                {
                    continue;
                }

                words.Add(new Word
                {
                    Transient = true,
                    Language = language,
                    Parent = parentWord,
                    Name = parentWord.Name,
                    IPA = language.SoundChange != null
                        ? Baum.Phonology.Notation.NotationParser.Parse(language.SoundChange).Apply(parentWord.IPA)
                        : parentWord.IPA
                });
            }
        }

        return words;
    }

    public async Task SaveAsync(Word word)
    {
        using var context = new ProjectContext(File);
        context.Update(word);
        await context.SaveChangesAsync();
    }

    public bool HasMigrations()
    {
        using var context = new ProjectContext(File);
        return context.Database.GetMigrations().Any();
    }

    public async Task MigrateAsync()
    {
        using var context = new ProjectContext(File);
        await context.Database.MigrateAsync();
    }

    public void SaveToFile(FileInfo fileInfo)
    {
        if (fileInfo != File)
        {
            File.CopyTo(fileInfo.FullName, true); // TODO: Prompt user to confirm overwrite
        }
    }
}