using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DevHostingExample.Tests.Integration.Lib;
using System.Net;

namespace DevHostingExample.Tests.Integration.Tests
{
    [TestFixture]
    public class HostedWithIisExpress
    {
        IisExpressWrapper _iew;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            Timer.Time(
                "IIS Express start",
                () =>
                {
                    _iew = IisExpressWrapper.Start(Settings.WebProjectPath);

                    // Make first request to ensure app is started
                    var wc = new WebClient();
                    wc.DownloadString(_iew.RootUrl + "Default.aspx");
                });
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Timer.Time(
                "IIS Express stop",
                () =>
                {
                    _iew.Dispose();
                    _iew = null;
                });
        }

        [TestCase(1000)]
        public void AssertSomeTextRepeatedly(int repeats)
        {
            Timer.Time(
                String.Format("IIS Express test {0} times", repeats),
                () =>
                {
                    foreach (int i in Enumerable.Range(0, repeats))
                    {
                        var cq = CsQuery.Server.CreateFromUrl(_iew.RootUrl);
                        var text = cq.Text();
                        Assert.That(text, Contains.Substring("Modify this template to jump-start your ASP.NET application"));
                    }
                });
        }
    }
}
