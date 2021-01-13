using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using MedStat.Core.Resources;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MedStat.WebAdmin.Pages.Companies
{
	[Authorize(Roles = UserRoles.SystemAdmin)]
	public class CompanyDeletePageModel : PageModel, IPopupLayoutData
	{
		private readonly ICompanyRepository _cmpRepository;
		private readonly IStringLocalizer<CompanyResource> _cmpLocalizer;


		[BindProperty(SupportsGet = true)]
		public int CompanyId { get; set; }

		[BindProperty]
		public string CompanyName { get; set; }

		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Display(Name = "Confirm command")] // "Команда подтверждения"
		public string ConfirmCommand { get; set; }


		#region IPopupLayoutData

		public bool EntityWasUpdated { get; set; }

		public bool EntityWasCreated { get; set; }

		public bool EntityWasDeleted { get; set; }

		#endregion


		public CompanyDeletePageModel(ICompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer)
		{
			this._cmpRepository = cmpRepository;
			this._cmpLocalizer = cmpLocalizer;
		}


		public async Task<IActionResult> OnGet()
		{
			var company = await this._cmpRepository.GetCompanyMainDataAsync(this.CompanyId);
			if (company == null)
				return NotFound();

			this.CompanyName = company.Name;

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				if (this.ConfirmCommand == null
				    || !this.ConfirmCommand.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewData["error_message"] = this._cmpLocalizer["Invalid confirm value"].Value;
				}
				else
				{
					try
					{
						await this._cmpRepository.DeleteCompanyAsync(this.CompanyId);

						this.EntityWasDeleted = true;

						ViewData["success_message"] = this._cmpLocalizer["Company was deleted successfully"].Value;
					}
					catch (Exception ex)
					{
						ViewData["error_message"] = this._cmpLocalizer.GetFormattedValue_ErrorHasOccurred(ex.Message);
					}
				}
			}

			return Page();
		}
	}
}
