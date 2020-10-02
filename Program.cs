using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace wtk
{
    public static class Program
    {
        /// <summary>
        /// Some useful tools for writers
        ///
        /// </summary>
        public static int Main(string[] args)
        {
            var rootOption = new Option("--root", "The location of the project folder. Defaults to current directory.")
            {
                Argument = new Argument<string>(getDefaultValue: () => Directory.GetCurrentDirectory())
            };

            var verboseOption = new Option("--verbose", "wtk tries to be silent unless you're in verbose mode.")
            {
                Argument = new Argument<bool>(getDefaultValue: () => false)
            };

            var rootCommand = new RootCommand("A bunch of useful writing tools");
            rootCommand.AddOption(rootOption);
            rootCommand.AddOption(verboseOption);
            rootCommand.AddCommand(Init());
            rootCommand.AddCommand(Count());
            rootCommand.AddCommand(Compile());
            rootCommand.AddCommand(Status());
            rootCommand.AddCommand(Todo());
            rootCommand.AddCommand(Config());
            return rootCommand.InvokeAsync(args).Result;
        }

        private static Command Init()
        {
            var cmd = new Command("init", "Initialises a folder for use with wtk")
            {
                Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Init)
            };
            return cmd;
        }

        private static Command Count()
        {
            Command byChapter = new Command("ch", "Display word counts by chapter")
            {
                Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.CountByChapter)
            };

            Command keep = new Command("keep", "Counts number of words in the manuscript and stores the result in the keep file")
            {
                Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.CountKeep)
            };

            var cmd = new Command("count", "Counts number of words in the manuscript");
            cmd.AddCommand(byChapter);
            cmd.AddCommand(keep);
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Count);
            return cmd;
        }

        private static Command Status()
        {
            var cmd = new Command("status", "Checks the status of the project")
            {
                Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Status)
            };
            return cmd;
        }

        private static Command Todo()
        {
            var cmd = new Command("todo", "Displays all TODO items in the projrct")
            {
                Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Todo)
            };
            return cmd;
        }

        private static Command Compile()
        {
            var cmd = new Command("compile", "Merge all chapters into a single document")
            {
                Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Compile)
            };
            return cmd;
        }
        private static Command Config()
        {
            Command list = new Command("list", "Displays all configuration entries")
            {
                Handler = CommandHandler.Create<string, bool, InvocationContext>(Configuration.List)
            };

            var cmd = new Command("config", "Reads and writes configuration entries");
            cmd.AddCommand(list);
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>(Configuration.List);
            return cmd;
        }
    }
}
