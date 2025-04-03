using System;
using System.IO;
using Voxels;

namespace Cub2Vox {
    class Cub2VoxApp {
        static void Main(string[] args) {
            string inputPath = GetInputDirectory();
            string outputPath = Environment.CurrentDirectory +
                "\\Cub2Vox Converted Files\\";
            Directory.CreateDirectory(outputPath);
            ConvertDirectory(inputPath, outputPath);
            Console.WriteLine($"Finished! Any converted files are in " +
                $"\"{outputPath}\"");
            Console.ReadLine();
        }
        static string GetInputDirectory() {
            Console.Write("File folder with .cub files: ");
            string inputPath = Console.ReadLine();
            try {
                VerifyDirectoryPath(inputPath);
            }
            catch (ArgumentException) {
                Console.WriteLine($"The path \"{inputPath}\" is invalid. " +
                    "Please enter a valid folder path.");
                inputPath = GetInputDirectory();
            }
            catch (System.Security.SecurityException) {
                Console.WriteLine("Insufficient permissions to access" +
                    $"\"{inputPath}\".{Environment.NewLine}" +
                    "Select another folder or run this " +
                    "application as administrator and try again.");
                inputPath = GetInputDirectory();
            }
            catch (NotSupportedException) {
                Console.WriteLine($"The path \"{inputPath}\" " +
                    "contains a colon ':' outside of the volume identifier. " +
                    "Please try again.");
                inputPath = GetInputDirectory();
            }
            catch (PathTooLongException) {
                Console.WriteLine($"The path \"{inputPath}\" " +
                    "is too long. Please enter a valid folder path.");
                inputPath = GetInputDirectory();
            }
            catch (DirectoryNotFoundException) {
                Console.WriteLine($"The path \"{inputPath}\" " +
                    "could not be found. Please enter a valid folder path.");
                inputPath = GetInputDirectory();
            }
            return inputPath;
        }
        static void VerifyDirectoryPath(string path) {
            Path.GetFullPath(path);
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
        }
        static void ConvertDirectory(string inputPath, string outputPath) {
            var inputFiles = new DirectoryInfo(inputPath).GetFiles();
            int progress = 1;
            foreach (var file in inputFiles) {
                Console.WriteLine($"Importing {progress}/" +
                    $"{inputFiles.Length}:\"{file.Name}\"");

                var voxData = ImportCubFile(file.FullName);
                if (voxData != null) {
                    Console.WriteLine($"Exporting {progress}/" +
                        $"{inputFiles.Length}:\"{file.Name}\"");
                    var outputFilePath = GetOutputFilePath(outputPath,
                        Path.GetFileNameWithoutExtension(file.FullName));

                    VoxExport.Export(voxData, outputFilePath);
                }

                progress++;
            }
        }
        static string GetOutputFilePath(string outputDir, string fileName) {
            return outputDir + fileName + ".vox";
        }

        public static VoxelData ImportVoxFile(string filePath) {
            VoxelData importedData = null;
            try {
                importedData = VoxelImport.Import(filePath);
            } catch (IOException) {
                Console.WriteLine($"Unable to read file {filePath}");
            }
            if (importedData == null)
                Console.WriteLine($"Unable to read file {filePath}");
            return importedData;
        }

        public static VoxelData ImportCubFile(string filePath) {
            VoxelData importedData = null;
            try {
                importedData = CubImport.Import(filePath);
            } catch (IOException e) {
                Console.WriteLine($"Exception while reading file {filePath}", e);
            }
            if (importedData == null)
                Console.WriteLine($"Unable to read file {filePath}");
            return importedData;
        }
    }
}