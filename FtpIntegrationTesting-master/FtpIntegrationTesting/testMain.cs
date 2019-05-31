using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpIntegrationTesting
{
    /// <summary>
    /// Test ftp using a main
    /// </summary>
    public static class testMain
    {
        public static void Main()
        {
            var server = new FtpIntegrationServer("C:\\temp", 2121);
        }
    }
}
