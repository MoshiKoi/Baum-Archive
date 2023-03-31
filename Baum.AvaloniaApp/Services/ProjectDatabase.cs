using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Baum.Phonology;
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
            select new LanguageModel(
                language.Name,
                language.ParentId,
                language.SoundChange) { Id = language.Id }).ToArrayAsync();
    }

    public async Task<IEnumerable<LanguageModel>> GetLanguagesAsync()
    {
        using var context = new ProjectContext(File);

        return await context.Languages
            .Select(l => new LanguageModel(l.Name,l.ParentId,l.SoundChange) { Id = l.Id })
            .ToArrayAsync();
    }

    async Task<WordModel> GetWordAsync(int wordId)
    {
        using var context = new ProjectContext(File);

        var word = await context.Words.FindAsync(wordId);
        if (word == null) throw new InvalidOperationException("Word doesn't exist");

        return new WordModel(word.Name, word.IPA)
        {
            Transient = false,
            Id = word.Id,
            AncestorId = word.AncestorId,
            LanguageId = word.LanguageId,
        };
    }


    public async Task<IEnumerable<WordModel>> GetWordsAsync(int languageId, PhonologyData data)
    {
        using var context = new ProjectContext(File);

        var language = await context.Languages.FindAsync(languageId);
        if (language == null) throw new InvalidOperationException("No language found in database");

        List<WordModel> words = new();

        await foreach (var word in context.Entry(language).Collection(l => l.Words).Query().AsAsyncEnumerable())
        {
            words.Add(new WordModel(word.Name, word.IPA)
            {
                Transient = false,
                Id = word.Id,
                AncestorId = word.AncestorId,
                LanguageId = word.LanguageId,
            });
        }

        if (language.ParentId != null)
        {
            var parentWords = await GetWordsAsync((int)language.ParentId, data);
            foreach (var parentWord in parentWords)
            {
                SoundChange.TryApply(parentWord.IPA, language.SoundChange, data, out var IPA);

                words.Add(new WordModel(parentWord.Name, IPA)
                {
                    Transient = true,
                    LanguageId = languageId,
                    AncestorId = parentWord.Transient ? parentWord.AncestorId : parentWord.Id
                });
            }
        }

        return words;
    }

    public async Task<IEnumerable<WordModel>> GetAncestryAsync(WordModel word, PhonologyData data)
    {
        using var context = new ProjectContext(File);

        if (word.AncestorId == null)
            return Enumerable.Empty<WordModel>();

        var ancester = await context.Words.FindAsync(word.AncestorId);

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
            if (!string.IsNullOrEmpty(wordLanguage.SoundChange))
                languageChain.Add(wordLanguage);

            await context.Entry(wordLanguage)
                .Reference(l => l.Parent)
                .LoadAsync();

            wordLanguage = wordLanguage.Parent ?? throw new InvalidOperationException("Ancestor is not an ancestor");
        }

        List<WordModel> wordChain = new();
        wordChain.Add(new(ancester.Name, ancester.IPA)
        {
            Transient = false,
            AncestorId = ancester.AncestorId,
            LanguageId = ancester.LanguageId,
        });

        foreach (Language intermediate in Enumerable.Reverse(languageChain))
        {
            var last = wordChain.Last();

            SoundChange.TryApply(last.IPA, intermediate.SoundChange, data, out var next);

            // TODO? Possibly do some error handling or notification here instead of just skipping
            if (last.IPA == next) continue;
            wordChain.Add(new WordModel(last.Name, next)
            {
                Transient = true,
                AncestorId = ancester.Id,
                LanguageId = intermediate.Id
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
            AncestorId = word.AncestorId,
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
        word.AncestorId = wordModel.AncestorId;

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