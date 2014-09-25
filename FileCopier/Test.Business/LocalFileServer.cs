using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Test.Business.Factories;
using Test.Common.IO;
using Test.Core;
using Test.Core.Exceptions;

namespace Test.Business
{
    public class LocalFileServer : IFileServer
    {
        private const int TaskWaitTimeoutMilliseconds = 300000; // 5 minutes
        private readonly IFileManager _fileManager;

        public LocalFileServer()
        {
            _fileManager = FileManagerFactory.Create(typeof(FileManager).AssemblyQualifiedName);
        }

        public CopyFilesResult CopyFiles(CopyFilesParam arg)
        {
            if (null == arg) throw new ArgumentNullException("arg");

            ValidateAndPreprocessCopyFilesParam(arg);

            var now = DateTime.Now;
            var fileMaxDate = new DateTime(now.Year, now.Month, now.Day).AddDays(-arg.MaxFileAgeDays);

            // process
            var newFiles = ProcessCopyFiles(arg.SourceDirectoryPath, arg.DestinationDirectoryPath, arg.FileSearchQuery, fileMaxDate,
                arg.MaxFilesPerFolder);

            Task.WaitAll(newFiles.ToArray(), TaskWaitTimeoutMilliseconds);

            var result = new CopyFilesResult();

            // create return object
            foreach (var task in newFiles)
            {
                if (null != task.Exception)
                {
                    foreach (var e in task.Exception.InnerExceptions)
                    {
                        var cfe = e as CopyFileException;

                        if (null != cfe)
                        {
                            result.FailedLocalFilePaths.Add(cfe.SourceFile);
                        }
                        else
                        {
                            throw task.Exception;
                        }
                    }
                }
                else
                {
                    result.CompletedFilePaths.Add(task.Result.FullName);
                }
            }

            return result;
        }

        #region private helpers
        private void ValidateAndPreprocessCopyFilesParam(CopyFilesParam arg)
        {
            const string sNull = "NULL";

            if (null == arg) return;

            var fileExt = _fileManager.GetFileExtension(arg.FileSearchQuery);
            if (string.IsNullOrWhiteSpace(fileExt))
                throw new ArgumentException(string.Format("Invalid file extension in search query {0}", arg.FileSearchQuery ?? sNull));

            arg.MaxFileAgeDays = (arg.MaxFileAgeDays < 0) ? 0 : arg.MaxFileAgeDays;
            arg.MaxFilesPerFolder = (arg.MaxFilesPerFolder < 1) ? int.MaxValue : arg.MaxFilesPerFolder;

            if (!_fileManager.DirectoryExists(arg.SourceDirectoryPath))
                throw new DirectoryNotFoundException(string.Format("{0} directory does not exist", arg.SourceDirectoryPath ?? sNull));
        }

        private List<Task<FileInfo>> ProcessCopyFiles(string sourcePath, string destinationPath, string fileQuery, DateTime fileMaxAgeDateTime,
            int maxFilesInDirectory)
        {
            // get all files
            var files = _fileManager.GetDirectoryFiles(sourcePath, fileQuery, maxFilesInDirectory, fileMaxAgeDateTime).ToList();
            var subDirs = _fileManager.GetSubdirectories(sourcePath, true);

            foreach (var subDir in subDirs)
            {
                files.AddRange(_fileManager.GetDirectoryFiles(subDir.FullName, fileQuery, maxFilesInDirectory,
                    fileMaxAgeDateTime));
            }

            var destFilesList = new List<Task<FileInfo>>();

            foreach (var sourceFile in files)
            {
                var destFilePath = GetNewFilePath(sourcePath, destinationPath, sourceFile.FullName);

                destFilesList.Add(_fileManager.CopyFileAsync(sourceFile.FullName, destFilePath));
            }

            return destFilesList;
        }

        private string GetNewFilePath(string sourceDirectoryPath, string destinationDirectoryPath, string sourceFilePath)
        {
            var pathBuilder = new StringBuilder(sourceFilePath);
            
            pathBuilder.Remove(0, sourceDirectoryPath.Length);

            return Path.Combine(destinationDirectoryPath, pathBuilder.ToString());
        }
        #endregion
    }
}
