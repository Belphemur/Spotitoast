using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Spotitoast.Configuration
{
    public class ConfigurationManager
    {
        private readonly string _root;

        public ConfigurationManager(string root)
        {
            _root = root;
            if (Directory.Exists(_root))
                return;

            Directory.CreateDirectory(_root);
        }

        /// <summary>
        /// Load a ConfigurationManager from its file
        /// </summary>
        /// <returns>The loaded configuration if the file exists, else a new instance of the configuration</returns>
        public T LoadConfiguration<T>() where T : IConfiguration, new()
        {
            var filePath = GetFilePath<T>();
            T obj;
            if (!File.Exists(filePath))
            {
                obj = new T();
                SaveConfiguration(obj);
            }
            else
            {
                var contents = File.ReadAllText(filePath);
                obj = JsonConvert.DeserializeObject<T>(contents);
                if (obj == null)
                {
                    Trace.WriteLine("Problem with deserialization");
                    Trace.WriteLine("Contents: " + contents);
                    obj = new T();
                }
            }
            obj.FileLocation = filePath;
            obj.Migrate();
            return obj;
        }

        private string GetFilePath<T>() where T : IConfiguration, new()
        {
            var filePath = Path.Combine(_root, typeof(T).Name + ".json");
            return filePath;
        }

        /// <summary>
        /// Save the configuration in a json file.
        /// </summary>
        /// <param name="configuration">configuration object to save</param>
        public void SaveConfiguration<T>(T configuration) where T : IConfiguration, new()
        {
            configuration.FileLocation = null;
            var serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
            using (var writer = new JsonTextWriter(new StreamWriter(GetFilePath<T>())))
            {
                serializer.Serialize(writer, configuration);
            }
        }
    }
}