namespace wtk 
{
    public class ConfigurationFile 
    {
        public Publish Publish {get;set;}                
    }

    public class Publish
    {
        public string PandocPath {get;set;}
        public string OutputFormat {get;set;}
    }
}