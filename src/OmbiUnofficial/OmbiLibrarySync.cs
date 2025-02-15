using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace OmbiUnofficial;

public sealed class OmbiLibrarySync(
    ILogManager logManager,
    IHttpClient http,
    IJsonSerializer jsonSerializer
) : ILibraryPostScanTask
{
    private readonly ILogger logger = logManager.GetLogger(OmbiPlugin.Instance.Name);

    public async Task Run(IProgress<double> progress, CancellationToken cancellationToken)
    {
        var configuration = OmbiPlugin.Instance.Configuration;
        var errors = configuration.Validate();
        if (errors.Length != 0)
        {
            logger.Error("Library scan completed, but could not sync content to Ombi.");
            logger.LogMultiline(errors, LogSeverity.Warn, new System.Text.StringBuilder());
        }
        else
        {
            logger.Info("Library scan completed, notifying Ombi.");
            var client = new OmbiClient(http, jsonSerializer);
            await client.ScanLibrary(configuration).ConfigureAwait(false);
        }
    }
}