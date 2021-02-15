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
	public class DeviceEditPageModel : DeviceManagePageModel
	{
		public string PageTitle { get; set; }


		public DeviceEditPageModel(IDeviceRepository dvRepository,
			IStringLocalizer<DeviceResource> dvLocalizer)
			: base(dvRepository, dvLocalizer)
		{
		}


		public async Task<IActionResult> OnGet(bool? isCreated)
		{
			var device = await this.DvRepository.GetDeviceAsync(this.DeviceId);
			if (device == null)
				return NotFound();

			this.DeviceData = new DeviceViewModel(device, false); // format MAC addresses on client
			this.PageTitle = $"#{device.InventoryNumber}";

			if (isCreated == true)
			{
				this.EntityWasCreated = true;

				ViewData["success_message"] = string.Format(
					this.DvLocalizer["Device #{0} was created successfully"].Value,
					this.DeviceData.Device.InventoryNumber);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostChangeDeviceTypeAsync()
		{
			// Reload Device Models based on selected Device Type.

			this.ModelState.Clear();

			var device = await this.DvRepository.GetDeviceAsync(this.DeviceId);
			this.PageTitle = $"#{device?.InventoryNumber}";

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await this.DvRepository.UpdateDeviceAsync(this.DeviceId,
						this.DeviceData.Device.DeviceModelUid, this.DeviceData.Device.InventoryNumber,
						this.DeviceData.WifiMac, this.DeviceData.EthernetMac);

					this.EntityWasUpdated = true;

					ViewData["success_message"] = this.DvLocalizer["Device data were updated"];
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = PageResourceHelper<DeviceResource>
						.GetFormattedValue_ErrorHasOccurred(this.DvLocalizer, ex.Message);
				}
			}

			var device = await this.DvRepository.GetDeviceAsync(this.DeviceId);
			this.PageTitle = $"#{device?.InventoryNumber}";

			return Page();
		}
	}
}
