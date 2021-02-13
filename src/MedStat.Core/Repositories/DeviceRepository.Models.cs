using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Device;
using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using MedStat.Core.Info;
using MedStat.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public partial class DeviceRepository
	{
		protected IQueryable<DeviceModel> DeviceModels => DeviceManager.GetDeviceModels().AsQueryable();


		#region Get

		public IEnumerable<DeviceModel> GetDeviceModelsByType(EnDeviceType type)
		{
			var models = this.DeviceModels.Where(m => m.Type == type).ToArray();

			return models;
		}

		#endregion


		#region Search

		public SearchResult<DeviceModel> FindDeviceModels(string name,
			string sortByProperty, bool isSortByAsc,
			int skip, int take)
		{
			var q = this.DeviceModels.Select(m => m);

			if (!string.IsNullOrEmpty(name))
			{
				q = q.Where(m => m.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
			}

			var result = new SearchResult<DeviceModel>();

			result.TotalRecords = q.Count(); // q.CountAsync();

			#region Sorting

			if (!string.IsNullOrEmpty(sortByProperty))
			{
				switch (sortByProperty)
				{
					case nameof(DeviceModel.Uid):
						q = isSortByAsc ? q.OrderBy(m => m.Uid) : q.OrderByDescending(m => m.Uid);
						break;

					case nameof(DeviceModel.Name):
						q = isSortByAsc ? q.OrderBy(m => m.Name) : q.OrderByDescending(m => m.Name);
						break;

					case nameof(DeviceModel.Description):
						q = isSortByAsc ? q.OrderBy(m => m.Description) : q.OrderByDescending(m => m.Description);
						break;

					case nameof(DeviceModel.Type):
						q = isSortByAsc ? q.OrderBy(m => m.Type) : q.OrderByDescending(m => m.Type);
						break;

					default:
						throw new NotSupportedException(sortByProperty);
				}
			}

			#endregion

			var models = q.Skip(skip).Take(take).AsNoTracking().ToArray(); // .ToArrayAsync();

			result.Data = models;

			return result;
		}

		#endregion
	}
}
