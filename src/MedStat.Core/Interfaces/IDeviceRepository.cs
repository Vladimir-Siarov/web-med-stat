using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedStat.Core.Interfaces
{
	public interface IDeviceRepository : IDeviceModelsRepository
	{
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
	}
}
