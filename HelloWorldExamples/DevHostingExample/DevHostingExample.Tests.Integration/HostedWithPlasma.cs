using System;
using System.Linq;
using NUnit.Framework;
using Plasma.Core;

namespace DevHostingExample.Tests.Integration
{
    [TestFixture]
    public class HostedWithPlasma
    {
        private AspNetApplication _plasmaApplication;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Timer.Time("Plasma start",
                () =>
                {
                    string projectPath =
                        @"D:\Programming\My Code\C#\ProgDotNet2012\WebTesting\Completed\HelloWorldExamples\CassiniDevHostingExample\CassiniDevHostingExample";
                    Console.WriteLine(
                        "Starting server for project path '{0}' . . .",
                        projectPath);
                    _plasmaApplication = new Plasma.Core.AspNetApplication("/",
                                                                            Settings
                                                                                .
                                                                                ProjectPath);

                    // Make first request to ensure app is started
                    var client =
                        new Plasma.Http.HttpPlasmaClient(_plasmaApplication);
                    client.Get("/");
                });
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Timer.Time("Plasma close",
                       () => _plasmaApplication.Close());
        }

        [TestCase(1000)]
        public void AssertHomePageTitleRepeatedly(int repeats)
        {
            Timer.Time(
                String.Format("Plasma test {0} times", repeats),
                () =>
                {
                    foreach (int i in Enumerable.Range(0, repeats))
                    {
                        var client = new Plasma.Http.HttpPlasmaClient(_plasmaApplication);
                        var response = client.Get("/");
                        var html = response.GetBody();

                        var dom = CsQuery.CQ.Create(html);
                        StringAssert.Contains("Welcome to ASP.NET MVC!", dom.Text());
                    }
                });
        }
    }
}
