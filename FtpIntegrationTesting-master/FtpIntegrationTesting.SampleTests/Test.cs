﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace FtpIntegrationTesting.SampleTests
{
    /// <summary>
    /// Integration tests for FTPDMIN server
    /// </summary>
    public class Test : IDisposable
    {
        private readonly FtpIntegrationServer server;
        private readonly DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "FtpIntegrationTesting"));

        public Test()
        {
            server = new FtpIntegrationServer(dir.FullName, 2121);
        }

        public void Dispose()
        {
            server.Dispose();
            dir.Delete(true);
        }

        [Fact]
        public void CanListFiles()
        {
            CreateSomeFiles(3);

            var rq = BuildRequest(WebRequestMethods.Ftp.ListDirectory);
            var res = GetResponseString(rq);

            Assert.Equal(string.Format("file.1{0}file.2{0}file.3{0}", Environment.NewLine), res);
        }

        private void CreateSomeFiles(int cnt)
        {
            for (var i = 1; i <= cnt; i++)
            {
                using (var fs = File.Create(Path.Combine(dir.FullName, "file." + i)))
                {
                    var bytes = Encoding.UTF8.GetBytes(string.Format("file {0} contents", i));
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        private FtpWebRequest BuildRequest(string method)
        {
            var rq = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1:2121");
            rq.Method = method;
            rq.Credentials = new NetworkCredential("anonymous", "an@nymo.us");
            return rq;
        }

        private string GetResponseString(FtpWebRequest rq)
        {
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
