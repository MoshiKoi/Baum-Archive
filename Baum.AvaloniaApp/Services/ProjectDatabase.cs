using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Baum.AvaloniaApp.Models;
using Baum.AvaloniaApp.Services.Database;

namespace Baum.AvaloniaApp.Services;

class ProjectDatabase : IProjectDatabase
{
    FileInfo File { get; }

    public ProjectDatabase(FileInfo fileInfo) => File = fileInfo;

    public async Task AddAsync(LanguageModel languageModel)
    {
        using var context = new ProjectContext(File);

        await context.Languages.AddAsync(new Language
        {
            Name = languageModel.Name,
            ParentId = languageModel.ParentId,
            SoundChange = languageModel.SoundChange,
        });

        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LanguageModel languageModel)
    {
        using var context = new ProjectContext(File);

        var language = await context.Languages.FindAsync(languageModel.Id);
        if (language == null) throw new InvalidOperationException("No language found in database");

        language.Name = languageModel.Name;
        language.SoundChange = languageModel.SoundChange;

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LanguageModel>> GetChildrenAsync(int languageId)
    {
        using var context = new ProjectContext(File);

        var entity = await context.Languages.FindAsync(languageId);
        if (entity == null) throw new InvalidOperationException();

        return await (
            from language in context.Languages
            where language.ParentId == languageId
            select new LanguageModel
            {
                Id = language.Id,
                Name = language.Name,
                ParentId = language.ParentId,
                SoundChange = language.SoundChange,
            }).ToArrayAsync();
    }

    public async Task<IEnumerable<LanguageModel>> GetLanguagesAsync()
    {
        using var context = new ProjectContext(File);

        return await context.Languages.Select(l => new LanguageModel
        {
            Id = l.Id,
            Name = l.Name,
            ParentId = l.ParentId,
            SoundChange = l.SoundChange,
        }).ToArrayAsync();
    }

    async Task<WordModel> GetWordAsync(int wordId)
    {
        using var context = new ProjectContext(File);

        var word = await context.Words.FindAsync(wordId);
        if (word == null) throw new InvalidOperationException("Word doesn't exist");

        return new WordModel
        {
            Transient = false,
            Id = word.Id,
            Name = word.Name,
            IPA = word.IPA,
            ParentId = word.ParentId,
            LanguageId = word.LanguageId,
        };
    }


    public async Task<IEnumerable<WordModel>> GetWordsAsync(int languageId)
    {
        using var context = new ProjectContext(File);

        var language = await context.Languages.FindAsync(languageId);
        if (language == null) throw new InvalidOperationException("No language found in database");

        List<WordModel> words = new();

        await foreach (var word in context.Entry(language).Collection(l => l.Words).Query().AsAsyncEnumerable())
        {
            words.Add(new WordModel
            {
                Transient = false,
                Id = word.Id,
                Name = word.Name,
                IPA = word.IPA,
                ParentId = word.ParentId,
                LanguageId = word.LanguageId,
            });
        }

        if (language.ParentId != null)
        {
            var parentWords = await GetWordsAsync((int)language.ParentId);
            foreach (var parentWord in parentWords)
            {
                words.Add(new WordModel
                {
                    Transient = true,
                    LanguageId = languageId,
                    ParentId = parentWord.Transient ? parentWord.ParentId : parentWord.Id,
                    Name = parentWord.Name,
                    IPA = !string.IsNullOrEmpty(language.SoundChange)
                        ? Baum.Phonology.Notation.NotationParser.Parse(language.SoundChange).Apply(parentWord.IPA)
                        : parentWord.IPA
                });
            }
        }

        return words;
    }

    public async Task<IEnumerable<WordModel>> GetAncestryAsync(WordModel word)
    {
        using var context = new ProjectContext(File);

        if (word.ParentId == null)
            return Enumerable.Empty<WordModel>();

        var ancester = await context.Words.FindAsync(word.ParentId);

        if (ancester == null)
            throw new InvalidOperationException("Ancestor does not exist");


        var wordLanguage = await context.Languages.FindAsync(word.LanguageId);
        if (wordLanguage == null)
            throw new InvalidOperationException("Language does not exist");

        await context.Entry(wordLanguage)
            .Reference(l => l.Parent)
            .LoadAsync();

        List<Language> languageChain = new() { };

        while (wordLanguage.Id != ancester.LanguageId)
        {
            if (wordLanguage.SoundChange != null)
                languageChain.Add(wordLanguage);

            await context.Entry(wordLanguage)
                .Reference(l => l.Parent)
                .LoadAsync();

            wordLanguage = wordLanguage.Parent ?? throw new InvalidOperationException("Ancestor is not an ancestor");
        }

        List<WordModel> wordChain = new();
        wordChain.Add(new()
        {
            Transient = false,
            ParentId = ancester.ParentId,
            LanguageId = ancester.LanguageId,
            Name = ancester.Name,
            IPA = ancester.IPA
        });

        foreach (var language in Enumerable.Reverse(languageChain))
        {
            var last = wordChain.Last();
            wordChain.Add(new WordModel
            {
                Transient = true,
                ParentId = ancester.Id,
                LanguageId = language.Id,
                Name = last.Name,
                IPA = Baum.Phonology.Notation.NotationParser.Parse(language.SoundChange).Apply(last.IPA),
            });
        }

        return wordChain;
    }

    public async Task<WordModel> AddAsync(WordModel word)
    {
        using var context = new ProjectContext(File);

        var language = await context.Languages.FindAsync(word.LanguageId);
        if (language == null) throw new InvalidOperationException("Language doesn't exist");

        var entry = await context.Words.AddAsync(new Word
        {
            Language = language,
            Name = word.Name,
            IPA = word.IPA,
            ParentId = word.ParentId,
        });

        await context.SaveChangesAsync();

        word.Id = entry.Entity.Id;
        word.Transient = false;

        return word;
    }

    public async Task UpdateAsync(WordModel wordModel)
    {
        using var context = new ProjectContext(File);

        var word = await context.Words.FindAsync(wordModel.Id);
        if (word == null) throw new InvalidOperationException("Word doesn't exist");

        word.Name = wordModel.Name;
        word.IPA = wordModel.IPA;
        word.ParentId = wordModel.ParentId;

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