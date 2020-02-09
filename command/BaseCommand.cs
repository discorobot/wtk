using System.IO;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace wtk.command
{
    public abstract class BaseCommand: ICommandHandler
    {
        protected const string WTK_SYSTEM_DIR = ".wtk";
        protected bool _verbose;
        protected DirectoryInfo _root;

        public BaseCommand(DirectoryInfo root, bool verbose)
        {
            _root = root;
            _verbose = verbose;
        }

        public abstract Task<int> InvokeAsync(InvocationContext context);
    }
    
}