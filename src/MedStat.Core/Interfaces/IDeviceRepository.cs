using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Device;

namespace MedStat.Core.Interfaces
{
	public interface IDeviceRepository : IDeviceModelsRepository
	{
		/// <summary>
		/// Returns device by specified device ID.
		/// </summary>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		Task<Device> GetDeviceAsync(int deviceId);


		/// <summary>
		/// Creates Device by specified parameters.
		/// </summary>
		/// <param name="deviceModelUid"></param>
		/// <param name="inventoryNumber"></param>
		/// <param name="wifiMac"></param>
		/// <param name="ethernetMac"></param>
		/// <returns></returns>
		Task<int> CreateDeviceAsync(string deviceModelUid,
			string inventoryNumber, string wifiMac, string ethernetMac);

		/// <summary>
		/// Update data of specified device. 
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="deviceModelUid"></param>
		/// <param name="inventoryNumber"></param>
		/// <param name="wifiMac"></param>
		/// <param name="ethernetMac"></param>
		/// <returns></returns>
		Task UpdateDeviceAsync(int deviceId,
			string deviceModelUid, string inventoryNumber, string wifiMac, string ethernetMac);
	}
}
