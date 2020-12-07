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
	public class CompanyCreateModel : CompanyBasePageModel
	{
		[BindProperty]
		public Company Company { get; set; }


		public CompanyCreateModel(ILogger<CompanyCreateModel> logger,
			CompanyRepository cmpRepository,
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
					int cmpId = await this.CmpRepository.CreateCompanyAsync(this.Company.Name, this.Company.Description);

					return RedirectToPage("./Edit", new { id = cmpId, isCreated = true });
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
