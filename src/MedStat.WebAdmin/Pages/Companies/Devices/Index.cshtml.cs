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
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Pages.Companies.Devices
{
	public class CompanyDevicesPageModel : CompanyBasePageModel
	{
		private readonly IDeviceRepository _deviceRepository;

		public override EnCompanySection Section => EnCompanySection.Devices;


		public CompanyDevicesPageModel(ILogger<CompanyDevicesPageModel> logger,
			ICompanyRepository cmpRepository,
			IDeviceRepository deviceRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
			: base(logger, cmpRepository, cmpLocalizer, dlgLocalizer)
		{
			this._deviceRepository = deviceRepository;
		}


		public async Task<IActionResult> OnGetAsync(bool? isCreated)
		{
			var company = await this.CmpRepository.GetCompanyMainDataAsync(this.CompanyId);
			if (company == null)
				return NotFound();

			this.CompanyName = company.Name;

			return Page();
		}


		// Grid Actions:

		public async Task<JsonResult> OnGetDeviceListAsync()
		{
			// TODO: ...
			throw new NotImplementedException();
		}
	}
}
