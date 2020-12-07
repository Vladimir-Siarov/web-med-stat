using Microsoft.AspNetCore.Mvc;
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
	public class CompanyEditModel : CompanyBasePageModel
	{
		[BindProperty]
		public Company Company { get; set; }

		[BindProperty(SupportsGet = true)]
		public EnCompanySection Section { get; set; } = EnCompanySection.Main;

		
		public CompanyEditModel(ILogger<CompanyCreateModel> logger,
			CompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(logger, cmpRepository, cmpLocalizer)
		{
		}


		public async Task<IActionResult> OnGetAsync(int id, bool? isCreated)
		{
			switch (this.Section)
			{
				case EnCompanySection.Main:
					this.Company = await this.CmpRepository.GetCompanyMainData(id);
					break;

				case EnCompanySection.Requisites:
					this.Company = await this.CmpRepository.GetCompanyRequisitesAsync(id);
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
					this.CmpLocalizer["Company \"{0}\" was created successfully"].Value,
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
							await this.CmpRepository.UpdateCompanyMainDataAsync(this.Company.Id,
								this.Company.Name, this.Company.Description);
							break;

						case EnCompanySection.Requisites:
						{
							await this.CmpRepository.UpdateCompanyRequisitesAsync(this.Company.Id,
								this.Company.MainRequisites, this.Company.BankRequisites);
						}
							break;

						default:
							throw new NotSupportedException();
					}

					ViewData["success_message"] = this.CmpLocalizer["Company data were updated"];
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = string.Format(this.CmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
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
