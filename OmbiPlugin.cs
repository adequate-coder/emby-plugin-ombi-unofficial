using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;
using OmbiUnofficial.Configuration;

namespace OmbiUnofficial;
public class OmbiPlugin : BasePlugin<OmbiPluginConfiguration>
{
    public OmbiPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public override string Name => "Ombi";

    public override string Description => "Unofficial connector";

    public override Guid Id => Guid.Parse("b68a226e-5a88-4ff8-9a72-e443646ea121");

    public static OmbiPlugin Instance { get; private set; } = null!;
}