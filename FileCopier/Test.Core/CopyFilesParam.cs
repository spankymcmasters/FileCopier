
namespace Test.Core
{
    public class CopyFilesParam
    {
        public string SourceDirectoryPath { get; set; }
        public string DestinationDirectoryPath { get; set; }
        public string FileSearchQuery { get; set; }
        public int MaxFileAgeDays { get; set; }
        public int MaxFilesPerFolder { get; set; }
    }
}
