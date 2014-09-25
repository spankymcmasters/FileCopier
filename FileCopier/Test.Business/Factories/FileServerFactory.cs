using System;

using Test.Core;

namespace Test.Business.Factories
{
    public class FileServerFactory
    {
        public static IFileServer Create(string fileServerType)
        {
            var type = Type.GetType(fileServerType);
            return (IFileServer)Activator.CreateInstance(type);
        }
    }
}
