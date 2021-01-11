using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies.Users
{
	public class ChangePhoneNumberPageModel : CompanyUserBasePageModel
	{
		private readonly IIdentityRepository _identityRepository;


		[BindProperty]
		public ChangePhoneNumberModel ChangeNumberModel { get; set; }

		[BindProperty]
		public string CmpUserName { get; set; }
		

		public ChangePhoneNumberPageModel(ICompanyRepository cmpRepository,
			IIdentityRepository identityRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(cmpRepository, cmpLocalizer)
		{
			this._identityRepository = identityRepository;
		}


		public async Task<IActionResult> OnGet()
		{
			var cmpUserInfo = await this.CmpRepository.GetCompanyUserInfoAsync(this.CmpUserId);
			if (cmpUserInfo == null)
				return NotFound();

			this.ChangeNumberModel = new ChangePhoneNumberModel
			{
				PhoneNumber = cmpUserInfo.User.Login.PhoneNumber
			};
			
			this.CmpUserName = $"{cmpUserInfo.User.Login.Surname} {cmpUserInfo.User.Login.FirstName}";

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await this._identityRepository.ChangeUserPhoneNumberAsync(this.ChangeNumberModel.PhoneNumber,
						this.ChangeNumberModel.NewPhoneNumber);

					this.ModelState.Clear(); // clear form data for "this.ChangeNumberModel.NewPhoneNumber" field
					this.ChangeNumberModel.PhoneNumber = this.ChangeNumberModel.NewPhoneNumber;
					this.ChangeNumberModel.NewPhoneNumber = null;

					this.EntityWasUpdated = true;

					ViewData["success_message"] = this.CmpLocalizer["User data were updated"];
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
