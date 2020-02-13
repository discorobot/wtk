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
            var cmd = new Command("count", "Counts number of words in the manuscript");
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>((root, verbose, context) => {
                context.Console.Out.WriteLine("This was written out via context");                
            });
            return cmd;
        }

        private static Command Status()
        {
            var cmd = new Command("status", "Checks the status of the project");
            cmd.Handler = CommandHandler.Create<string, bool, InvocationContext>(Commands.Status);
            return cmd;
        }


        static void WordCount(string fileMask)
        {
            var startDir = Path.GetDirectoryName(fileMask);
            var mask = Path.GetFileName(fileMask);
            Console.WriteLine($"Original = {fileMask}");
            Console.WriteLine($"StartDir = {startDir}");
            Console.WriteLine($"fileMask = {mask}");

            var di = new DirectoryInfo(startDir);
            var files = di.GetFiles(mask, SearchOption.AllDirectories);
            
            var totalWordCount = 0;
            foreach(var file in files)
            {
                int wordCount = CountWordsFromFile(file.FullName);
                Console.WriteLine($"{wordCount}\t{file.FullName}");
                totalWordCount = totalWordCount + wordCount;
            }
            Console.WriteLine($"Total word count {totalWordCount}");
        }

        static int CountWordsFromFile(string fileName)
        {
            return CountWords(File.ReadAllText(fileName));
        }
        static int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
    }
}
