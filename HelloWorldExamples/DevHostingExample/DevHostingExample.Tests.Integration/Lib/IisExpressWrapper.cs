using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace DevHostingExample.Tests.Integration.Lib
{
    public class IisExpressWrapper : IDisposable
    {
        private Process _process;
        private readonly int _port;
        private readonly string _appPath;
        private readonly ManualResetEventSlim _iisStartedEvent = new ManualResetEventSlim(initialState: false);

        private IisExpressWrapper(
            string appPath,
            int port,
            Process process)
        {
            _appPath = appPath;
            _port = port;
            _process = process;
        }

        public static IisExpressWrapper Start(string appPath, TraceLevel traceLevel = TraceLevel.none)
        {
            int port =
                CassiniDev.CassiniNetworkUtils.GetAvailablePort(
                    8000,
                    8999,
                    System.Net.IPAddress.Loopback,
                    includeIdlePorts: false);

            if (port == 0)
            {
                throw new ApplicationException("Couldn't get available port for new IISExpress instance.");
            }

            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                Arguments = String.Format(@"/path:""{0}"" /systray:true /clr:v4.0 /trace:{1} /port:{2}", appPath, traceLevel.ToString(), port),
                FileName = @"C:\Program Files (x86)\IIS Express\iisexpress.exe",
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                //RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            var process = System.Diagnostics.Process.Start(startInfo);
            
            var iew = new IisExpressWrapper(appPath, port, process);

            process.OutputDataReceived += iew.process_OutputDataReceived;
            process.ErrorDataReceived += iew.process_ErrorDataReceived;

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            // Hack: wait a little for IIS Express to start.
            iew._iisStartedEvent.Wait(TimeSpan.FromMilliseconds(2000));

            return iew;
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            if (!e.Data.StartsWith("Request started") &&
                !e.Data.StartsWith("Request ended"))
            {
                Console.WriteLine("IIS Express stdout: {0}", e.Data);
            }

            if (e.Data.StartsWith("IIS Express is running."))
            {
                Console.WriteLine("IIS Express has started, signalling Start() to return");
                this._iisStartedEvent.Set();
            }
        }

        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            Console.WriteLine("IIS Express stderr: {0}", e.Data);
        }

        public string AppPath { get { return _appPath; } }
        public int Port { get { return _port; } }
        public string RootUrl { get { return String.Format("http://localhost:{0}/", Port); } }


        public void Dispose()
        {
            _process.CloseMainWindow();
            if (!_process.WaitForExit(2000))
            {
                // Still running. Kill it!
                _process.Kill();
            }
            _process.Dispose();

            _process = null;
        }

        public enum TraceLevel
        {
            none,
            info,
            warning,
            error,
        }
    }
}
