using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyCreateModel : PageModel
	{
		private readonly ILogger<CompanyCreateModel> _logger;
		private readonly CompanyRepository _cmpRepository;
		private readonly IStringLocalizer<CompanyResource> _cmpLocalizer;


		[BindProperty]
		public Company Company { get; set; }


		public CompanyCreateModel(ILogger<CompanyCreateModel> logger,
			CompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
		{
			_logger = logger;
			_cmpRepository = cmpRepository;
			_cmpLocalizer = cmpLocalizer;
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
					int cmpId = await _cmpRepository.CreateCompanyAsync(this.Company.Name, this.Company.Description);

					return RedirectToPage("./Edit", new { id = cmpId, isCreated = true });
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = string.Format(_cmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			return Page();
		}
  }
}
