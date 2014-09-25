using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test.Core.Exceptions;

namespace Test.Common.IO
{
    public class FileManager : IFileManager
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string GetFileExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            var extIndex = fileName.LastIndexOf('.') + 1;

            if (0 == extIndex || extIndex == fileName.Length)
                throw new ArgumentException("Invalid file name", fileName);

            return fileName.Substring(extIndex);
        }

        public IEnumerable<FileInfo> GetDirectoryFiles(string directoryPath, string searchQuery, int maxFiles,
            DateTime maxDateCreated)
        {
            if (!DirectoryExists(directoryPath)) throw new DirectoryNotFoundException(directoryPath);

            var utcMaxCreatedDate = maxDateCreated.ToUniversalTime();
            var dirInfo = new DirectoryInfo(directoryPath);
            return dirInfo.GetFiles(searchQuery,
                SearchOption.TopDirectoryOnly)
                .Where(f => f.CreationTimeUtc >= utcMaxCreatedDate)
                .OrderByDescending(f => f.CreationTimeUtc)
                .Take(maxFiles);
        }

        public IEnumerable<DirectoryInfo> GetSubdirectories(string rootPath, bool recurse)
        {
            const string allSearchPattern = "*";

            if (!DirectoryExists(rootPath)) throw new DirectoryNotFoundException(rootPath);

            var dirInfo = new DirectoryInfo(rootPath);
            return dirInfo.GetDirectories(allSearchPattern,
                recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public Task<FileInfo> CopyFileAsync(string sourcePath, string destinationPath, TaskScheduler context = null)
        {
            // start task
            var token = Task.Factory.CancellationToken;

            if (null == context)
            {
                return Task.Factory.StartNew(() => CopyFile(sourcePath, destinationPath), token);
            }

            return Task.Factory.StartNew(() => CopyFile(sourcePath, destinationPath), token, TaskCreationOptions.None,
                context);
        }

        public FileInfo CopyFile(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath)) throw new ArgumentNullException("sourcePath");
            if (string.IsNullOrWhiteSpace(destinationPath)) throw new ArgumentNullException("destinationPath");

            try
            {
                var sourceFile = new FileInfo(sourcePath);
                var destFile = new FileInfo(destinationPath);

                if (!DirectoryExists(destFile.DirectoryName)) CreateDirectory(destFile.DirectoryName);

                return sourceFile.CopyTo(destFile.FullName, true);
            }
            catch (Exception ex)
            {
                throw new CopyFileException("CopyFile failed", ex)
                      {
                          SourceFile = sourcePath,
                          DestinationFile = destinationPath
                      };
            }
        }
    }
}
