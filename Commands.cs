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
            if (isInitialised)
            {
                context.Console.Out.Write("this is a wtk folder");
            }
            else
            {
                context.Console.Error.Write("not a wtk folder");
            }
            context.ResultCode = isInitialised? 0 : 1;
        }   
        
        private static bool IsInitialised(string root)
        {
            var rootDir = new DirectoryInfo(root);
            var hasSystemDir = rootDir.GetDirectories(WTK_SYSTEM_DIR, SearchOption.TopDirectoryOnly).Length == 1;
            bool result = hasSystemDir;
            return result;    
        }

        public static void Init(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = IsInitialised(root);
            if (isInitialised)
            {
                context.Console.Error.Write("folder already initialised");
            }
            else
            {
                var rootDir = new DirectoryInfo(root);
                rootDir.CreateSubdirectory(WTK_SYSTEM_DIR);
                context.Console.Out.Write("folder iniitalised");
            }
        }
    }
}