using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AppImageLauncher.Classes;
using Mono.Unix;
namespace AppImageLauncher.Controller;
public class Starter
{
    public static bool IsExecutable(string path)
    {
        var fileInfo = new UnixFileInfo(path);
        return fileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.UserExecute);
    }
    public static async Task<int?> RunApp(Application app)
    {
        var appStatus = IsExecutable(app.AppImagePath!);
        try
        {
            if (!string.IsNullOrWhiteSpace(app.AppImagePath))
            {
                if (!appStatus)
                {
                    using var chmodProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "chmod",
                            Arguments = $"+x \"{app.AppImagePath}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = false,
                        }
                    };

                    chmodProcess.Start();
                    await chmodProcess.WaitForExitAsync();

                    if (chmodProcess.ExitCode != 0)
                    {
                        var error = await chmodProcess.StandardError.ReadToEndAsync();
                        Console.WriteLine("chmod failed: " + error);
                        return null;
                    }
                }
                var appProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = app.AppImagePath,
                        Arguments = app.Argument,
                        UseShellExecute = false,
                    },
                    EnableRaisingEvents = true
                };

                var tcs = new TaskCompletionSource<int?>();
                appProcess.Exited += (_, _) =>
                {
                    Console.WriteLine(appProcess.ExitCode);
                    tcs.SetResult(appProcess.ExitCode);
                    appProcess.Dispose();
                };

                appProcess.Start();
                return await tcs.Task;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return null;
    }

}