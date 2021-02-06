using System;
using System.Linq;
using MedStat.Core.BE.Device;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes;

namespace MedStat.WebAdmin.Pages.Devices.Models
{
	public class DeviceModelListModel : PageModel
	{
		private readonly IDeviceRepository _deviceRepository;


		public DeviceModelListModel(ILogger<DeviceListModel> logger,
			IDeviceRepository deviceRepository)
		{
			_deviceRepository = deviceRepository;
		}


		public void OnGet()
		{
		}


		// Grid Actions:

		public JsonResult OnGetModelListAsync()
		{
			var model = DataTableHelper.ParseDataTableRequest(this.Request);

			string sortByProperty;
			switch (model.SortByColumnIndex)
			{
				case 0:
					sortByProperty = nameof(DeviceModel.Uid);
					break;

				case 1:
					sortByProperty = nameof(DeviceModel.Name);
					break;

				case 2:
					sortByProperty = nameof(DeviceModel.Description);
					break;

				case 3:
					sortByProperty = nameof(DeviceModel.Type);
					break;

				default:
					throw new NotSupportedException(model.SortByColumnIndex.ToString());
			}

			var searchResult = _deviceRepository.FindDeviceModels(model.SearchTerm,
				sortByProperty, model.IsSortByAsc,
				model.Skip, model.Take);

			var jsonResponse = new
			{
				recordsTotal = searchResult.TotalRecords,
				recordsFiltered = searchResult.TotalRecords,
				data = searchResult.Data.ToArray()
			};

			return
				new JsonResult(jsonResponse);
		}
	}
}
