using System;

namespace Test.Core.Exceptions
{
    public class CopyFileException : Exception
    {
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }

        public CopyFileException()
            : this(null, null)
        {
        }

        public CopyFileException(string message)
            : this(message, null)
        {
        }

        public CopyFileException(string messge, Exception innerException)
            : base(messge, innerException)
        {
        }
    }
}
