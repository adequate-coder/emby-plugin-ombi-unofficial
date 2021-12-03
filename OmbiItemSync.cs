using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace OmbiUnofficial;

public sealed class OmbiItemSync : IServerEntryPoint
{
    private readonly ILogger logger;

    private readonly ILibraryManager libraryManager;

    private readonly IHttpClient http;

    private readonly IJsonSerializer jsonSerializer;

    public OmbiItemSync(
        ILogManager logManager,
        ILibraryManager libraryManager,
        IHttpClient http,
        IJsonSerializer jsonSerializer
    )
    {
        this.logger = logManager.GetLogger(OmbiPlugin.Instance.Name);
        this.libraryManager = libraryManager ?? throw new ArgumentNullException(nameof(http));
        this.http = http ?? throw new ArgumentNullException(nameof(http));
        this.jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
    }

    public void Run()
    {
        this.libraryManager.ItemAdded += SyncRecentlyAdded;
    }

    public void Dispose()
    {
        this.libraryManager.ItemAdded -= SyncRecentlyAdded;
    }

    private async void SyncRecentlyAdded(object sender, ItemChangeEventArgs args)
    {
        var configuration = OmbiPlugin.Instance.Configuration;
        var errors = configuration.Validate();
        if (errors.Length != 0)
        {
            logger.Info("An item was added to the library, but could not sync recently added to Ombi.");
            logger.LogMultiline(errors, LogSeverity.Warn, new System.Text.StringBuilder());
        }
        else
        {
            var client = new OmbiClient(http, jsonSerializer);

            // BUG: do a full sync for TV episodes because Recently Added scan skips existing TV shows
            if (args.Item is Episode)
            {
                logger.Info("A TV show episode was added to the library, starting full library sync.");
                await client.ScanLibrary(configuration).ConfigureAwait(false);
            }
            else
            {
                logger.Info("An item was added to the library, syncing recently added to Ombi.");
                await client.ScanRecentlyAdded(configuration).ConfigureAwait(false);
            }
        }
    }
}