using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Test.Common.IO
{
    public interface IFileManager
    {
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        string GetFileExtension(string fileName);
        IEnumerable<FileInfo> GetDirectoryFiles(string directoryPath, string searchQuery, int maxFiles,
            DateTime maxDateCreated);
        IEnumerable<DirectoryInfo> GetSubdirectories(string rootPath, bool recurse);
        Task<FileInfo> CopyFileAsync(string sourcePath, string destinationPath, TaskScheduler context = null);
        FileInfo CopyFile(string sourcePath, string destinationPath);
    }
}
