using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SfkFtpIntegrationTesting
{
    /// <summary>
    /// Class to create an FTP server on demand (using SFK194)
    /// </summary>
    public class SfkFtpIntegrationServer : IDisposable
    {
        private readonly Process ftpProcess;

        public SfkFtpIntegrationServer(string rootDirectory, int port = 21, bool hideFtpWindow = true)
        {
            var psInfo = new ProcessStartInfo
            {
                FileName = GetFilePath("sfk194.exe"),
                Arguments = string.Format("ftpserv -port={0} -rw \"{1}\"", port, rootDirectory),
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
