using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MedStat.Core.BE.Device;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using MedStat.Core.Interfaces;
using MedStat.Core.Resources;
using MedStat.WebAdmin.Classes.SharedResources;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedStat.WebAdmin.Models
{
	public class DeviceViewModel
	{
		public Device Device { get; set; }

		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Display(Name = "Type")] // "Тип"
		public EnDeviceType DeviceType { get; set; } = EnDeviceType.SmartWatch;

		[Display(Name = "WiFi MAC address")] // "WiFi MAC адрес"
		public string WifiMac { get; set; }

		[Display(Name = "Ethernet MAC address")] // "Ethernet MAC адрес"
		public string EthernetMac { get; set; }


		// parameters less constructor for ASP.NET
		public DeviceViewModel()
		{
		}

		public DeviceViewModel(Device device, bool formatMacAddresses = true)
		{
			this.Device = device;
			
			this.DeviceType = device.Model.Type;
			this.WifiMac = device.NormalizedWifiMac;
			this.EthernetMac = device.NormalizedEthernetMac;

			if (formatMacAddresses)
			{
				throw new NotImplementedException();
			}
		}
	}


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

	public abstract class DeviceManagePageModel : DeviceBasePageModel
	{
		[BindProperty]
		public DeviceViewModel DeviceData { get; set; }


		public SelectListItem[] DeviceTypes => new[]
		{
			new SelectListItem(DvLocalizer["__DeviceType__SmartWatch"].Value, EnDeviceType.SmartWatch.ToString()),
			new SelectListItem(DvLocalizer["__DeviceType__Gateway"].Value, EnDeviceType.Gateway.ToString())
		};

		public SelectListItem[] DeviceModels
			=> this.DvRepository.GetDeviceModelsByType(this.DeviceData.DeviceType)
				.Select(m => new SelectListItem(m.Name, m.Uid))
				.ToArray();


		private protected DeviceManagePageModel(IDeviceRepository dvRepository,
			IStringLocalizer<DeviceResource> dvLocalizer)
			:base(dvRepository, dvLocalizer)
		{
		}
	}
}
