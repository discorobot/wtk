using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.CommandLine.Invocation;

namespace wtk
{
    public class Commands
    {        
        private const string REGEX_MANUSCRIPT_FILE_MATCH = @"\d\d.*[.]md";
        private const string REGEX_CHAPTER_MATCH = @"((?>ch)|(?>ch_)|(?>chapter_))(?<chapter>\d\d)";
        private const string REGEX_PART_MATCH = @"((?>part)|(?>part_)|(?>section_))(?<part>\d\d)";

        static public void Status(string root, bool verbose, InvocationContext context)
        {
            if (System.CheckInitialised(root, context))
            {
                context.ResultCode = (int)ExitCode.Success;
                context.Console.Out.Write("this is a wtk folder");
            }
        }
      

        public static void Init(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = System.IsInitialised(root);
            if (isInitialised)
            {
                context.Console.Error.Write("folder already initialised");
                context.ResultCode = (int)ExitCode.CannotReinitialise;
            }
            else
            {
                var rootDir = new DirectoryInfo(root);
                rootDir.CreateSubdirectory(System.WTK_SYSTEM_DIR);
                rootDir.CreateSubdirectory(System.MANUSCRIPT_DIR);
                Configuration.InitialiseConfigFileIfMissing(root);
                context.Console.Out.Write("folder initialised");
            }
        }

        public static void CountByChapter(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = System.CheckInitialised(root, context);
            if (isInitialised)
            {
                var allMetadata = GetAllMetadata(root);
                var chapters = from p in allMetadata
                    group p by new {p.Part, p.Chapter}
                    into grp
                    select new ChapterMetadata {Part = grp.Key.Part, 
                Chapter = grp.Key.Chapter, Words = grp.Sum(p => p.Words)};
                var sortedChapters = chapters.OrderBy(c => c.Part).ThenBy(c => c.Chapter);

                foreach(var c in sortedChapters)
                {
                    context.Console.Out.Write($"part {c.Part} chapter {c.Chapter}\t{c.Words} words\n");
                }

                var wordcount = sortedChapters.Sum(m => m.Words);
                context.Console.Out.Write($"Total {wordcount} words");   
            }
        }
        public static void Count(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = System.CheckInitialised(root, context);
            if (isInitialised)
            {
                var wordcount = Count(root);
                context.Console.Out.Write($"{wordcount} words");
            }
        }

        public static void CountKeep(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = System.CheckInitialised(root, context);
            if (isInitialised)
            {
                var wordcount = Count(root);
                var fullPath = Path.Combine(root, System.WTK_SYSTEM_DIR, System.WC_LOG_FILE);
                var timestamp = DateTime.Now.ToString("O");
                var lineToWrite = $"{timestamp}\t{wordcount}\n";
                File.AppendAllText(fullPath, lineToWrite);
                context.Console.Out.Write($"{wordcount} words");
            }
        }

        private static int Count(string root)
        {
            var allMetadata = GetAllMetadata(root);
            var wordcount = allMetadata.Sum(m => m.Words);
            return wordcount;
        }
        private static List<PartFileMetadata> GetAllMetadata(string root)
        {
            var result = new List<PartFileMetadata>();
            // we're going to count all files under manuscript (and subdirectories)
            var manuscriptDir = new DirectoryInfo(Path.Combine(root, System.MANUSCRIPT_DIR));
            // only looking for markdown files that start with a number
            var allFiles = manuscriptDir.GetFiles("*.md", SearchOption.AllDirectories);
            
            var justManuscriptFiles = allFiles.Where(f => Regex.IsMatch(f.Name,  REGEX_MANUSCRIPT_FILE_MATCH));
            foreach(FileInfo file in justManuscriptFiles)
            {
                var metadata = GetPartFileMetadata(file.FullName);  
                result.Add(metadata);  
            }
            return result;
        }
        private static PartFileMetadata GetPartFileMetadata(string pathAndFileName)
        {
            var chapter = GetChapterFromPath(pathAndFileName);
            var part = GetPartFromPath(pathAndFileName);
            var words = CountWordsFromFile(pathAndFileName);
            return new PartFileMetadata{
                FullPath = pathAndFileName,
                Chapter = chapter,
                Part = part,
                Words = words
            };
        }


        private static int CountWordsFromFile(string fileName)
        {
            var s = File.ReadAllText(fileName);
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
        
        private static int? GetChapterFromPath(string path)
        {
            var groups = Regex.Match(path, REGEX_CHAPTER_MATCH).Groups;
            var result = groups.Values.LastOrDefault().Value;
            return ToNullableInt(result);    
        }

        private static int? GetPartFromPath(string path)
        {
            var groups = Regex.Match(path, REGEX_PART_MATCH).Groups;
            var result = groups.Values.LastOrDefault().Value;
            return ToNullableInt(result);
        }

        private static int? ToNullableInt(string s)
        {
            int tempNumber;
            return (int.TryParse(s.Trim(), out tempNumber) ? tempNumber as int? : null);
        }

    }


}