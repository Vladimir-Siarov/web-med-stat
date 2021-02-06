using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Device;
using MedStat.Core.Info;

namespace MedStat.Core.Interfaces
{
	public interface IDeviceModelsRepository
	{
		/// <summary>
		/// Finds device models and sorts results by specified params.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="sortByProperty"></param>
		/// <param name="isSortByAsc"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		SearchResult<DeviceModel> FindDeviceModels(string name,
			string sortByProperty, bool isSortByAsc,
			int skip, int take);
	}
}
