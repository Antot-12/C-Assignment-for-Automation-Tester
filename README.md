# DirectoryInfoApp

A C# console application that processes directory structures, extracts file information, and serializes/deserializes directory data to/from JSON files.

## Features

- **Directory Processing**: Recursively scans a directory, extracting file names and extensions.
- **JSON Serialization**: Saves the directory structure and file information into a JSON file.
- **JSON Deserialization**: Loads directory structure and file information from a JSON file.
- **File Extension Analysis**: Counts and lists unique file extensions in the directory.
- **Interactive Console**: Provides a simple console interface for user interaction.

## Usage

### Running the Application

1. Clone the repository and open the project in your favorite IDE.
2. Build the project using .NET Core SDK.
3. Run the application from the console.

    ```bash
    dotnet run
    ```

### Directory Structure Processing

The application allows you to input a directory path or a JSON file path. It will process the directory structure, extract file information, and optionally save it to a JSON file.


### JSON Output Example

Here is an example of how the directory structure is saved to a JSON file:

```json
{
  "DirectoryName": "ExampleFolder",
  "Files": [
    {
      "FileName": "FirstLevelFile1.txt",
      "Extension": ".txt"
    },
    {
      "FileName": "FirstLevelFile2.test",
      "Extension": ".test"
    }
  ],
  "NestedDirectories": [
    {
      "DirectoryName": "FirstLevelFolder1",
      "Files": [
        {
          "FileName": "SecondLevelFile1.cs",
          "Extension": ".cs"
        }
      ],
      "NestedDirectories": [
        {
          "DirectoryName": "SecondLevelFolder1",
          "Files": [
            {
              "FileName": "ThirdLevelFile1.txt",
              "Extension": ".txt"
            }
          ],
          "NestedDirectories": []
        }
      ]
    }
  ]
}
