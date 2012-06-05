using System;
using System.IO;
using System.Threading;
namespace DevHostingExample.Tests.Integration
{
    internal static class Settings
    {
        private readonly static System.Lazy<DirectoryInfo> _solutionPath =
            new System.Lazy<DirectoryInfo>(GetSolutionPath, LazyThreadSafetyMode.ExecutionAndPublication);

        public static string WebProjectPath
        {
            get
            {
                return SolutionPath.FullName.TrimEnd('\\') + @"\DevHostingExample.Web";
            }
        }

        public static DirectoryInfo SolutionPath
        {
            get
            {
                return _solutionPath.Value;
            }
        }

        private static DirectoryInfo GetSolutionPath()
        {
            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string codeBase = thisAssembly.CodeBase;
            const string prefixToSkip = "file:///";

            if (!codeBase.StartsWith(prefixToSkip))
            {
                throw new ApplicationException(
                    String.Format(
@"While trying to get solution path, encountered an assembly CodeBase of unexpected format.
Expected CodeBase to start with '{0}', but it did not.
The CodeBase was '{1}'.",
                        prefixToSkip,
                        codeBase));
            }

            string assemblyPath =   codeBase
                                        .Substring(prefixToSkip.Length)
                                        .Replace('/', '\\');
            var assemblyFileInfo = new System.IO.FileInfo(assemblyPath);
            var solutionDirectory =
                assemblyFileInfo
                    .Directory  // e.g. Solution\TestProj\bin\x86\Debug
                    .Parent     // e.g. Solution\TestProj\bin\x86
                    .Parent     // e.g. Solution\TestProj\bin
                    .Parent     // e.g. Solution\TestProj
                    .Parent;    // e.g. Solution
            
            return solutionDirectory;
        }
    }
}
