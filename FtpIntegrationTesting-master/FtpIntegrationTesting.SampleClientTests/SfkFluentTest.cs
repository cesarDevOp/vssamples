using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FluentAssertions;
using FluentFTP;
using NUnit.Framework;
using SfkFtpIntegrationTesting;

namespace FtpIntegrationTesting.SampleClientTests
{
    /// <summary>
    /// Integration tests for SFKFTP server using fluent ftp framework
    /// </summary>
    [TestFixture]
    public class SfkFluentTest
    {
        private readonly DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "FtpIntegrationTesting"));
        private SfkFtpIntegrationServer server;
        private string moveFile = "toMove.txt";

        [SetUp]
        public void Init()
        {
            server = new SfkFtpIntegrationServer(dir.FullName, 2122);
            CreateSomeFiles(3);
            CreateFile(moveFile);
            Directory.CreateDirectory(Path.Combine(dir.FullName, "out"));
        }

        [TearDown]
        public void Dispose()
        {
            server.Dispose();
            dir.Delete(true);
        }

        [Test]
        public void TestGestListingFiles()
        {
            // Arrange
            var expected = GetDirectoryFiles();

            FtpClient client = new FtpClient("127.0.0.1", 2122, new NetworkCredential("anonymous", "an@nymo.us"));
            client.DataConnectionEncryption = false;
            client.Connect();

            // Act
            var res = client.GetNameListing("test");
            client.Disconnect();

            // Assert
            res.Should().BeEquivalentTo(expected); ////.Equal(String.Format("file.1{0}file.2{0}file.3{0}", Environment.NewLine));
        }

        [Test]
        public void TestMoveFile()
        {
            // Arrange
            var fileToMove = $"test\\{moveFile}";

            var previews = GetDirectoryFiles();

            FtpClient client = new FtpClient("127.0.0.1", 2122, new NetworkCredential("anonymous", "an@nymo.us"));
            client.DataConnectionEncryption = false;
            client.Connect();

            // Act
            string[] contentFile;
            using (var stream = client.OpenRead(fileToMove))
            using (var streamReader = new StreamReader(stream))
            {
                contentFile = streamReader.ReadToEnd().Split('\n');
            }

            client.MoveFile(fileToMove, $"out\\{moveFile}");
            client.Disconnect();

            var res = client.GetNameListing("test");

            // Assert
            res.Should().NotBeEquivalentTo(previews);
            contentFile[0].Should().Be($"file {moveFile} contents");
        }

        private void CreateSomeFiles(int cnt)
        {
            var testDirectory = Directory.CreateDirectory(Path.Combine(dir.FullName, "test"));
            for (var i = 1; i <= cnt; i++)
            {
                using (var fs = File.Create(Path.Combine(testDirectory.FullName, "file." + i)))
                {
                    var bytes = Encoding.UTF8.GetBytes(string.Format("file {0} contents", i));
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        private void CreateFile(string name)
        {
            var testDirectory = Directory.CreateDirectory(Path.Combine(dir.FullName, "test"));
            using (var fs = File.Create(Path.Combine(testDirectory.FullName, name)))
            {
                var bytes = Encoding.UTF8.GetBytes(string.Format("file {0} contents", name));
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        private string[] GetDirectoryFiles()
        {
            FileInfo[] Files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            var fileList = new List<string>();
            foreach (FileInfo file in Files)
            {
                fileList.Add(file.FullName);
            }

            return fileList.ToArray();
        }
    }
}
