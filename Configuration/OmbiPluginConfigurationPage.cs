using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;

namespace OmbiUnofficial.Configuration;

public class OmbiPluginConfigurationPage : IPluginConfigurationPage
{
    public string Name => Plugin.Name;

    public ConfigurationPageType ConfigurationPageType => ConfigurationPageType.PluginConfiguration;

    public IPlugin Plugin => OmbiPlugin.Instance;

    public Stream GetHtmlStream()
    {
        return GetType().Assembly.GetManifestResourceStream("OmbiUnofficial.Configuration.Ombi.html");
    }
}