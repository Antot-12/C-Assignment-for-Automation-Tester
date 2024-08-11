using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApp1
{
    [TestFixture]
    public class DirectoryProcessorTests
    {
        private string _testBasePath;

        [SetUp]
        public void Setup()
        {
            // Setup a temporary directory for testing
            _testBasePath = Path.Combine(Path.GetTempPath(), "TestDirectory");
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }
            Directory.CreateDirectory(_testBasePath);
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup the test directory
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }
        }

        [Test]
        public void ProcessDirectory_ShouldReturnCorrectDirectoryData()
        {
            // Arrange: Create test files and directories
            string firstLevelFile = Path.Combine(_testBasePath, "file1.txt");
            File.Create(firstLevelFile).Close();
            
            string firstLevelDir = Path.Combine(_testBasePath, "SubDir1");
            Directory.CreateDirectory(firstLevelDir);
            string secondLevelFile = Path.Combine(firstLevelDir, "file2.txt");
            File.Create(secondLevelFile).Close();

            // Act: Process the directory
            var result = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Assert: Verify the structure
            Assert.AreEqual("TestDirectory", result.DirectoryName);
            Assert.AreEqual(1, result.Files.Count);
            Assert.AreEqual("file1.txt", result.Files[0].FileName);
            Assert.AreEqual(".txt", result.Files[0].Extension);

            Assert.AreEqual(1, result.NestedDirectories.Count);
            Assert.AreEqual("SubDir1", result.NestedDirectories[0].DirectoryName);
            Assert.AreEqual(1, result.NestedDirectories[0].Files.Count);
            Assert.AreEqual("file2.txt", result.NestedDirectories[0].Files[0].FileName);
            Assert.AreEqual(".txt", result.NestedDirectories[0].Files[0].Extension);
        }

        [Test]
        public void GetUniqueFileExtensions_ShouldReturnCorrectExtensions()
        {
            // Arrange: Create test files and directories
            File.Create(Path.Combine(_testBasePath, "file1.txt")).Close();
            File.Create(Path.Combine(_testBasePath, "file2.cs")).Close();
            File.Create(Path.Combine(_testBasePath, "file3.json")).Close();

            var directoryData = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Act: Get unique file extensions
            var extensions = DirectoryProcessor.GetUniqueFileExtensions(directoryData);

            // Assert: Verify unique extensions
            Assert.AreEqual(3, extensions.Count);
            Assert.Contains(".txt", extensions.ToList());
            Assert.Contains(".cs", extensions.ToList());
            Assert.Contains(".json", extensions.ToList());
        }

        [Test]
        public void GetFileExtensionCounts_ShouldReturnCorrectCounts()
        {
            // Arrange: Create test files and directories
            File.Create(Path.Combine(_testBasePath, "file1.txt")).Close();
            File.Create(Path.Combine(_testBasePath, "file2.txt")).Close();
            File.Create(Path.Combine(_testBasePath, "file3.cs")).Close();

            var directoryData = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Act: Get file extension counts
            var extensionCounts = DirectoryProcessor.GetFileExtensionCounts(directoryData);

            // Assert: Verify file extension counts
            Assert.AreEqual(2, extensionCounts[".txt"]);
            Assert.AreEqual(1, extensionCounts[".cs"]);
        }

        [Test]
        public void SerializeAndDeserialize_ShouldWorkCorrectly()
        {
            // Arrange: Create test files and directories
            File.Create(Path.Combine(_testBasePath, "file1.txt")).Close();
            string jsonFilePath = Path.Combine(_testBasePath, "output.json");

            var directoryData = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Act: Serialize and Deserialize
            DirectoryProcessor.SerializeToJson(directoryData, jsonFilePath);
            var deserializedData = DirectoryProcessor.DeserializeFromJson(jsonFilePath);

            // Assert: Verify the data is correctly serialized and deserialized
            Assert.AreEqual(directoryData.DirectoryName, deserializedData.DirectoryName);
            Assert.AreEqual(directoryData.Files.Count, deserializedData.Files.Count);
            Assert.AreEqual(directoryData.Files[0].FileName, deserializedData.Files[0].FileName);
            Assert.AreEqual(directoryData.Files[0].Extension, deserializedData.Files[0].Extension);
        }
    }
}
