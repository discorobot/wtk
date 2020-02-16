using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;


namespace wtk
{
    class Program
    {
        ///
        /// Some useful tools for writers
        ///
        static int Main(string[] args)
        {
            var rootOption = new Option("--root", "The location of the project folder. Defaults to current directory.")
            {
                Argument = new Argument<string>(defaultValue: () => { return  Directory.GetCurrentDirectory().ToString();})
            };

            var verboseOption = new Option("--verbose", "wtk tries to be silent unless you're in verbose mode.")
            {
                Argument = new Argument<bool>(defaultValue: () => false)
            };

            var rootCommand = new RootCommand("A bunch of useful writing tools"){};
            rootCommand.AddOption(rootOption);
            rootCommand.AddOption(verboseOption);
            rootCommand.AddCommand(Init());
            rootCommand.AddCommand(Count());
            rootCommand.AddCommand(Status());
            rootCommand.AddCommand(Config());
            return rootCommand.InvokeAsync(args).Result;
        }

        private static Command Init()
        {       
            var cmd = new Command("init", "Initialises a folder for use with gtk");
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Init);
            return cmd;
        }

        private static Command Count()
        {
            Command byChapter = new Command ("ch", "Display word counts by chapter");
            byChapter.Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.CountByChapter);

            Command keep = new Command("keep", "Counts number of words in the manuscript and stores the result in the keep file");
            keep.Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.CountKeep);

            var cmd = new Command("count", "Counts number of words in the manuscript");
            cmd.AddCommand(byChapter);
            cmd.AddCommand(keep);
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Count);
            return cmd;
        }

        private static Command Status()
        {
            var cmd = new Command("status", "Checks the status of the project");
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Status);
            return cmd;
        }

        private static Command Config()
        {
            Command list = new Command("list", "Displays all configuration entries");
            list.Handler = CommandHandler.Create<string, bool, InvocationContext>(Configuration.List);

            var cmd = new Command("config", "Reads and writes configuration entries");
            cmd.AddCommand(list);
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>(Configuration.List);
            return cmd;
        }


        
       
    }
}
