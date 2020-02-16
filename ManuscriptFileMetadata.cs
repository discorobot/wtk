namespace wtk
{
    public class PartFileMetadata {
        public string FullPath {get;set;}
        public int? Chapter {get;set;}
        public int? Part {get;set;}
        public int Words {get;set;}

        public override string ToString(){
            return $"part {Part} chapter {Chapter} words {Words} FullPath {FullPath}";
        }
    }

    public class ChapterMetadata 
    {
        public int? Part {get;set;}
       public int? Chapter {get;set;}
        public int Words {get;set;}
    }
}