using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies.Staff
{
	public class CompanyStaffPageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Staff;

		
		public CompanyStaffPageModel(ILogger<CompanyStaffPageModel> logger,
			ICompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
			: base(logger, cmpRepository, cmpLocalizer, dlgLocalizer)
		{
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

		public async Task<JsonResult> OnGetEmployeeListAsync()
		{
			// TODO: ...

			throw new NotImplementedException();
		}
	}
}
