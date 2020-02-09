using System.IO;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace wtk.command
{
    public class Status: BaseCommand
    {
        public Status(DirectoryInfo root, bool verbose): base(root, verbose)
        { 
        }

        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            context.ResultCode = 1;
            return context.ResultCode;
        }

        private bool IsInitialised(DirectoryInfo root)
        {
            var hasSystemDir = root.GetDirectories(WTK_SYSTEM_DIR, SearchOption.TopDirectoryOnly).Length == 0;
            bool result = hasSystemDir;
            return result;
        }
    }
}