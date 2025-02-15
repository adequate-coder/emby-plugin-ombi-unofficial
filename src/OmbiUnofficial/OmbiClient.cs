using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;

namespace OmbiUnofficial;

public sealed class OmbiClient(IHttpClient http, IJsonSerializer jsonSerializer)
{
    public async Task ScanRecentlyAdded(OmbiPluginOptions options)
    {
        var request = new HttpRequestOptions
        {
            Url = options.GetUrl("api/v1/Job/embyrecentlyadded")
        };
        request.RequestHeaders["ApiKey"] = options.ApiKey;
        if (!string.IsNullOrWhiteSpace(options.User))
        {
            request.RequestHeaders["UserName"] = options.User;
        }
        await http.Post(request).ConfigureAwait(false);
    }

    public async Task ScanLibrary(OmbiPluginOptions options)
    {
        var request = new HttpRequestOptions
        {
            Url = options.GetUrl("api/v1/Job/embycontentcacher")
        };
        request.RequestHeaders["ApiKey"] = options.ApiKey;
        if (!string.IsNullOrWhiteSpace(options.User))
        {
            request.RequestHeaders["UserName"] = options.User;
        }
        await http.Post(request).ConfigureAwait(false);
    }

    public async Task<string> GetOmbiVersion(OmbiPluginOptions options)
    {
        var request = new HttpRequestOptions
        {
            Url = options.GetUrl("api/v1/Status/info")
        };

        using var responseStream = await http.Get(request).ConfigureAwait(false);
        return await jsonSerializer.DeserializeFromStreamAsync<string>(responseStream);
    }
    
    public async Task<AboutPage> AboutOmbi(OmbiPluginOptions options)
    {
        var request = new HttpRequestOptions
        {
            Url = options.GetUrl("api/v1/Settings/about")
        };
		request.RequestHeaders["ApiKey"] = options.ApiKey;
		if (!string.IsNullOrWhiteSpace(options.User))
		{
			request.RequestHeaders["UserName"] = options.User;
		}

		using var responseStream = await http.Get(request).ConfigureAwait(false);
        return await jsonSerializer.DeserializeFromStreamAsync<AboutPage>(responseStream);
    }
}