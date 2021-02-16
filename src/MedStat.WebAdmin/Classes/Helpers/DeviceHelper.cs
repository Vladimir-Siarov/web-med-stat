﻿using System.Text;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MedStat.Core.BE.Device;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Classes.Helpers
{
	public static class DeviceHelper
	{
		private const string InputMaskUsageFlag = "INPUT_MASK_USAGE";
		public const char MacAddressSeparator = '-';


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

		public static string FormatMacAddress(string mac)
		{
			if (string.IsNullOrEmpty(mac))
				return mac;

			StringBuilder sb = new StringBuilder();
			int i = 0;
			
			foreach (char symbol in mac)
			{
				if (i == 2)
				{
					sb.Append(MacAddressSeparator);
					i = 0;
				}

				sb.Append(symbol);
				i++;
			}

			return sb.ToString();
		}


		// Extensions:

		public static void SetInputMaskUsageFlag(this ViewDataDictionary viewData)
		{
			viewData[InputMaskUsageFlag] = true;
		}

		public static bool IsInputMaskUsage(this ViewDataDictionary viewData)
		{
			return
				viewData.ContainsKey(InputMaskUsageFlag) && (bool)viewData[InputMaskUsageFlag];
		}



		public class DeviceTypeInfo
		{
			public string Type { get; set; }
			public string IconCss { get; set; }
			public string Title { get; set; }
		}
	}
}
