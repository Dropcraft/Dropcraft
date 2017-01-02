using System.Collections.Generic;

namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Custom configuration root
    /// </summary>
    public interface ICustomConfiguration
    {
        /// <summary>
        /// Returns configuration root element
        /// </summary>
        /// <returns><see cref="ICustomConfigObject"/></returns>
        ICustomConfigObject Get();
    }

    /// <summary>
    /// Represents a hierarchical configuration
    /// </summary>
    public interface ICustomConfigObject
    {
        /// <summary>
        /// Returns all configurations values
        /// </summary>
        /// <returns>List of the values</returns>
        IReadOnlyCollection<ICustomConfigValue> GetChildren();

        /// <summary>
        /// Returns configuration value by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Configuration value</returns>
        ICustomConfigValue GetChild(string key);

        /// <summary>
        /// Indicates whether the object has children
        /// </summary>
        /// <returns>True if the object has children</returns>
        bool HasChildren();
    }

    /// <summary>
    /// Custom configuration value
    /// </summary>
    public interface ICustomConfigValue : ICustomConfigObject
    {
        /// <summary>
        /// Key which is used to represent this value in parent
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Configuration value
        /// </summary>
        string Value { get; }
    }
}