using System.IO;

namespace Baum.AvaloniaApp.Services;

class ProjectDatabaseFactory : IProjectDatabaseFactory
{
    public IProjectDatabase Create(FileInfo fileInfo) => new ProjectDatabase(fileInfo);
}