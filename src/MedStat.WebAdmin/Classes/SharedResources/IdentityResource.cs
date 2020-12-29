
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Classes.SharedResources
{
	/// <summary>
	/// Define path to the shared resources for Identity BE and actions.
	/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-5.0#make-the-apps-content-localizable-2
	/// https://metanit.com/sharp/aspnet5/28.8.php 
	/// </summary>
	public class IdentityResource
	{
	}


	// Helper class
	public static class IdentityStringLocalizerExtensions
	{
		// Plain values:
		
		public static string GetValue_InvalidPhoneNumber(this IStringLocalizer<IdentityResource> localizer)
			=> localizer["Invalid phone number"].Value;


		// Formatted values:

		public static string GetFormattedValue_ErrorHasOccurred(this IStringLocalizer<IdentityResource> localizer, 
			string errorMessage)
			=> string.Format(localizer["Error has occurred: {0}"].Value, errorMessage);
	}
}
