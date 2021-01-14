using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies.Users
{
	public class CompanyUserCreatePageModel : CompanyUserBasePageModel
	{
		[BindProperty(SupportsGet = true)]
		public int CompanyId { get; set; }

		[BindProperty]
		public CompanyUserInfo CmpUserData { get; set; }


		public CompanyUserCreatePageModel(ICompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(cmpRepository, cmpLocalizer)
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
					int cmpUserId = await this.CmpRepository.CreateCompanyUserAsync(this.CompanyId,
						this.CmpUserData.User.Description,
						this.CmpUserData.User.Login,
						this.CmpUserData.IsPowerUser);
					
					return 
						RedirectToPage("./Edit", 
							new
							{
								CmpUserId = cmpUserId,
								isCreated = true
							});
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = this.CmpLocalizer.GetFormattedValue_ErrorHasOccurred(ex.Message);
				}
			}

			return Page();
		}
  }
}
