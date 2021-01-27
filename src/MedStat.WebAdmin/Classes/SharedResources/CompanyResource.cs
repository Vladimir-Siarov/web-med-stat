
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Classes.SharedResources
{
	/// <summary>
	/// Define path to the shared resources for Companies.
	/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-5.0#make-the-apps-content-localizable-2
	/// https://metanit.com/sharp/aspnet5/28.8.php 
	/// </summary>
	public class CompanyResource
	{
	}

	// Helper class
	public static class CompanyStringLocalizerExtensions
	{
		// Plain values:

		//public static string GetValue_InvalidPhoneNumber(this IStringLocalizer<CompanyResource> localizer)
		//	=> localizer["Invalid phone number"].Value;


		// Formatted values:

		public static string GetFormattedValue_ErrorHasOccurred(this IStringLocalizer<CompanyResource> localizer,
			string errorMessage)
			=> string.Format(localizer["Error has occurred: {0}"].Value, errorMessage);


		// Company Grid:

		//public static string GetValue_CmpGridColumn_Name(this IStringLocalizer<CompanyResource> localizer)
		//	=> localizer["__CmpGridColumn__Name"].Value.Replace("__CmpGridColumn__", "");

		//public static string GetValue_CmpGridColumn_Description(this IStringLocalizer<CompanyResource> localizer)
		//	=> localizer["__CmpGridColumn__Description"].Value.Replace("__CmpGridColumn__", "");
		
		//public static string GetValue_CmpGridColumn_Users(this IStringLocalizer<CompanyResource> localizer)
		//	=> localizer["__CmpGridColumn__Users"].Value.Replace("__CmpGridColumn__", "");
	}
}
