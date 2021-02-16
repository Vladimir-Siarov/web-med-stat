using System;
using System.Linq;
using System.Threading.Tasks;
using MedStat.Core.BE.Device;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes;
using MedStat.WebAdmin.Classes.Helpers;

namespace MedStat.WebAdmin.Pages.Devices
{
	public class DeviceListModel : PageModel
	{
		private readonly IDeviceRepository _deviceRepository;


		public DeviceListModel(ILogger<DeviceListModel> logger,
			IDeviceRepository deviceRepository)
		{
			_deviceRepository = deviceRepository;
		}


		public void OnGet()
		{
		}


		// Grid Actions:

		public async Task<JsonResult> OnGetDeviceListAsync()
		{
			var model = DataTableHelper.ParseDataTableRequest(this.Request);

			string sortByProperty;
			switch (model.SortByColumnIndex)
			{
				case 0:
					sortByProperty = nameof(Device.Id);
					break;

				case 1:
					sortByProperty = nameof(Device.InventoryNumber);
					break;

				case 2:
					sortByProperty = nameof(Device.Model.Name);
					break;

				case 3:
					sortByProperty = nameof(Device.Model.Type);
					break;

				default:
					throw new NotSupportedException(model.SortByColumnIndex.ToString());
			}

			var searchResult = await _deviceRepository.FindDevicesAsync(model.SearchTerm,
				sortByProperty, model.IsSortByAsc,
				model.Skip, model.Take);

			var jsonResponse = new
			{
				recordsTotal = searchResult.TotalRecords,
				recordsFiltered = searchResult.TotalRecords,
				data = searchResult.Data
					.Select(d => new
					{
						d.Id,
						d.InventoryNumber,
						WifiMac = DeviceHelper.FormatMacAddress(d.NormalizedWifiMac),
						EthernetMac = DeviceHelper.FormatMacAddress(d.NormalizedEthernetMac),
						ModelName = d.Model.Name,
						d.Model.Type
					})
					.ToArray()
			};

			return
				new JsonResult(jsonResponse);
		}
	}
}
