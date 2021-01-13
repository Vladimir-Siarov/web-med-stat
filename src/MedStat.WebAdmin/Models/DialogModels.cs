using MedStat.WebAdmin.Classes.SharedResources;
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Models
{
	/// <summary>
	/// Define data model for "_PopupDialogPartial.cshtml" partial.
	/// </summary>
	public class PopupDialogModel
	{
		public string ControlId { get; set; }

		/// <summary>
		/// Define visual style of dialog window.
		/// </summary>
		public bool IsDangerAction { get; set; }

		/// <summary>
		/// Define default width of dialog window.
		/// </summary>
		public bool IsLarge { get; set; }


		public string ContentFrameUrl { get; set; }

		public string ContentFrameId => $"{this.ControlId}_iframe";

		public string TitleId => $"{this.ControlId}_title";


		public LocalizedStrings Localization { get; set; }


		public PopupDialogModel()
		{
		}

		public PopupDialogModel(IStringLocalizer<DialogResources> localizer)
			: this()
		{
			this.Localization = new LocalizedStrings
			{
				Loading = localizer["Loading..."].Value
			};
		}



		public class LocalizedStrings
		{
			public string Loading { get; set; }
		}
	}
}
