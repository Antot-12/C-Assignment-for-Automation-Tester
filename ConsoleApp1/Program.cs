using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConsoleApp1
{
    // Class to hold the file information
    public class FileInfoData
    {
        public string FileName { get; set; }  // Name of the file
        public string Extension { get; set; } // File extension (e.g., .txt, .json)
    }

    // Class to hold the directory information
    public class DirectoryData
    {
        public string DirectoryName { get; set; } // Name of the directory
        public List<FileInfoData> Files { get; set; } = new List<FileInfoData>(); // List of files in the directory
        public List<DirectoryData> NestedDirectories { get; set; } = new List<DirectoryData>(); // List of nested directories
    }

    public class DirectoryProcessor
    {
        // Method to process a directory and return its structure as DirectoryData
        public static DirectoryData ProcessDirectory(string path)
        {
            // Initialize DirectoryData with the directory name
            DirectoryData directoryData = new DirectoryData
            {
                DirectoryName = Path.GetFileName(path)
            };

            // Get all files in the current directory and add them to the directoryData.Files list
            foreach (var file in Directory.GetFiles(path))
            {
                directoryData.Files.Add(new FileInfoData
                {
                    FileName = Path.GetFileName(file),
                    Extension = Path.GetExtension(file)
                });
            }

            // Get all subdirectories and process each recursively
            foreach (var directory in Directory.GetDirectories(path))
            {
                directoryData.NestedDirectories.Add(ProcessDirectory(directory));
            }

            return directoryData; // Return the populated directory data
        }

        // Method to serialize DirectoryData to a JSON file
        public static void SerializeToJson(DirectoryData data, string filePath)
        {
            // Ensure the directory exists before writing the JSON file
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var options = new JsonSerializerOptions { WriteIndented = true }; // Format JSON with indentation
            var json = JsonSerializer.Serialize(data, options); // Serialize DirectoryData to JSON
            File.WriteAllText(filePath, json); // Write JSON to a file
        }

        // Method to deserialize JSON file back into DirectoryData
        public static DirectoryData DeserializeFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath); // Read JSON from a file
            return JsonSerializer.Deserialize<DirectoryData>(json); // Deserialize JSON to DirectoryData
        }

        // Method to retrieve a unique set of file extensions within the directory data
        public static HashSet<string> GetUniqueFileExtensions(DirectoryData data)
        {
            HashSet<string> extensions = new HashSet<string>(); // Set to store unique extensions

            // Add extensions of all files in the current directory
            foreach (var file in data.Files)
            {
                extensions.Add(file.Extension);
            }

            // Recursively add extensions from nested directories
            foreach (var nestedDir in data.NestedDirectories)
            {
                var nestedExtensions = GetUniqueFileExtensions(nestedDir);
                extensions.UnionWith(nestedExtensions);
            }

            return extensions; // Return the set of unique file extensions
        }

        // Method to count occurrences of each file extension in the directory data
        public static Dictionary<string, int> GetFileExtensionCounts(DirectoryData data)
        {
            Dictionary<string, int> extensionCounts = new Dictionary<string, int>();

            // Count occurrences of each file extension in the current directory
            foreach (var file in data.Files)
            {
                if (extensionCounts.ContainsKey(file.Extension))
                {
                    extensionCounts[file.Extension]++;
                }
                else
                {
                    extensionCounts[file.Extension] = 1;
                }
            }

            // Recursively count occurrences in nested directories
            foreach (var nestedDir in data.NestedDirectories)
            {
                var nestedCounts = GetFileExtensionCounts(nestedDir);
                foreach (var kvp in nestedCounts)
                {
                    if (extensionCounts.ContainsKey(kvp.Key))
                    {
                        extensionCounts[kvp.Key] += kvp.Value;
                    }
                    else
                    {
                        extensionCounts[kvp.Key] = kvp.Value;
                    }
                }
            }

            return extensionCounts;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please provide a folder or a JSON with folder information:");
                string inputPath = Console.ReadLine();

                // Ask if the user wants to quit
                if (inputPath.ToLower() == "exit")
                {
                    Console.WriteLine("Are you sure you want to quit? (y/n)");
                    string quitConfirmation = Console.ReadLine();
                    if (quitConfirmation.ToLower() == "y")
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                DirectoryData directoryData = null;

                // Process the directory if the path is valid
                if (Directory.Exists(inputPath))
                {
                    directoryData = DirectoryProcessor.ProcessDirectory(inputPath);
                }
                // Deserialize JSON file if a valid JSON file path is provided
                else if (File.Exists(inputPath) && Path.GetExtension(inputPath) == ".json")
                {
                    directoryData = DirectoryProcessor.DeserializeFromJson(inputPath);
                }
                else
                {
                    Console.WriteLine("Invalid directory or JSON file path.");
                    continue; // Ask for input again
                }

                // Get and display the unique file extensions found in the directory structure
                var extensions = DirectoryProcessor.GetUniqueFileExtensions(directoryData);
                var extensionCounts = DirectoryProcessor.GetFileExtensionCounts(directoryData);

                Console.WriteLine($"Number of unique extensions found: {extensions.Count}");
                Console.WriteLine("Extensions found in folder:");
                foreach (var kvp in extensionCounts)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value} file(s)");
                }

                Console.WriteLine("Save to JSON? (y/n)");
                string saveToJson = Console.ReadLine();

                // If the user chooses to save the directory data to a JSON file
                if (saveToJson.ToLower() == "y")
                {
                    Console.WriteLine("Please provide the JSON file location (with the file extension):");
                    string jsonFilePath = Console.ReadLine();
                    DirectoryProcessor.SerializeToJson(directoryData, jsonFilePath);
                    Console.WriteLine($"Directory information saved to {jsonFilePath}");
                }
            }
        }
    }
}
