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
		try
		{
			var client = new OmbiClient(httpClient, jsonSerializer);
			var versionTask = client.GetOmbiVersion(options);
			Task.WaitAll(versionTask);
			
			if (!Version.TryParse(versionTask.Result, out _))
			{
				throw new InvalidOperationException(
					$"Ombi version could not be determined, check if this is correct: '{options.GetUrl("api/v1/Status/info")}'.");
			}
			else
			{
				if (options.NotifyOnError)
				{
					NotifyAdmins(
						"Emby plugin for Ombi notifications are enabled",
						"You will now receive Ombi error notifications from this Emby plugin."
					);
				}
			}

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

		return true;
	}

	private void NotifyAdmins(string title, string description)
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

	protected override void OnOptionsSaved(OmbiPluginOptions options)
	{
		base.OnOptionsSaved(options);
	}
}