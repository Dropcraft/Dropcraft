namespace Dropcraft.Contracts.Configuration
{
    public interface IPackageConfigurationParser
    {
        IParsedPackageConfiguration Parse(PackageInfo packageInfo);
    }
}