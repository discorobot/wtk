using System;

namespace wtk
{
    public class StatusItem
    {
        public DateTime LogDate {get;set;}
        public int WordCount {get;set;}

        public static StatusItem Parse(string logEntry)
        {
            if (string.IsNullOrEmpty(logEntry))
                return Empty;
            var parts = logEntry.Split('\t');
            return new StatusItem { LogDate = DateTime.Parse(parts[0]), WordCount = int.Parse(parts[1]) };
        }

        private static readonly StatusItem Empty = new StatusItem {
            WordCount = 0,
            LogDate = DateTime.MaxValue
        };
    }
}