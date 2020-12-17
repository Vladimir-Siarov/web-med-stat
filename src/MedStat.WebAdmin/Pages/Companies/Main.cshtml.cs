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
	public class CompanyMainPageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Main;

		// Main tab data
		[BindProperty]
		public Company MainData { get; set; }

		
		public CompanyMainPageModel(ILogger<CompanyMainPageModel> logger,
			CompanyRepository cmpRepository, 
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

			this.MainData = company;
			this.CompanyName = company.Name;

			if (isCreated == true)
			{
				ViewData["success_message"] = string.Format(
					this.CmpLocalizer["Company \"{0}\" was created successfully"].Value,
					this.CompanyName);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await this.CmpRepository.UpdateCompanyMainDataAsync(this.CompanyId,
						this.MainData.Name, this.MainData.Description);

					ViewData["success_message"] = this.CmpLocalizer["Company data were updated"];
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = string.Format(this.CmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			// m.b. we have to use more lighter method for retrieve Id, Name properties
			var company = await this.CmpRepository.GetCompanyMainDataAsync(this.CompanyId);

			this.CompanyName = company.Name;

			return Page();
		}
	}
}
