using Emby.Notifications;
using MediaBrowser.Common;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Notifications;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Serialization;
using static System.Net.WebRequestMethods;

namespace OmbiUnofficial;

public class OmbiPlugin : BasePluginSimpleUI<OmbiPluginOptions>
{
	private readonly INotificationManager notifications;
	private readonly IUserManager users;
	private readonly IHttpClient httpClient;
	private readonly IJsonSerializer jsonSerializer;

	public OmbiPlugin(IApplicationHost host,
		INotificationManager notifications,
		IUserManager users,
		IHttpClient httpClient,
		IJsonSerializer jsonSerializer
	)
		: base(host)
	{
		Instance = this;
		this.notifications = notifications;
		this.users = users;
		this.httpClient = httpClient;
		this.jsonSerializer = jsonSerializer;
	}

	public override string Name => "Ombi";

	public override string Description => "Unofficial connector";

	public override Guid Id => Guid.Parse("b68a226e-5a88-4ff8-9a72-e443646ea121");

	public static OmbiPlugin Instance { get; private set; } = null!;

	public OmbiPluginOptions Configuration => GetOptions();

	protected override bool OnOptionsSaving(OmbiPluginOptions options)
	{
		var client = new OmbiClient(httpClient, jsonSerializer);

		string? version;
		try
		{
			var versionTask = client.GetOmbiVersion(options);
			Task.WaitAll(versionTask);
			version = versionTask.Result;

		}
		catch (Exception reason)
		{
			var msg = $"""
				Ombi version could not be determined.
				Check if this is correct: '{options.GetUrl("api/v1/Status/info")}'.
				{reason.Message}
				""";
			throw new InvalidOperationException(msg, reason);
		}


		if (!Version.TryParse(version, out _))
		{
			var msg = $"""
				Ombi version could not be determined.
				Check if this is correct: '{options.GetUrl("api/v1/Status/info")}'.
				""";
			throw new InvalidOperationException(msg);
		}

		AboutPage about;
		try
		{
			var aboutTask = client.AboutOmbi(options);
			Task.WaitAll(aboutTask);
			about = aboutTask.Result;
		}
		catch (Exception reason)
		{
			var msg = $"""
				Ombi API key could not be validated.
				{reason.Message}
				""";
			throw new InvalidOperationException(msg, reason);
		}

		if (about is null)
		{
			var msg = $"""
				Ombi API key could not be validated
				""";
			throw new InvalidOperationException(msg);
		}

		if (options.NotifyOnError)
		{
			NotifyAdmins(
				"Emby plugin for Ombi notifications are enabled",
				"You will now receive Ombi error notifications from this Emby plugin."
			);
		}
		return true;
	}

	public void NotifyAdmins(string title, string description)
	{
		var admins = users.GetUsers(new UserQuery
		{
			IsAdministrator = true,
			IsDisabled = false
		});

		foreach (var admin in admins.Items)
		{
			notifications.SendNotification(new NotificationRequest
			{
				User = admin,
				Title = title,
				Description = description
			});
		}
	}
}