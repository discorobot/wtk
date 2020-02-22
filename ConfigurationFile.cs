namespace wtk 
{
    public class ConfigurationFile 
    {
        public Compile Compile {get;set;}              
    }

    public class Compile
    {
        public string SectionBreak {get;set;}
        public string ChapterBreak {get;set;}
        public string PartBreak {get;set;}
    }
}