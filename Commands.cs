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
        private const string COMPILE_OUTPUT_NAME = "manuscript.md";

        private const string CONTROL_TODO = "TODO";

        private const string CONTROL_SYNOPSIS = "SYNOPSIS";

        private const string TARGET_WORDCOUNT = "TARGET_WORDCOUNT"; 

        static public void Status(string root, bool verbose, InvocationContext context)
        {
            if (System.CheckInitialised(root, context))
            {
                // get word count by chapter
                // get last 10 lines of the keep log
                CountByChapter(root, verbose, context);
                LastEntriesFromKeepFile(root, verbose, context);
                context.ResultCode = (int)ExitCode.Success;
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
                    Chapter = grp.Key.Chapter, Words = grp.Sum(p => p.Words), 
                    Path = Path.GetDirectoryName(grp.FirstOrDefault().FullPath)};
                var sortedChapters = chapters.OrderBy(c => c.Part).ThenBy(c => c.Chapter).ToList();

                foreach (var c in sortedChapters)
                {
                    var path = c.Path + @"\synopsis.md";
                    GetSpecialValuesFromSynopsis(path, c); 
                }
                foreach(var c in sortedChapters)
                {
                    context.Console.Out.Write($"part {c.Part} chapter {c.Chapter}\t{c.Words}/{c.TargetWords} words\t{c.Synopsis}\n");
                }

                var wordcount = sortedChapters.Sum(m => m.Words);
                context.Console.Out.Write($"Total {wordcount} words\n\n");   
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

        private static void LastEntriesFromKeepFile(string root, bool verbose, InvocationContext context)
        {
            
            var fullPath = Path.Combine(root, System.WTK_SYSTEM_DIR, System.WC_LOG_FILE);
            if (File.Exists(fullPath))
            {
                var keepEntries = File.ReadAllLines(fullPath);
                var lastTen = keepEntries.Skip(Math.Max(0, keepEntries.Count() - 10));
                foreach (var s in lastTen)
                {
                    context.Console.Out.Write($"{s}\n");
                }
            }
        }
        
        public static void Compile(string root, bool verbose, InvocationContext context)
        {
            var isInitialised = System.CheckInitialised(root, context);
            if (isInitialised)
            {
                var configurationFile = Configuration.LoadConfiguration(root, context);
                var outputPath = Path.Combine(root, COMPILE_OUTPUT_NAME);
                var currentSection = 0;
                var currentChapter = 0;
                var metadata = GetAllMetadata(root);
                
                using (StreamWriter outputFile = new StreamWriter(outputPath))
                {
                    foreach (var md in metadata)
                    {
                        if (md.Part != currentSection)
                        {
                            outputFile.WriteLine(string.Format(configurationFile.Compile.SectionBreak, md.Part));
                            currentSection = md.Part.Value;
                        }
                        if (md.Chapter != currentChapter)
                        {
                            outputFile.WriteLine(string.Format(configurationFile.Compile.ChapterBreak, md.Chapter));
                            currentChapter = md.Chapter.Value;
                        }   
                        var fileContents = LoadFileContents(md.FullPath);
                        outputFile.Write(fileContents);
                        outputFile.Write(configurationFile.Compile.PartBreak);
                    }
                }
            }
        }

        public static void Todo(string root, bool vebose, InvocationContext context)
        {
             var isInitialised = System.CheckInitialised(root, context);
            if (isInitialised)
            {
                var allMetadata = GetAllTodoItems(root);
                var chapters = from p in allMetadata
                    group p by new {p.Part, p.Chapter}
                    into grp
                    select new ChapterMetadata {Part = grp.Key.Part, 
                Chapter = grp.Key.Chapter, TodoItems = grp.SelectMany(p => p.TodoItems).ToList()};
                var sortedChapters = chapters.OrderBy(c => c.Part).ThenBy(c => c.Chapter);

                foreach(var c in sortedChapters)
                {
                    if (c.TodoItems.Count > 0)
                    {
                        if(c.Part != null)
                        {
                            context.Console.Out.Write($"part {c.Part} "); 
                        }
                        if (c.Chapter != null)
                        {
                            context.Console.Out.Write($"chapter {c.Chapter}"); 
                        }
                        if (c.Part != null || c.Chapter != null)
                        {
                            context.Console.Out.Write("\n");
                        }
                        
                        foreach(var todo in c.TodoItems)
                        {
                            context.Console.Out.Write($"\t{todo}\n");
                        }
                    }
                }
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

        private static List<PartFileMetadata> GetAllTodoItems(string root)
        {
            var result = new List<PartFileMetadata>();
            // we're going to count all files under manuscript (and subdirectories)
            var manuscriptDir = new DirectoryInfo(Path.Combine(root, System.MANUSCRIPT_DIR));
            // only looking for markdown files 
            var allFiles = manuscriptDir.GetFiles("*.md", SearchOption.AllDirectories);
            foreach(FileInfo file in allFiles)
            {
                var metadata = GetPartFileTodoItems(file.FullName);  
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

        private static PartFileMetadata GetPartFileTodoItems(string pathAndFileName)
        {
            var chapter = GetChapterFromPath(pathAndFileName);
            var part = GetPartFromPath(pathAndFileName);
            var todoItems = GetTodoItemsFromFile(pathAndFileName);
            return new PartFileMetadata {
                FullPath = pathAndFileName,
                Chapter = chapter,
                Part = part,
                TodoItems = todoItems
            };
        }

        private static List<String> GetTodoItemsFromFile(string pathAndFileName)
        {
            var contents = File.ReadAllLines(pathAndFileName);
            var todoLines = contents.Where(line => line.Contains(CONTROL_TODO));
            return todoLines.ToList();
        }

        private static void GetSpecialValuesFromSynopsis(string pathAndFileName, ChapterMetadata chapter)
        {
            var result = "";
            if (File.Exists(pathAndFileName))
            {
                var contents = File.ReadAllLines(pathAndFileName);
                var synopsisLine = contents.Where(line => line.StartsWith(CONTROL_SYNOPSIS)).DefaultIfEmpty("").FirstOrDefault();
                synopsisLine = synopsisLine.Replace(CONTROL_SYNOPSIS, "");
                chapter.Synopsis = synopsisLine;
                var targetWordCount = contents.Where(line => line.StartsWith(TARGET_WORDCOUNT)).FirstOrDefault();
                int twc = 0;
                if (!string.IsNullOrWhiteSpace(targetWordCount))
                {   
                    int.TryParse(targetWordCount.Replace(TARGET_WORDCOUNT, "").Trim(), out twc);
                }
                chapter.TargetWords = twc;
            }
        }
        private static int CountWordsFromFile(string fileName)
        {
            var s = LoadFileContents(fileName);
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        private static string LoadFileContents(string fileName)
        {
            var s = File.ReadAllText(fileName);
            return s;
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