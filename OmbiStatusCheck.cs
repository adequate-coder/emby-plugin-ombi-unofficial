using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;

namespace OmbiUnofficial;

public class OmbiVersion
{
    public string Version { get; set; } = "";
}

[Route("/Ombi/Version", "GET")]
public class OmbiVersionQuery : IReturn<OmbiVersion>
{
    public string Host { get; set; } = "";

    public int Port { get; set; }

    public string UrlBase { get; set; } = "";

    public bool UseHttps { get; set; }

    public string ApiKey { get; set; } = "";

    public string? User { get; set; }
}

public sealed class OmbiStatusCheck : IService
{
    private readonly ILogger logger;

    private readonly IHttpClient http;

    private readonly IJsonSerializer jsonSerializer;

    public OmbiStatusCheck(
        ILogManager logManager,
        IHttpClient http,
        IJsonSerializer jsonSerializer
        )
    {
        this.logger = logManager.GetLogger(OmbiPlugin.Instance.Name);
        this.http = http ?? throw new ArgumentNullException(nameof(http));
        this.jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
    }

    public async Task<object> Get(OmbiVersionQuery request)
    {
        var configuration = new Configuration.OmbiPluginConfiguration
        {
            Host = request.Host,
            Port = request.Port,
            UrlBase = request.UrlBase,
            UseHttps = request.UseHttps,
            ApiKey = request.ApiKey,
            User = request.User
        };

        var errors = configuration.Validate();
        if (errors.Length != 0)
        {
            throw new InvalidOperationException(errors);
        }

        var client = new OmbiClient(http, jsonSerializer);
        var version = await client.GetOmbiVersion(configuration).ConfigureAwait(false);
        if (!Version.TryParse(version, out _))
        {
            throw new InvalidOperationException(
                $"Ombi version could not be determined, check if this is correct: '{configuration.GetUrl("api/v1/Status/info")}'.");
        }

        return new OmbiVersion
        {
            Version = version
        };
    }
}