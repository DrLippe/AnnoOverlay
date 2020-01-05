using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;

namespace AnnoOverlayUpdater
{
    public class Updater
    {

        public Updater()
        {

        }

        public bool TryCheckVersion()
        {
            try
            {
                string appPath = Path.Combine(Environment.CurrentDirectory, "AnnoOverlay.exe");
                string url = "https://drlippe.github.io/AnnoOverlay/versions";
                
                string appVersion = FileVersionInfo.GetVersionInfo(appPath).FileVersion;

                using (WebClient client = new WebClient())
                {
                    return client.DownloadString($"{url}/latest.txt") == appVersion;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void TryUpdate()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("AnnoOverlay");
                if (processes.Length > 0)
                    foreach (Process process in processes)
                        process.Kill();

                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        MainWindow.viewModel.DownloadProgress = e.ProgressPercentage;
                    };

                    string appFilePath = Path.Combine(Environment.CurrentDirectory, "AnnoOverlay.exe");

                    string url = $"https://drlippe.github.io/AnnoOverlay/versions";
                    string latestVersion = client.DownloadString($"{url}/latest.txt").Replace("\n", "");

                    string appUrl = $"https://github.com/DrLippe/AnnoOverlay/releases/download/{latestVersion}/AnnoOverlay.exe";

                    client.DownloadFile(appUrl, appFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
