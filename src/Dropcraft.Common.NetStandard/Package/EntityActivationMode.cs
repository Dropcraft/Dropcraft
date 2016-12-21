namespace Dropcraft.Common.Package
{
    /// <summary>
    /// EntityActivationMode allows to control package and package's entities activation mode
    /// </summary>
    public enum EntityActivationMode
    {
        /// <summary>
        /// Entity will be activated immediatelly 
        /// </summary>
        Immediate,

        /// <summary>
        /// Entity will be activated asynchroniously, after all Immediate entities are activated
        /// </summary>
        Deferred,
    }
}