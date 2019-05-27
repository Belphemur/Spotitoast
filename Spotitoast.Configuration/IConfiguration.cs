namespace Spotitoast.Configuration
{
    public interface IConfiguration
    {
        /// <summary>
        /// Where is the configuration saved
        /// Any change to this will be discarded at the next loading of the configuration
        /// </summary>
        string FileLocation { get; set; }

        /// <summary>
        /// Migrate configuration to a new schema
        /// </summary>
        void Migrate();
    }
}