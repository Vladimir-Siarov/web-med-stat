using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyEditModel : PageModel
	{
		private readonly ILogger<CompanyCreateModel> _logger;
		private readonly CompanyRepository _cmpRepository;
		private readonly IStringLocalizer<CompanyResource> _cmpLocalizer;


		[BindProperty]
		public Company Company { get; set; }

		[BindProperty(SupportsGet = true)]
		public EnCompanySection Section { get; set; } = EnCompanySection.Main;

		
		public CompanyEditModel(ILogger<CompanyCreateModel> logger,
			CompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer)
		{
			_logger = logger;
			_cmpRepository = cmpRepository;
			_cmpLocalizer = cmpLocalizer;
		}


		public async Task<IActionResult> OnGetAsync(int id, bool? isCreated)
		{
			switch (this.Section)
			{
				case EnCompanySection.Main:
					this.Company = await _cmpRepository.GetCompanyMainData(id);
					break;

				case EnCompanySection.Requisites:
					this.Company = await _cmpRepository.GetCompanyRequisitesAsync(id);
					break;

				default:
					throw new NotSupportedException(this.Section.ToString());
			}
			
			if (this.Company == null)
			{
				return NotFound();
			}

			if (isCreated == true)
			{
				ViewData["success_message"] = string.Format(
					_cmpLocalizer["Company \"{0}\" was created successfully"].Value,
					this.Company.Name);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			this.FilterModelValidationError();

			if (ModelState.IsValid)
			{
				try
				{
					switch (this.Section)
					{
						case EnCompanySection.Main:
							await _cmpRepository.UpdateCompanyMainDataAsync(this.Company.Id,
								this.Company.Name, this.Company.Description);
							break;

						case EnCompanySection.Requisites:
						{
							await _cmpRepository.UpdateCompanyRequisitesAsync(this.Company.Id,
								this.Company.MainRequisites, this.Company.BankRequisites);
						}
							break;

						default:
							throw new NotSupportedException();
					}

					ViewData["success_message"] = _cmpLocalizer["Company data were updated"];
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = string.Format(_cmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			return Page();
		}


		private void FilterModelValidationError()
		{
			string[] keys;

			switch (this.Section)
			{
				case EnCompanySection.Main:
					keys = ModelState.Keys.Where(k => k.Contains('.')).ToArray();
					break;

				case EnCompanySection.Requisites:
					{
						string mainRequisitesPrefix = $"{nameof(this.Company.MainRequisites)}.";
						string bankRequisitesPrefix = $"{nameof(this.Company.BankRequisites)}.";

						keys = ModelState.Keys
							.Where(k => !k.StartsWith(mainRequisitesPrefix) && !k.StartsWith(bankRequisitesPrefix))
							.ToArray();
					}
					break;

				default:
					throw new NotSupportedException(this.Section.ToString());
			}

			foreach (string key in keys)
			{
				ModelState[key].Errors.Clear();
				ModelState[key].ValidationState = ModelValidationState.Skipped;
			}
		}
  }
}
