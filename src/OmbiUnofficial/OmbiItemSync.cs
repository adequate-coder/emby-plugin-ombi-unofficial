using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace OmbiUnofficial;

public sealed class OmbiItemSync(
    ILogManager logManager,
    ILibraryManager libraryManager,
    IHttpClient http,
    IJsonSerializer jsonSerializer
) : IServerEntryPoint
{
    private readonly ILogger logger = logManager.GetLogger(OmbiPlugin.Instance.Name);

    public void Run()
    {
        libraryManager.ItemAdded += SyncRecentlyAdded;
    }

    public void Dispose()
    {
        libraryManager.ItemAdded -= SyncRecentlyAdded;
    }

    private async void SyncRecentlyAdded(object sender, ItemChangeEventArgs args)
    {
        var configuration = OmbiPlugin.Instance.Configuration;
        var errors = configuration.Validate();
        if (errors.HasErrors)
        {
            logger.Info("An item was added to the library, but could not sync recently added to Ombi.");
			logger.LogMultiline(errors.GetErrorMessage(), LogSeverity.Warn, new System.Text.StringBuilder());
        }
        else
        {
            var client = new OmbiClient(http, jsonSerializer);

            // Wait a minute before notifying Ombi
            // because the ItemAdded event is raised before Emby is done processing all the metadata
            await Task.Delay(TimeSpan.FromMinutes(1));

            // Hopefully Emby has completed scraping all metadata now
            // otherwise items with incomplete metadata are added to Ombi, e.g. episodes without numbers
            // and this totally breaks request processing until you do "Clear Data And Resync" in Ombi settings
            if (configuration.RecentOnly)
            {
                logger.Info($"An item was added to the library, start syncing recently added to Ombi.");
                await client.ScanRecentlyAdded(configuration).ConfigureAwait(false);
            }
            else
            {
                logger.Info($"An item was added to the library, starting syncing full library to Ombi.");
                await client.ScanLibrary(configuration).ConfigureAwait(false);
            }
        }
    }
}