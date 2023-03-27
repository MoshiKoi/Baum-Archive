using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Baum.AvaloniaApp.Models;

namespace Baum.AvaloniaApp.Services;

public interface IProjectDatabaseFactory
{
    IProjectDatabase Create(FileInfo fileInfo);
}