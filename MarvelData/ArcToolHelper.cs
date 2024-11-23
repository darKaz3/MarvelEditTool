using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MarvelData
{
    public partial class ArcToolHelper
    {
        public static void ChooseFileAndUnpack()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ARC files (*.arc)|*.arc|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of the specified file
                    lastFilePath = openFileDialog.FileName;
                    // Call the UnpackArcFile method with the selected file path
                    UnpackArcFile(lastFilePath);
                }
            }
        }

        // Static variable to store the file path
        private static string lastFilePath;

        private static string GetArcToolPath()
        {
            // Get the path of the directory where the application is running
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ARCtool.exe");
        }

        private const string ArcToolArguments = "-pc -dmc4se -texDMC4SE -alwayscomp -tex -txt -v 7";

        public static void UnpackArcFile(string arcFilePath)
        {
            string arcToolPath = GetArcToolPath();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = arcToolPath,
                Arguments = $"{ArcToolArguments} \"{arcFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginOutputReadLine();
                    process.ErrorDataReceived += (sender, args) => Console.WriteLine("ERROR: " + args.Data);
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        MessageBox.Show($"Successfully unpacked to {Path.GetDirectoryName(arcFilePath)}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to unpack the ARC file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while unpacking the ARC file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void OpenFolderAndRepack()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the folder to repack";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                //folderBrowserDialog.SelectedPath = Path.GetDirectoryName(RemoveArcExtension(lastFilePath));
                folderBrowserDialog.SelectedPath = RemoveArcExtension();
                folderBrowserDialog.ShowNewFolderButton = false;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of the specified folder
                    string folderPath = folderBrowserDialog.SelectedPath;
                    // Call the RepackArcFile method with the selected folder path
                    RepackArcFile(folderPath);
                }
            }
        }

        public static void RepackArcFile(string folderPath)
        {
            string arcToolPath = GetArcToolPath();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = arcToolPath,
                Arguments = $"{ArcToolArguments} \"{folderPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginOutputReadLine();
                    process.ErrorDataReceived += (sender, args) => Console.WriteLine("ERROR: " + args.Data);
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        // Delete the folder and the .txt file
                        Directory.Delete(folderPath, true);
                        string txtFilePath = folderPath + ".arc.txt";
                        if (File.Exists(txtFilePath))
                        {
                            File.Delete(txtFilePath);
                        }

                        MessageBox.Show($"Successfully repacked from {folderPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to repack the folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while repacking the folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string RemoveArcExtension()
        {
            if (string.IsNullOrEmpty(lastFilePath))
                return null;


            if (lastFilePath.EndsWith(".arc", StringComparison.OrdinalIgnoreCase))
            {
                return lastFilePath.Substring(0, lastFilePath.Length - 4);
            }
            return lastFilePath;
        }
    }
}
