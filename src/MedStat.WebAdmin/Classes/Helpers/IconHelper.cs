using MedStat.Core.BE.Device;
using MedStat.WebAdmin.Classes.SharedResources;
using Microsoft.AspNetCore.Mvc.Localization;

namespace MedStat.WebAdmin.Classes.Helpers
{
	public static class IconHelper
	{
		// Comment: Use "IHtmlLocalizer", because this method usually is called from View.
		public static DeviceTypeInfo[] GetDeviceTypeInfoList(IHtmlLocalizer<DeviceResource> DvLocalizer)
		{
			return
				new[]
				{
					new DeviceTypeInfo
					{
						Type = EnDeviceType.SmartWatch.ToString(),
						IconCss = "ms-icon-devicetype-smartwatch",
						Title = DvLocalizer["__DeviceType__SmartWatch"].Value
					},
					new DeviceTypeInfo
					{
						Type = EnDeviceType.Gateway.ToString(),
						IconCss = "ms-icon-devicetype-gateway",
						Title = DvLocalizer["__DeviceType__Gateway"].Value
					}
				};
		}


		public class DeviceTypeInfo
		{
			public string Type { get; set; }
			public string IconCss { get; set; }
			public string Title { get; set; }
		}
	}
}
