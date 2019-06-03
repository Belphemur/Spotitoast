using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Spotitoast.Configuration
{
    public abstract class BaseConfiguration : IConfiguration
    {
        [JsonIgnore]
        public string FileLocation { get; set; }

        [JsonIgnore]
        public IObservable<string> PropertyUpdated => _propertyUpdated.AsObservable();
        [JsonIgnore]
        private readonly Subject<string> _propertyUpdated = new Subject<string>();
        public abstract void Migrate();

        protected void PropertyChanged([CallerMemberName] string name = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            _propertyUpdated.OnNext(name);
        }
    }
}