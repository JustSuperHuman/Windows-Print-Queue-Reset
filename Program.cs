using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        if (!IsRunAsAdministrator())
        {
            RestartAsAdministrator();
            return;
        }

        Console.WriteLine("Resetting local print queue...");
        await ResetPrintQueueAsync();

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    static bool IsRunAsAdministrator()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    static void RestartAsAdministrator()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = Environment.GetCommandLineArgs()[0],
            Verb = "runas",
            UseShellExecute = true
        };

        try
        {
            Process.Start(startInfo);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            Console.WriteLine("This program must be run as administrator.");
        }

        Environment.Exit(0);
    }

    static async Task ResetPrintQueueAsync()
    {
        // Stopping spooler service
        ExecuteCommand("net stop spooler");

        // Deleting print queue
        string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string spoolPath = Path.Combine(system32Path, "spool\\PRINTERS");
        foreach (string file in Directory.GetFiles(spoolPath))
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete file: {file}. Error: {ex.Message}");
            }
        }

        // Starting spooler service
        ExecuteCommand("net start spooler");

        // Emulating web requests
        await SendRequestAsync("stop_spooler");
        await SendRequestAsync("delete_queue");
        await SendRequestAsync("start_spooler");

        Console.WriteLine("Print queue has been reset.");
    }

    static void ExecuteCommand(string command)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe")
        {
            Verb = "runas",
            Arguments = "/c " + command,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        using (Process process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }

    static async Task SendRequestAsync(string action)
    {
        switch (action)
        {
            case "stop_spooler":
                Console.WriteLine("Print Server Stopping...");
                break;
            case "delete_queue":
                Console.WriteLine("Print Server Queue Clearing...");
                break;
            case "start_spooler":
                Console.WriteLine("Print Server Starting...");
                break;
        }
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("action", action)
            });

                HttpResponseMessage response = await client.PostAsync("http://dental-dc/reset_print_queue.php", content);

                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null && responseContent.ToLower().Contains("success"))
                {
                    switch (action) {
                        case "stop_spooler":
                            Console.WriteLine("Print Server Stopped");
                            break;
                        case "delete_queue":
                            Console.WriteLine("Print Server Queue Cleared");
                            break;
                        case "start_spooler":
                            Console.WriteLine("Print Server Started!");
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Couldn't send command to server");
        }

    }
}

public static class Extensions
{
    public static void ForEach<T>(this T[] array, Action<T> action)
    {
        foreach (T item in array)
        {
            action(item);
        }
    }
}