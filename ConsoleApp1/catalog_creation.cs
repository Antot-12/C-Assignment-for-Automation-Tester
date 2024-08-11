// using System;
// using System.IO;
//
// namespace DirectoryInfoApp
// {
//     class TestDirectoryCreator
//     {
//         static void Main(string[] args)
//         {
//             // Base path where the test directory will be created
//             string basePath = @"C:\Temp\ExampleFolder";
//
//             // Ensure the base directory exists
//             if (!Directory.Exists(basePath))
//             {
//                 Directory.CreateDirectory(basePath);
//             }
//
//             // Create files at the first level
//             File.Create(Path.Combine(basePath, "FirstLevelFile1.txt")).Close();
//             File.Create(Path.Combine(basePath, "FirstLevelFile2.test")).Close();
//             File.Create(Path.Combine(basePath, "myfolder.json")).Close();
//
//             // Create a first-level directory
//             string firstLevelFolder = Path.Combine(basePath, "FirstLevelFolder1");
//             Directory.CreateDirectory(firstLevelFolder);
//
//             // Create files in the first-level directory
//             File.Create(Path.Combine(firstLevelFolder, "SecondLevelFile1.cs")).Close();
//             File.Create(Path.Combine(firstLevelFolder, "SecondLevelFile2.txt")).Close();
//
//             // Create a second-level directory within the first-level directory
//             string secondLevelFolder = Path.Combine(firstLevelFolder, "SecondLevelFolder1");
//             Directory.CreateDirectory(secondLevelFolder);
//
//             // Create files in the second-level directory
//             File.Create(Path.Combine(secondLevelFolder, "ThirdLevelFile1.txt")).Close();
//
//             Console.WriteLine("Test directory structure created successfully!");
//         }
//     }
// }