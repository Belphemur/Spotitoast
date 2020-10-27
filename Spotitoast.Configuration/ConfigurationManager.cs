using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.IO;
using Newtonsoft.Json;

namespace Spotitoast.Configuration
{
    public class ConfigurationManager
    {
        private readonly string _root;
        private readonly RecyclableMemoryStreamManager _streamManager = new RecyclableMemoryStreamManager();

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
        public async Task<T> LoadConfiguration<T>() where T : BaseConfiguration, new()
        {
            var filePath = GetFilePath<T>();
            T obj;
            if (!File.Exists(filePath))
            {
                obj = new T();
                await SaveConfiguration(obj);
            }
            else
            {
                await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var reader = new StreamReader(stream);
                var contents = await reader.ReadToEndAsync();

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
            obj.PropertyUpdated
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(async property => await SaveConfiguration(obj));
            return obj;
        }

        private string GetFilePath<T>() where T : BaseConfiguration, new()
        {
            var filePath = Path.Combine(_root, typeof(T).Name + ".json");
            return filePath;
        }

        /// <summary>
        /// Save the configuration in a json file.
        /// </summary>
        /// <param name="configuration">configuration object to save</param>
        public async Task SaveConfiguration<T>(T configuration) where T : BaseConfiguration, new()
        {
            using var file = File.Open(GetFilePath<T>(), FileMode.Create, FileAccess.Write);
            using var memoryStream = _streamManager.GetStream();

            using var writer = new StreamWriter(memoryStream);
            var serializer = JsonSerializer.CreateDefault();

            serializer.Serialize(writer, configuration);

            await writer.FlushAsync().ConfigureAwait(false);

            memoryStream.Seek(0, SeekOrigin.Begin);

            await memoryStream.CopyToAsync(file).ConfigureAwait(false);


            await file.FlushAsync().ConfigureAwait(false);
        }
    }
}