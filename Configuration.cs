using System.IO;
using System.CommandLine.Invocation;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace wtk
{
    public class Configuration {
        private const string CONFIG_FILE = "config.json";
        public static void Config(string root, bool verbose, InvocationContext context)
        {

        }
        public static void List(string root, bool verbose, InvocationContext context)
        {
            if (System.CheckInitialised(root, context))
            {         
                var configFilePath = Path.Combine(root, System.WTK_SYSTEM_DIR, CONFIG_FILE);
                var fileContents = File.ReadAllText(configFilePath);
                context.Console.Out.Write(fileContents);
            }
        }

        public static void InitialiseConfigFileIfMissing(string root)
        {
            var configFilePath = Path.Combine(root, System.WTK_SYSTEM_DIR, CONFIG_FILE);

            if (!File.Exists(configFilePath))
            {
                InitialiseConfigFile(configFilePath);
            }      
        }

        private static void InitialiseConfigFile(string path)
        {
            ConfigurationFile configurationFile = new ConfigurationFile {Publish = new Publish {OutputFormat = ".docx", PandocPath = "unknown"}} ;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var jsonString = JsonSerializer.Serialize(configurationFile, options);
            File.WriteAllText(path, jsonString);
        }
    }
}