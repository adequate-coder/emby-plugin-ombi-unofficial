using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using OmbiUnofficial.Configuration;

namespace OmbiUnofficial;

public sealed class OmbiClient(IHttpClient http, IJsonSerializer jsonSerializer)
{
    public async Task ScanRecentlyAdded(OmbiPluginConfiguration configuration)
    {
        var options = new HttpRequestOptions
        {
            Url = configuration.GetUrl("api/v1/Job/embyrecentlyadded")
        };
        options.RequestHeaders["ApiKey"] = configuration.ApiKey;
        if (!string.IsNullOrWhiteSpace(configuration.User))
        {
            options.RequestHeaders["UserName"] = configuration.User;
        }
        await http.Post(options).ConfigureAwait(false);
    }

    public async Task ScanLibrary(OmbiPluginConfiguration configuration)
    {
        var options = new HttpRequestOptions
        {
            Url = configuration.GetUrl("api/v1/Job/embycontentcacher")
        };
        options.RequestHeaders["ApiKey"] = configuration.ApiKey;
        if (!string.IsNullOrWhiteSpace(configuration.User))
        {
            options.RequestHeaders["UserName"] = configuration.User;
        }
        await http.Post(options).ConfigureAwait(false);
    }

    public async Task<string> GetOmbiVersion(OmbiPluginConfiguration configuration)
    {
        var options = new HttpRequestOptions
        {
            Url = configuration.GetUrl("api/v1/Status/info")
        };

        using var responseStream = await http.Get(options).ConfigureAwait(false);
        return await jsonSerializer.DeserializeFromStreamAsync<string>(responseStream);
    }
}