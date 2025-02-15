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
		if (errors.HasErrors)
		{
			logger.Error("Ombi plugin configuration is invalid");
			logger.LogMultiline(errors.GetErrorMessage(), LogSeverity.Warn, new System.Text.StringBuilder());
			OmbiPlugin.Instance.NotifyAdmins("Ombi plugin configuration is invalid", errors.GetErrorMessage());
		}
		else
		{
			try
			{
				logger.Info("Library scan completed, notifying Ombi.");
				var client = new OmbiClient(http, jsonSerializer);
				await client.ScanLibrary(configuration).ConfigureAwait(false);
			}
			catch (Exception reason)
			{
				logger.ErrorException("Failed to notify Ombi after library scan", reason);
				OmbiPlugin.Instance.NotifyAdmins("Ombi plugin failed to notify Ombi after library scan", reason.Message);
			}
		}
	}
}