using System.Collections.Generic;

namespace Test.Core
{
    public class CopyFilesResult
    {
        public IList<string> CompletedFilePaths { get; set; }
        public IList<string> FailedLocalFilePaths { get; set; }

        public CopyFilesResult()
        {
            CompletedFilePaths = new List<string>();
            FailedLocalFilePaths = new List<string>();
        }
    }
}
