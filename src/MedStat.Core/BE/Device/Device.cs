using System;
using System.ComponentModel.DataAnnotations;
using MedStat.Core.Resources;

namespace MedStat.Core.BE.Device
{
	public class Device
	{
		public int Id { get; set; }

		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[StringLength(25)]
		[Display(Name = "Inventory number")] // "Инвентарный номер"
		public string InventoryNumber { get; set; }

		public DeviceModel Model { get; set; }
		
		[StringLength(12)]
		public string NormalizedEthernetMac { get; set; }

		[StringLength(12)]
		public string NormalizedWifiMac { get; set; }

		public DateTime CreatedUtc { get; set; }


		// Navigation Properties:

		[Display(Name = "Model")] // "Модель"
		public string DeviceModelUid { get; set; } // can be use for device list loading optimization

		public int? CompanyId { get; set; }

		// TODO: public int TrackedPersonId { get; set; }
	}
}
