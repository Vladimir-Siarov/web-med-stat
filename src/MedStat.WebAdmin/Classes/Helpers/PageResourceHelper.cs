using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Classes.Helpers
{
	public class PageResourceHelper<T> where T: class // resource
	{
		// Formatted values:

		public static string GetFormattedValue_ErrorHasOccurred(IStringLocalizer<T> localizer,
			string errorMessage)
			=> string.Format(localizer["Error has occurred: {0}"].Value, errorMessage);
	}
}
