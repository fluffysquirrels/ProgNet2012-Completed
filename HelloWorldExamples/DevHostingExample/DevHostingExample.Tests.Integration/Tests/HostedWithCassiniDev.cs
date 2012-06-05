using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using CassiniDev;

namespace DevHostingExample.Tests.Integration
{
    [TestFixture]
    public class HostedWithCassiniDev
    {
        private CassiniDevServer _server;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Timer.Time(
                "Cassini start",
                () =>
                    {
                        _server = new CassiniDevServer();
                        string projectPath = Settings.WebProjectPath;
                        Console.WriteLine("Starting server for project path '{0}' . . .", projectPath);
                        _server.StartServer(projectPath);

                        string rootUrl = _server.NormalizeUrl("/");
                        Console.WriteLine("Root URL: '{0}'", rootUrl);

                        // Make first request to ensure app is started
                        var wc = new WebClient();
                        wc.DownloadString(rootUrl);
                    });
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Timer.Time(
                "Cassini stop",
                () =>
                    {
                        _server.StopServer();
                        _server.Dispose();
                        _server = null;
                    });
        }

        [TestCase(1000)]
        public void AssertHomePageTitleRepeatedly(int repeats)
        {
            Timer.Time(
                String.Format("Cassini test {0} times", repeats),
                () =>
                    {
                        foreach (int i in Enumerable.Range(0, repeats))
                        {
                            string rootUrl = _server.NormalizeUrl("/");
                            var dom = CsQuery.Server.CreateFromUrl(rootUrl);
                            Assert.That(dom.Text(), Contains.Substring("Modify this template to jump-start your ASP.NET application"));
                        }
                    });
        }
    }
}
