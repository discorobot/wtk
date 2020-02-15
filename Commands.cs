using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.CommandLine.Invocation;

namespace wtk
{
    public class Commands
    {
        protected const string WTK_SYSTEM_DIR = ".wtk";
        protected const string MANUSCRIPT_DIR = "manuscript";
        static public void Status(string root, bool verbose, InvocationContext context)
        {
            if (CheckInitialised(root, context))
            {
                context.ResultCode = (int)ExitCode.Success;
                context.Console.Out.Write("this is a wtk folder");
            }
        }

        private static bool IsInitialised(string root)
        {
            var rootDir = new DirectoryInfo(root);
            var hasWtkDir = rootDir.GetDirectories(WTK_SYSTEM_DIR, SearchOption.TopDirectoryOnly).Length == 1;
            return hasWtkDir;
        }
        private static bool CheckInitialised(string root, InvocationContext context)
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

        public static void Init(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = IsInitialised(root);
            if (isInitialised)
            {
                context.Console.Error.Write("folder already initialised");
                context.ResultCode = (int)ExitCode.CannotReinitialise;
            }
            else
            {
                var rootDir = new DirectoryInfo(root);
                rootDir.CreateSubdirectory(WTK_SYSTEM_DIR);
                rootDir.CreateSubdirectory(MANUSCRIPT_DIR);
                context.Console.Out.Write("folder initialised");
            }
        }

        public static void Count(string root, bool verbose, InvocationContext context)
        {
            // we're going to count all files under manuscript (and subdirectories)
            var manuscriptDir = new DirectoryInfo(Path.Combine(root, MANUSCRIPT_DIR));
            // only looking for markdown files that start with a number
            var allFiles = manuscriptDir.GetFiles("*.md", SearchOption.AllDirectories);
            var justManuscriptFiles = allFiles.SelectMany(f => Regex.Matches(f.Name, @"\d\d.*[.]md"));
            foreach(var file in justManuscriptFiles)
            {
                context.Console.Out.Write($"{file}\n");
            }

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

        /*static void WordCount(string fileMask)
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
        }*/

    }


}