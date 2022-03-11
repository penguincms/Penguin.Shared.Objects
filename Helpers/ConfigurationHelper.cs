using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Penguin.Shared.Helpers
{
    /// <summary>
    /// Saves and loads a class to a file of a predetermined name,
    /// intended to be used for easy access to simple singleton style
    /// configurations
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Overrides the configuration serialization settings
        /// </summary>
        public static JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public static string RootDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.RelativeSearchPath ?? "");

        /// <summary>
        /// Checks if an existing configuration for the file name exists
        /// </summary>
        /// <typeparam name="T">The type to check for</typeparam>
        /// <returns>True if a file exists</returns>
        public static bool Exists<T>() where T : class, new()
        {
            return File.Exists(GetConfigurationPath<T>());
        }

        /// <summary>
        /// Just use File.Exists
        /// </summary>
        /// <returns>True if a file exists</returns>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Gets the file name for the configuration
        /// </summary>
        /// <typeparam name="T">The type to check for</typeparam>
        /// <returns>The file name for the Configuration</returns>
        public static string GetConfigurationPath<T>() where T : class, new()
        {
            return Path.Combine(RootDirectory, $"{typeof(T).FullName}.json");
        }

        /// <summary>
        /// Loads an existing configuration from the disk, or creates a default file
        /// based on a new instance if it does not
        /// </summary>
        /// <typeparam name="T">The type to init or load</typeparam>
        /// <returns>The configuration requested</returns>
        public static T Load<T>() where T : class, new()
        {
            return Load<T>(out _);
        }

        /// <summary>
        /// Loads an existing configuration from the disk, or creates a default file
        /// based on a new instance if it does not, at the requested path
        /// </summary>
        /// <typeparam name="T">The type to init or load</typeparam>
        /// <returns>The configuration requested</returns>
        public static T Load<T>(string path) where T : class, new()
        {
            return Load<T>(out _, path);
        }

        /// <summary>
        /// Loads an existing configuration from the disk, or creates a default file
        /// based on a new instance if it does not
        /// </summary>
        /// <typeparam name="T">The type to init or load</typeparam>
        /// <param name="isNew">True if the file doesn't already exist</param>
        /// <param name="path">Optional path to load the file from</param>
        /// <returns>The configuration requested</returns>
        public static T Load<T>(out bool isNew, string path = null) where T : class, new()
        {
            T toReturn;

            path = path ?? GetConfigurationPath<T>();

            if (!Exists(path))
            {
                isNew = true;
                toReturn = new T();
                Save(toReturn, path);
            }
            else
            {
                isNew = false;
                toReturn = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), SerializerSettings);
            }

            return toReturn;
        }

        /// <summary>
        /// Saves the configuration to a predetermined file
        /// </summary>
        /// <typeparam name="T">The type to save</typeparam>
        /// <param name="toSave">The configuration to save</param>
        /// <param name="path">Optional path to save the file to</param>
        /// <returns>True if a file already exists. Will always return true if saved from a load, because load creates the default object</returns>
        public static bool Save<T>(T toSave = null, string path = null) where T : class, new()
        {
            path = path ?? GetConfigurationPath<T>();

            bool exists = Exists(path);

            File.WriteAllText(path, JsonConvert.SerializeObject(toSave ?? new T(), SerializerSettings));

            return exists;
        }
    }
}