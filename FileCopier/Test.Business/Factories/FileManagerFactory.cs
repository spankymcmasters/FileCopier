using System;
using Test.Common.IO;

namespace Test.Business.Factories
{
    public class FileManagerFactory
    {
        public static IFileManager Create(string fileManagerType)
        {
            var type = Type.GetType(fileManagerType);
            return (IFileManager)Activator.CreateInstance(type);
        }
    }
}
