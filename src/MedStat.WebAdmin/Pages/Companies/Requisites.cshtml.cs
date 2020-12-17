using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyRequisitesPageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Requisites;

		// Requisites tab data
		[BindProperty]
		public CompanyRequisites Requisites { get; set; }


		public CompanyRequisitesPageModel(ILogger<CompanyRequisitesPageModel> logger,
			CompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
			: base(logger, cmpRepository, cmpLocalizer, dlgLocalizer)
		{
		}


		public async Task<IActionResult> OnGetAsync()
		{
			var company = await this.CmpRepository.GetCompanyWithRequisitesAsync(this.CompanyId);
			if (company == null)
				return NotFound();

			this.Requisites = company.Requisites;
			this.CompanyName = company.Name;

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await this.CmpRepository.UpdateCompanyRequisitesAsync(this.CompanyId,
						this.Requisites.MainRequisites, this.Requisites.BankRequisites);

					ViewData["success_message"] = this.CmpLocalizer["Company requisites were updated"];
				}
				catch (Exception ex)
				{
					ViewData["error_message"] 
						= string.Format(this.CmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			// m.b. we have to use more lighter method for retrieve Id, Name properties
			var company = await this.CmpRepository.GetCompanyMainDataAsync(this.CompanyId);

			this.CompanyName = company.Name;

			return Page();
		}
	}
}
