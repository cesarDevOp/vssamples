using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FtpIntegrationTesting
{
    /// <summary>
    /// Class to create an FTP server on demand (using FTPDMIN)
    /// </summary>
    public class FtpIntegrationServer : IDisposable
    {
        private readonly Process ftpProcess;

        public FtpIntegrationServer(string rootDirectory, int port = 21, bool hideFtpWindow = true)
        {
            var psInfo = new ProcessStartInfo
            {
                FileName = GetFilePath("ftpdmin.exe"),
                Arguments = string.Format("-p {0} -ha 127.0.0.1 \"{1}\"", port, rootDirectory),
                //// WindowStyle = hideFtpWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                UseShellExecute = false
            };

            ftpProcess = Process.Start(psInfo);
        }

        public void Dispose()
        {
            if (!ftpProcess.HasExited)
            {
                ftpProcess.Kill();
                ftpProcess.WaitForExit();
            }
        }

        private string GetFilePath(string fileName)
        {
            string assemblyCodeBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

            if (assemblyCodeBase == null)
            {
                throw new InvalidOperationException("Unable to determina the application base path");
            }

            string path = Path.Combine(new Uri(assemblyCodeBase).LocalPath, fileName);

            return path;
        }
    }
}
