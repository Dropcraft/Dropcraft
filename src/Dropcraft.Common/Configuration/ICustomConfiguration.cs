using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    public interface ICustomConfiguration
    {
        ICustomConfigObject Get();
    }

    /// <summary>
    /// Represents a hierarchical configuration
    /// </summary>
    public interface ICustomConfigObject
    {
        IEnumerable<ICustomConfigValue> GetChildren();

        ICustomConfigValue GetChild(string key);
    }

    /// <summary>
    /// 
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