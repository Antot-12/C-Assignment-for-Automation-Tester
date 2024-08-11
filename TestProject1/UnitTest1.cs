using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using ConsoleApp1;
using Xunit;

namespace TestProject1
{
    public class DirectoryProcessorTests : IDisposable
    {
        private readonly string _testBasePath;

        public DirectoryProcessorTests()
        {
            // Setup a temporary directory for testing
            _testBasePath = Path.Combine(Path.GetTempPath(), "TestDirectory");
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }
            Directory.CreateDirectory(_testBasePath);
        }

        public void Dispose()
        {
            // Cleanup the test directory
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }
        }

        [Fact]
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
            Assert.Equal("TestDirectory", result.DirectoryName);
            Assert.Single(result.Files);
            Assert.Equal("file1.txt", result.Files[0].FileName);
            Assert.Equal(".txt", result.Files[0].Extension);

            Assert.Single(result.NestedDirectories);
            Assert.Equal("SubDir1", result.NestedDirectories[0].DirectoryName);
            Assert.Single(result.NestedDirectories[0].Files);
            Assert.Equal("file2.txt", result.NestedDirectories[0].Files[0].FileName);
            Assert.Equal(".txt", result.NestedDirectories[0].Files[0].Extension);
        }

        [Fact]
        public void ProcessDirectory_ShouldHandleEmptyDirectory()
        {
            // Act: Process the empty directory
            var result = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Assert: Verify the structure
            Assert.Equal("TestDirectory", result.DirectoryName);
            Assert.Empty(result.Files);
            Assert.Empty(result.NestedDirectories);
        }

        [Fact]
        public void ProcessDirectory_ShouldHandleDirectoryWithOnlySubdirectories()
        {
            // Arrange: Create subdirectories without files
            string firstLevelDir = Path.Combine(_testBasePath, "SubDir1");
            Directory.CreateDirectory(firstLevelDir);
            string secondLevelDir = Path.Combine(firstLevelDir, "SubSubDir1");
            Directory.CreateDirectory(secondLevelDir);

            // Act: Process the directory
            var result = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Assert: Verify the structure
            Assert.Equal("TestDirectory", result.DirectoryName);
            Assert.Empty(result.Files);

            Assert.Single(result.NestedDirectories);
            Assert.Equal("SubDir1", result.NestedDirectories[0].DirectoryName);
            Assert.Empty(result.NestedDirectories[0].Files);
            Assert.Single(result.NestedDirectories[0].NestedDirectories);
            Assert.Equal("SubSubDir1", result.NestedDirectories[0].NestedDirectories[0].DirectoryName);
            Assert.Empty(result.NestedDirectories[0].NestedDirectories[0].Files);
        }

        [Fact]
        public void ProcessDirectory_ShouldHandleFilesWithoutExtensions()
        {
            // Arrange: Create files without extensions
            string fileWithoutExtension = Path.Combine(_testBasePath, "fileWithoutExtension");
            File.Create(fileWithoutExtension).Close();

            // Act: Process the directory
            var result = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Assert: Verify the structure
            Assert.Single(result.Files);
            Assert.Equal("fileWithoutExtension", result.Files[0].FileName);
            Assert.Equal(string.Empty, result.Files[0].Extension);
        }

        [Fact]
        public void ProcessDirectory_ShouldHandleLargeNumberOfFiles()
        {
            // Arrange: Create a large number of files
            for (int i = 0; i < 1000; i++)
            {
                File.Create(Path.Combine(_testBasePath, $"file{i}.txt")).Close();
            }

            // Act: Process the directory
            var result = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Assert: Verify the structure
            Assert.Equal(1000, result.Files.Count);
            Assert.All(result.Files, file => Assert.Equal(".txt", file.Extension));
        }

        [Fact]
        public void ProcessDirectory_ShouldHandleDeeplyNestedDirectories()
        {
            // Arrange: Create deeply nested directories
            string currentPath = _testBasePath;
            for (int i = 0; i < 10; i++)
            {
                currentPath = Path.Combine(currentPath, $"SubDir{i}");
                Directory.CreateDirectory(currentPath);
                File.Create(Path.Combine(currentPath, $"file{i}.txt")).Close();
            }

            // Act: Process the directory
            var result = DirectoryProcessor.ProcessDirectory(_testBasePath);

            // Assert: Verify the nested structure
            DirectoryData currentData = result;
            for (int i = 0; i < 10; i++)
            {
                Assert.Single(currentData.NestedDirectories);
                Assert.Equal($"SubDir{i}", currentData.NestedDirectories[0].DirectoryName);
                Assert.Single(currentData.NestedDirectories[0].Files);
                Assert.Equal($"file{i}.txt", currentData.NestedDirectories[0].Files[0].FileName);
                currentData = currentData.NestedDirectories[0];
            }
        }

        [Fact]
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
            Assert.Equal(3, extensions.Count);
            Assert.Contains(".txt", extensions.ToList());
            Assert.Contains(".cs", extensions.ToList());
            Assert.Contains(".json", extensions.ToList());
        }

        [Fact]
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
            Assert.Equal(2, extensionCounts[".txt"]);
            Assert.Equal(1, extensionCounts[".cs"]);
        }

        [Fact]
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
            Assert.Equal(directoryData.DirectoryName, deserializedData.DirectoryName);
            Assert.Equal(directoryData.Files.Count, deserializedData.Files.Count);
            Assert.Equal(directoryData.Files[0].FileName, deserializedData.Files[0].FileName);
            Assert.Equal(directoryData.Files[0].Extension, deserializedData.Files[0].Extension);
        }
        
    }
}
