using System.ComponentModel;
using System.Text;
using Emby.Web.GenericEdit;
using Emby.Web.GenericEdit.Validation;
using MediaBrowser.Model.Attributes;

namespace OmbiUnofficial;
public class OmbiPluginOptions : EditableOptionsBase
{
	public override string EditorTitle => OmbiPlugin.Instance.Name;

	public override string EditorDescription => """
		<h3>Enter your Ombi instance details here.</h3>
		""";

	[DisplayName("Ombi host (name or IP)")]
	[Description("E.g. <code>localhost</code>, <code>your.ombi.xyz</code> or <code>172.16.12.34</code>")]
	[Required]
	public string Host { get; set; } = "localhost";

	[DisplayName("Port")]
	[Description("Ombi port number, e.g. <code>3579</code>")]
	[Required]
	[MinValue(1)]
	[MaxValue(65535)]
	public int Port { get; set; } = 3579;

	[DisplayName("URL base")]
	[Description("For reverse proxy support, default is empty, .e.g. <code>/ombi</code>.")]
	public string UrlBase { get; set; } = "";

	[DisplayName("Use HTTPS")]
	[Description("""
		Enable HTTPS if your Ombi instance is protected by TLS.
		Typically only desirable when Emby and Ombi are on different machines.
		""")]
	public bool UseHttps { get; set; }

	[DisplayName("Ombi API key")]
	[Description("Copy the API key from your Ombi instance's general configuration.")]
	[IsPassword]
	[Required]
	public string ApiKey { get; set; } = "";

	[DisplayName("Ombi user (optional)")]
	[Description("Choose an Ombi user to impersonate when syncing content to Ombi, or leave empty to use an anonymous admin user.")]
	public string? User { get; set; }

	[DisplayName("Sync only recently added (fast sync)")]
	[Description("""
		When enabled, only recently added items are synced to Ombi. This speeds up request
		processing, but it requires that you
		configure Emby Server: settings >
		Library > Advanced > "Date added behavior for new content" > "Use date scanned into the
		library"
		""")]
	public bool RecentOnly { get; set; }

	[DisplayName("Notify admins on error")]
	[Description("""
		When enabled, and a notification mechanism for external events has been configured,
		all admins will receive a notification in case of connection problems etc.
		""")]
	public bool NotifyOnError { get; set; }

	protected override void Validate(ValidationContext context)
	{
		if (string.IsNullOrEmpty(Host))
		{
			context.AddValidationError(nameof(Host), "Ombi host is not configured.");
		}

		if (Port == 0)
		{
			context.AddValidationError(nameof(Port), "Ombi port is not configured.");
		}
		else if (Port < 1 || Port > 65535)
		{
			context.AddValidationError(nameof(Port), "Ombi port is not in allowed range.");
		}

		if (string.IsNullOrEmpty(ApiKey))
		{
			context.AddValidationError(nameof(ApiKey), "Ombi API key is not configured.");
		}
	}

	public string Validate2()
	{
		var errors = new StringBuilder();

		if (string.IsNullOrEmpty(ApiKey))
		{
			errors.AppendLine("Ombi API key is not configured.");
		}

		return errors.ToString();
	}

	public string GetUrl(string relativePath)
	{
		var urlBase = (UrlBase?.TrimEnd(['/']) ?? "") + "/";
		relativePath = relativePath?.TrimStart(['/']) ?? "";
		return new UriBuilder
		{
			Scheme = UseHttps ? "https" : "http",
			Host = Host,
			Port = Port,
			Path = urlBase + relativePath
		}.ToString();
	}
}
