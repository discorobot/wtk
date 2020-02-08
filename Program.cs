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
        ///
        ///
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("A bunch of useful writer tools");
            rootCommand.AddCommand(Init());
            rootCommand.AddCommand(Count());
            rootCommand.AddCommand(Status());
            return rootCommand.InvokeAsync(args).Result;
        }

        private static Command Init()
        {       
            var cmd = new Command("init", "Initialises a folder for use with gtk");
            cmd.Handler = CommandHandler.Create(() => {
                Console.WriteLine("this is init");
            });
            return cmd;
        }

        private static Command Count()
        {
            var cmd = new Command("count", "Counts number of words in the manuscript");
            cmd.Handler = CommandHandler.Create(() => {
                Console.WriteLine("this is count");
            });
            return cmd;
        }

        private static Command Status()
        {
            var cmd = new Command("status", "Checks the status of the project");
            cmd.Handler = CommandHandler.Create(() => {
                Console.WriteLine("this is status");
            });
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
