using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Identity;
using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;
using Microsoft.AspNetCore.Http;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyEditModel : CompanyBasePageModel
	{
		// Common page data. Used for Header, navigation and etc.
		public Company Company { get; set; }

		[BindProperty(SupportsGet = true)]
		public EnCompanySection Section { get; set; } = EnCompanySection.Main;

		[BindProperty]
		public string ConfirmCommand { get; set; }

		
		// Tabs data:

		// Main tab data
		[BindProperty]
		public Company MainData { get; set; }

		// Requisites tab data
		[BindProperty]
		public CompanyRequisites Requisites { get; set; }
		
		
		public CompanyEditModel(ILogger<CompanyCreateModel> logger,
			CompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
			: base(logger, cmpRepository, cmpLocalizer, dlgLocalizer)
		{
		}


		public async Task<IActionResult> OnGetAsync(int id, bool? isCreated)
		{
			switch (this.Section)
			{
				case EnCompanySection.Main:
					{
						this.Company = await this.CmpRepository.GetCompanyMainData(id);
						if (this.Company == null)
							return NotFound();

						this.MainData = this.Company;
					}
					break;

				case EnCompanySection.Requisites:
					{
						this.Company = await this.CmpRepository.GetCompanyWithRequisitesAsync(id);
						if (this.Company == null)
							return NotFound();
						
						this.Requisites = this.Company.Requisites;
					}
					break;

				default:
					throw new NotSupportedException(this.Section.ToString());
			}

			if (isCreated == true)
			{
				ViewData["success_message"] = string.Format(
					this.CmpLocalizer["Company \"{0}\" was created successfully"].Value,
					this.Company.Name);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int id)
		{
			this.FilterModelValidationError();

			if (ModelState.IsValid)
			{
				try
				{
					switch (this.Section)
					{
						case EnCompanySection.Main:
							{
								await this.CmpRepository.UpdateCompanyMainDataAsync(id,
									this.MainData.Name, this.MainData.Description);

								ViewData["success_message"] = this.CmpLocalizer["Company data were updated"];
							}
							break;

						case EnCompanySection.Requisites:
							{
								await this.CmpRepository.UpdateCompanyRequisitesAsync(id,
									this.Requisites.MainRequisites, this.Requisites.BankRequisites);

								ViewData["success_message"] = this.CmpLocalizer["Company requisites were updated"];
							}
							break;

						default:
							throw new NotSupportedException();
					}
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = string.Format(this.CmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			// m.b. we have to use more lighter method for retrieve Id, Name properties
			this.Company = await this.CmpRepository.GetCompanyMainData(id);

			return Page();
		}


		public async Task<IActionResult> OnPostRemoveAsync(int id)
		{
			if (!User.IsInRole(UserRoles.SystemAdmin))
			{
				// don't use Forbid() for prevent redirect to another page
				return StatusCode(StatusCodes.Status403Forbidden, this.DlgLocalizer["Access denied"]);
			}

			if (this.ConfirmCommand == null
			    || !this.ConfirmCommand.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
			{
				return BadRequest(this.DlgLocalizer["Invalid confirm value"].Value);
			}

			try
			{
				await this.CmpRepository.DeleteCompanyAsync(id);
			}
			catch (Exception ex)
			{
				var errorMsg = string.Format(this.CmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				
				return 
					StatusCode(StatusCodes.Status500InternalServerError, errorMsg);
			}

			return 
				StatusCode(StatusCodes.Status200OK, this.CmpLocalizer["Company was deleted successfully"].Value);
		}


		private void FilterModelValidationError()
		{
			string[] keys;

			switch (this.Section)
			{
				case EnCompanySection.Main:
					{
						string companyPrefix = $"{nameof(this.MainData)}.";

						keys = ModelState.Keys
							.Where(k => !k.StartsWith(companyPrefix))
							.ToArray();
					}
					break;

				case EnCompanySection.Requisites:
					{
						string requisitesPrefix = $"{nameof(this.Requisites)}.";

						keys = ModelState.Keys
							.Where(k => !k.StartsWith(requisitesPrefix))
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
