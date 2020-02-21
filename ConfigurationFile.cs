namespace wtk 
{
    public class ConfigurationFile 
    {
        public Publish Publish {get;set;}  
        public Compile Compile {get;set;}              
    }

    public class Publish
    {
        public string PandocPath {get;set;}
        public string OutputFormat {get;set;}
    }

    public class Compile
    {
        public string SectionBreak {get;set;}
        public string ChapterBreak {get;set;}
        public string PartBreak {get;set;}
    }
}