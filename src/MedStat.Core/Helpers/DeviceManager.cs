using System;
using System.Text.RegularExpressions;
using MedStat.Core.BE.Device;
using Microsoft.Extensions.Configuration;

namespace MedStat.Core.Helpers
{
	public static class DeviceManager
	{
		private static DeviceModel[] _deviceModels = null;


		// NOTE: Device model list should not be modified by users, because:
		//	- Model list should be the same on DEV, QA, STAGE and PROD environments (for the same system version)
		//	- New (watch) model can be added only after adding the appropriate algorithm for parsing device data 
		/// <summary>
		/// Returns array of Device Models supported by system.
		/// </summary>
		/// <returns></returns>
		public static DeviceModel[] GetDeviceModels()
		{
			if (_deviceModels == null)
			{
				IConfigurationRoot modelsConfiguration = new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("deviceModels.json")
					.Build();

				var models = modelsConfiguration.GetSection("Models").Get<DeviceModel[]>();

				_deviceModels = models ?? new DeviceModel[0];
			}

			return 
				_deviceModels;
		}


		// Helpers:
		
		public static string NormalizeMacAddress(string address)
		{
			if (address == null)
				return null;

			address = Regex.Replace(address.ToUpper(), @"[^\d|A-Z]", "");

			return address;
		}
	}
}
