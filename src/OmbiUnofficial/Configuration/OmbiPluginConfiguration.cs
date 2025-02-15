using System.Text;
using MediaBrowser.Model.Plugins;

namespace OmbiUnofficial.Configuration;
public class OmbiPluginConfiguration : BasePluginConfiguration
{
    public string Host { get; set; } = "";

    public int Port { get; set; }

    public string UrlBase { get; set; } = "";

    public bool UseHttps { get; set; }

    public string ApiKey { get; set; } = "";

    public string? User { get; set; }

    public bool RecentOnly { get; set; }

    public string Validate()
    {
        var errors = new StringBuilder();
        if (string.IsNullOrEmpty(Host))
        {
            errors.AppendLine("Ombi host is not configured.");
        }

        if (Port == 0)
        {
            errors.AppendLine("Ombi port is not configured.");
        }
        else if (Port < 1 || Port > 65535)
        {
            errors.AppendLine("Ombi port is not in allowed range.");
        }

        if (string.IsNullOrEmpty(ApiKey))
        {
            errors.AppendLine("Ombi API key is not configured.");
        }

        return errors.ToString();
    }

    public string GetUrl(string relativePath)
    {
        var urlBase = (UrlBase?.TrimEnd(new[] { '/' }) ?? "") + "/";
        relativePath = relativePath?.TrimStart(new[] { '/' }) ?? "";
        return new UriBuilder
        {
            Scheme = UseHttps ? "https" : "http",
            Host = Host,
            Port = Port,
            Path = urlBase + relativePath
        }.ToString();
    }
}
