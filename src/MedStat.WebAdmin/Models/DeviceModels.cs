using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Models
{
	public abstract class DeviceBasePageModel : PageModel, IPopupLayoutData
	{
		protected IDeviceRepository DvRepository { get; }

		protected IStringLocalizer<DeviceResource> DvLocalizer { get; }


		[BindProperty(SupportsGet = true)]
		public int DeviceId { get; set; }


		private protected DeviceBasePageModel(IDeviceRepository dvRepository,
			IStringLocalizer<DeviceResource> dvLocalizer)
		{
			//this.Logger = logger;
			this.DvRepository = dvRepository;
			this.DvLocalizer = dvLocalizer;
		}


		#region IPopupLayoutData

		public bool EntityWasUpdated { get; set; }

		public bool EntityWasCreated { get; set; }

		public bool EntityWasDeleted { get; set; }

		#endregion
	}
}
