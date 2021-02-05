using System;
using System.ComponentModel.DataAnnotations;
using MedStat.Core.Resources;

namespace MedStat.Core.BE.Device
{
	// not managed by EF
	public class DeviceModel
	{
		[StringLength(20)]
		public string Uid { get; set; }

		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[StringLength(50)]
		[Display(Name = "Name")] // "Название"
		public string Name { get; set; }

		[Display(Name = "Type")] // "Тип"
		public EnDeviceType Type { get; set; }

		[Display(Name = "Description")] // "Описание"
		public string Description { get; set; }

		
		// TODO: Fields related to the device data:
		// - Field (Enum value) that specifies algorithm for device data parsing
		// - decimal DeltaT1 // specify default (for that model) deviation for temperature measurement
	}


	public enum EnDeviceType
	{
		SmartWatch = 0,
		Gateway = 10
	}
}
