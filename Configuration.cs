using System.IO;
using System.CommandLine.Invocation;
using System.Text.Json;
namespace wtk
{
    public class Configuration {
        private const string CONFIG_FILE = "config.json";
        
        public static ConfigurationFile LoadConfiguration(string root, InvocationContext context)
        {
            if (System.CheckInitialised(root, context))
            {
                var configFilePath = Path.Combine(root, System.WTK_SYSTEM_DIR, CONFIG_FILE);
                var fileContents = File.ReadAllText(configFilePath);
                return (ConfigurationFile) JsonSerializer.Deserialize(fileContents, typeof(ConfigurationFile));
            }
            else
            {
                return null;
            }
        }
        public static void List(string root, bool verbose, InvocationContext context)
        {            
            if (System.CheckInitialised(root, context))
            {
                if (verbose)
                {
                    context.Console.Out.Write("Listing...\n");
                }

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
            ConfigurationFile configurationFile = new ConfigurationFile {
                Compile = new Compile {
                    SectionBreak = "# Section {0}",
                    ChapterBreak = "## Chapter {0}",
                    PartBreak = "\n\n\n\n"
                }};
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var jsonString = JsonSerializer.Serialize(configurationFile, options);
            File.WriteAllText(path, jsonString);
        }
    }
}