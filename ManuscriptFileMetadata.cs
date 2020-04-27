using System.Collections.Generic;

namespace wtk
{
    public class PartFileMetadata {
        public string FullPath {get;set;}
        public int? Chapter {get;set;}
        public int? Part {get;set;}
        public int Words {get;set;}

        public List<string> TodoItems {get;set;}

        public override string ToString(){
            return $"part {Part} chapter {Chapter} words {Words} FullPath {FullPath}";
        }
    }

    public class ChapterMetadata 
    {
        public int? Part {get;set;} 
       public int? Chapter {get;set;}
        public int Words {get;set;}
        public int TargetWords {get;set;}
        public string Synopsis {get;set;}
        public string Path {get;set;}
        public List<string> TodoItems {get;set;}
    }
}