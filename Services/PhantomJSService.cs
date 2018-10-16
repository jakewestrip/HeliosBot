using Discord;
using HeliosBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HeliosBot.Services
{
    public interface IPhantomJSService
    {
        Task<string> SaveChart(string url);
    }

    public class PhantomJSService : IPhantomJSService
    {
        private readonly IDiscordClient _discordClient;

        public PhantomJSService(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        enum OS
        {
            LINUX,
            WINDOWS,
            OSX
        }

        private OS GetOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OS.WINDOWS;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OS.LINUX;
            }

            throw new Exception("PdfGenerator: OS Environment could not be probed, halting!");
        }

        public async Task<string> SaveChart(string url)
        {
            var PhantomRootFolder = "phantomJS";
            var currentPlatform = GetOsPlatform();
            string phantomExeToUse = (currentPlatform == OS.WINDOWS) ? "windows_phantomjs.exe" : "linux64_phantomjs.exe";
            string phantomJsAbsolutePath = Path.Combine(PhantomRootFolder, phantomExeToUse);

            ProcessStartInfo startInfo = new ProcessStartInfo(phantomJsAbsolutePath);
            startInfo.WorkingDirectory = PhantomRootFolder;
            startInfo.Arguments = $"chart.js {url}";
            startInfo.UseShellExecute = false;

            Process proc = new Process() { StartInfo = startInfo };
            proc.Start();
            proc.WaitForExit();

            return Path.Combine(PhantomRootFolder, "chart.png");
        }
    }
}
