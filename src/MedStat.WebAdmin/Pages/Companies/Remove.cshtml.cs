using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyRemovePageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Main; // add "Remove" value?

		
		public CompanyRemovePageModel(ILogger<CompanyRemovePageModel> logger,
			ICompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
			: base(logger, cmpRepository, cmpLocalizer, dlgLocalizer)
		{
		}


		public IActionResult OnGet()
		{
			// System doesn't support GET action for this page
			return BadRequest();
		}

		// Ajax action
		public async Task<IActionResult> OnPostAjaxAsync()
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
				await this.CmpRepository.DeleteCompanyAsync(this.CompanyId);
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
	}
}
