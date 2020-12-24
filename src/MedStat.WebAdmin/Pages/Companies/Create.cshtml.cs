using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyCreatePageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Main;

		// Main tab data
		[BindProperty]
		public Company MainData { get; set; }


		public CompanyCreatePageModel(ILogger<CompanyCreatePageModel> logger,
			ICompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(logger, cmpRepository, cmpLocalizer)
		{
		}


		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					int cmpId = await this.CmpRepository.CreateCompanyAsync(this.MainData.Name, this.MainData.Description);

					return RedirectToPage("./Main", new { CompanyId = cmpId, isCreated = true });
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = string.Format(this.CmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			return Page();
		}
  }
}
