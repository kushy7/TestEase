using System.Diagnostics;
using System;
using System.Threading.Tasks;

namespace TestEaseUpdater;
public partial class MainPage : ContentPage
{
    private int dotCount = 0;
    private System.Timers.Timer timer;

    [Obsolete]
    public MainPage()
    {
        InitializeComponent();

        Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
        {
            dotCount = (dotCount + 1) % 4;
            updatingLabel.Text = "Updating" + new string('.', dotCount);
            return true; // return true to keep the timer running
        });

        StartProcessWithDelay();
    }

    private async void StartProcessWithDelay()
    {
        // Get the command line arguments
        string[] args = Environment.GetCommandLineArgs();
        string argsText = string.Join(" ", args.Skip(1)); // Skip the first argument which is the executable path
        await Task.Delay(2000); // Wait for 2 seconds

        //string testhardcodepath = "C:\\Users\\user\\Downloads\\publish\\TestEase.exe";

        // Get the directory of the .exe file
        string exeDirectory = Directory.GetParent(Path.GetDirectoryName(argsText)).FullName;

        File.WriteAllText("log.txt", exeDirectory);

        foreach (var file in Directory.GetFiles(exeDirectory))
        {
            try
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            catch (Exception ex)
            {
                // Log the exception to a file
                File.WriteAllText("log.txt", $"Failed to delete file {file}. Error: {ex.Message}\n");
            }
        }

        foreach (var dir in Directory.GetDirectories(exeDirectory))
        {
            if (Path.GetFileName(dir) != "publish")
            {
                try
                {
                    File.SetAttributes(dir, FileAttributes.Normal);
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                    // Log the exception to a file
                    File.WriteAllText("log.txt", $"Failed to delete directory {dir}. Error: {ex.Message}\n");
                }
            }

        }


        // Define the publish directory
        string publishDirectory = Path.Combine(exeDirectory, "publish");

        // Move all files and directories from the publish directory to the exeDirectory
        foreach (var file in Directory.GetFiles(publishDirectory))
        {
            string destFile = Path.Combine(exeDirectory, Path.GetFileName(file));
            try
            {
                File.Move(file, destFile);
            }
            catch(Exception ex)
            {
                File.WriteAllText("log.txt", $"Failed to move file {file}. Error: {ex.Message}\n");

            }
        }

        foreach (var dir in Directory.GetDirectories(publishDirectory))
        {
            string destDir = Path.Combine(exeDirectory, Path.GetFileName(dir));
            try
            {
                Directory.Move(dir, destDir);
            }
            catch (Exception ex)
            {
                File.WriteAllText("log.txt", $"Failed to dir file {dir}. Error: {ex.Message}\n");

            }
        }

        // Delete the publish directory
        Directory.Delete(publishDirectory, true);

        string testEasePath = Path.Combine(exeDirectory, "TestEase.exe");



        Process.Start(testEasePath);
        Application.Current.Quit();
    }
}
