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
	public class DeviceCreatePageModel : DeviceManagePageModel
	{
		public DeviceCreatePageModel(IDeviceRepository dvRepository,
			IStringLocalizer<DeviceResource> dvLocalizer)
			: base(dvRepository, dvLocalizer)
		{
		}


		public void OnGet()
		{
			this.DeviceData = new DeviceViewModel();
		}

		public void OnPostChangeDeviceTypeAsync()
		{
			// Reload Device Models based on selected Device Type.

			this.ModelState.Clear();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					int deviceId = await this.DvRepository.CreateDeviceAsync(this.DeviceData.Device.DeviceModelUid,
						this.DeviceData.Device.InventoryNumber, this.DeviceData.WifiMac, this.DeviceData.EthernetMac);

					return 
						RedirectToPage("./Edit", 
							new
							{
								DeviceId = deviceId,
								isCreated = true
							});
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
