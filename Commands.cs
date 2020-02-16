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
        protected const string WTK_SYSTEM_DIR = ".wtk";
        protected const string MANUSCRIPT_DIR = "manuscript";
        private const string REGEX_MANUSCRIPT_FILE_MATCH = @"\d\d.*[.]md";
        private const string REGEX_CHAPTER_MATCH = @"((?>ch)|(?>ch_)|(?>chapter_))(?<chapter>\d\d)";
        private const string REGEX_PART_MATCH = @"((?>part)|(?>part_)|(?>section_))(?<part>\d\d)";

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

        public static void CountByChapter(string root, bool verbose, InvocationContext context)
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
        public static void Count(string root, bool verbose, InvocationContext context)
        {
            var allMetadata = GetAllMetadata(root);
            var wordcount = allMetadata.Sum(m => m.Words);
            context.Console.Out.Write($"{wordcount} words");
        }
        static List<PartFileMetadata> GetAllMetadata(string root)
        {
            var result = new List<PartFileMetadata>();
            // we're going to count all files under manuscript (and subdirectories)
            var manuscriptDir = new DirectoryInfo(Path.Combine(root, MANUSCRIPT_DIR));
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
        static PartFileMetadata GetPartFileMetadata(string pathAndFileName)
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


        static int CountWordsFromFile(string fileName)
        {
            var s = File.ReadAllText(fileName);
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
        
        static int? GetChapterFromPath(string path)
        {
            var groups = Regex.Match(path, REGEX_CHAPTER_MATCH).Groups;
            var result = groups.Values.LastOrDefault().Value;
            return ToNullableInt(result);    
        }

        static int? GetPartFromPath(string path)
        {
            var groups = Regex.Match(path, REGEX_PART_MATCH).Groups;
            var result = groups.Values.LastOrDefault().Value;
            return ToNullableInt(result);
        }

        static int? ToNullableInt(string s)
        {
            int tempNumber;
            return (int.TryParse(s.Trim(), out tempNumber) ? tempNumber as int? : null);
        }

    }


}