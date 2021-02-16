using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.Helpers;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Devices
{
	public class DeviceDeletePageModel : DeviceBasePageModel
	{
		[BindProperty]
		public string PageTitle { get; set; }


		public DeviceDeletePageModel(IDeviceRepository dvRepository,
			IStringLocalizer<DeviceResource> dvLocalizer)
			: base(dvRepository, dvLocalizer)
		{
		}


		public async Task<IActionResult> OnGet()
		{
			var device = await this.DvRepository.GetDeviceAsync(this.DeviceId);
			if (device == null)
				return NotFound();

			this.PageTitle = $"#{device.InventoryNumber}";

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await this.DvRepository.DeleteDeviceAsync(this.DeviceId);

					this.EntityWasDeleted = true;

					ViewData["success_message"] = this.DvLocalizer["Device was deleted successfully"];
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = PageResourceHelper<DeviceResource>
						.GetFormattedValue_ErrorHasOccurred(this.DvLocalizer, ex.Message);
				}
			}

			return Page();
		}
  }
}
