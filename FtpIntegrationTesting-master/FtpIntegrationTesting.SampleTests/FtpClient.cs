using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FtpIntegrationTesting.SampleTests
{
    /// <summary>
    /// Ftp client sample
    /// </summary>
    public class FtpClient
    {
        public FtpClient()
        {
        }

        public string ListFiles()
        {
            var rq = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1:2121");
            rq.Method = WebRequestMethods.Ftp.ListDirectory;
            rq.Credentials = new NetworkCredential("anonymous", "an@nymo.us");

            var response = (FtpWebResponse)rq.GetResponse();
            string res;
            using (var responseStream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(responseStream))
                {
                    res = sr.ReadToEnd();
                }
            }

            return res;
        }
    }
}
