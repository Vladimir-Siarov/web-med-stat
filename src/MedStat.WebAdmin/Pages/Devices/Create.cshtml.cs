using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using MedStat.Core.BE.Device;
using MedStat.Core.Interfaces;
using MedStat.Core.Resources;
using MedStat.WebAdmin.Classes.Helpers;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedStat.WebAdmin.Pages.Devices
{
	public class DeviceCreatePageModel : DeviceBasePageModel
	{
		[BindProperty]
		public Device DeviceData { get; set; }

		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Display(Name = "Type")] // "Тип"
		public EnDeviceType DeviceType { get; set; } = EnDeviceType.SmartWatch;

		[BindProperty]
		[Display(Name = "WiFi MAC address")] // "WiFi MAC адрес"
		public string WifiMac { get; set; }

		[BindProperty]
		[Display(Name = "Ethernet MAC address")] // "Ethernet MAC адрес"
		public string EthernetMac { get; set; }
		

		public SelectListItem[] DeviceTypes => new[]
		{
			new SelectListItem(DvLocalizer["__DeviceType__SmartWatch"].Value, EnDeviceType.SmartWatch.ToString()),
			new SelectListItem(DvLocalizer["__DeviceType__Gateway"].Value, EnDeviceType.Gateway.ToString())
		};

		public SelectListItem[] DeviceModels
			=> this.DvRepository.GetDeviceModelsByType(this.DeviceType)
				.Select(m => new SelectListItem(m.Name, m.Uid))
				.ToArray();


		public DeviceCreatePageModel(IDeviceRepository dvRepository,
			IStringLocalizer<DeviceResource> dvLocalizer)
			: base(dvRepository, dvLocalizer)
		{
		}


		public void OnGet()
		{
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
					int deviceId = await this.DvRepository.CreateDeviceAsync(this.DeviceData.DeviceModelUid,
						this.DeviceData.InventoryNumber, this.WifiMac, this.EthernetMac);

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
					ViewData["error_message"] = PageResourceStringLocalizerHelper<DeviceResource>
						.GetFormattedValue_ErrorHasOccurred(this.DvLocalizer, ex.Message);
				}
			}

			return Page();
		}
  }
}
