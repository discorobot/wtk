using System.IO;
using System.CommandLine.Invocation;

namespace wtk
{
    public class Commands
    {
        protected const string WTK_SYSTEM_DIR = ".wtk";
        static public void Status(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = IsInitialised(root);
            var message = $"is initialised {isInitialised}";
            context.Console.Out.Write(message);
            context.ResultCode = isInitialised? 0 : 1;
        }
        
        private static bool IsInitialised(string root)
        {
            var rootDir = new DirectoryInfo(root);
            var hasSystemDir = rootDir.GetDirectories(WTK_SYSTEM_DIR, SearchOption.TopDirectoryOnly).Length == 1;
            bool result = hasSystemDir;
            return result;    
        }
    }
}