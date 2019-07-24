using System;
using System.IO;
using TheDialgaTeam.Microsoft.Extensions.DependencyInjection;

namespace Tuckfirtle.Node.Services.IO
{
    public sealed class FilePathService : IInitializable
    {
        public string LoggerFilePath { get; private set; }

        public void Initialize()
        {
            var logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            LoggerFilePath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
        }
    }
}