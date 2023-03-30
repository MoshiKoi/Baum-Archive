using System.IO;

namespace Baum.AvaloniaApp.Services;

public interface IProjectDatabaseFactory
{
    IProjectDatabase Create(FileInfo fileInfo);
}