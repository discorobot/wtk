using System.IO;
using System.CommandLine.Invocation;
namespace wtk
{
    public class System 
    {
        public  const string WTK_SYSTEM_DIR = ".wtk";
        public const string WC_LOG_FILE = "wc.log";
        public const string MANUSCRIPT_DIR = "manuscript";    

        public static bool IsInitialised(string root)
        {
            var rootDir = new DirectoryInfo(root);
            var hasWtkDir = rootDir.GetDirectories(System.WTK_SYSTEM_DIR, SearchOption.TopDirectoryOnly).Length == 1;
            return hasWtkDir;
        }
        public static bool CheckInitialised(string root, InvocationContext context)
        {
            bool result = true;
            if (!IsInitialised(root))
            {
                context.Console.Error.Write(".wtk folder not found. Did you wtk init?\n");
                context.ResultCode = (int)ExitCode.NotInitialised;
                result = false;
            }
            return result;
        }
    }
}