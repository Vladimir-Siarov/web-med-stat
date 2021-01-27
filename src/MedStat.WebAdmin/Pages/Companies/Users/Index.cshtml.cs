using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies.Users
{
	public class CompanyUsersPageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Users;

		
		public CompanyUsersPageModel(ILogger<CompanyMainPageModel> logger,
			ICompanyRepository cmpRepository, 
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
			: base(logger, cmpRepository, cmpLocalizer, dlgLocalizer)
		{
		}


		public async Task<IActionResult> OnGetAsync(bool? isCreated)
		{
			var company = await this.CmpRepository.GetCompanyMainDataAsync(this.CompanyId);
			if (company == null)
				return NotFound();

			this.CompanyName = company.Name;

			return Page();
		}


		// Grid Actions:

		public async Task<JsonResult> OnGetCompanyUserListAsync()
		{
			var model = DataTableHelper.ParseDataTableRequest(this.Request);

			string sortByProperty;
			switch (model.SortByColumnIndex)
			{
				case 0:
					sortByProperty = nameof(CompanyUser.Id);
					break;

				case 1:
					sortByProperty = nameof(CompanyUser.Login.Surname);
					break;

				case 2:
					sortByProperty = nameof(CompanyUser.Login.FirstName);
					break;

				case 3:
					sortByProperty = nameof(CompanyUser.Login.Patronymic);
					break;

				case 4:
					sortByProperty = nameof(CompanyUser.Login.PhoneNumber);
					break;

				case 5:
					sortByProperty = nameof(CompanyUser.Description);
					break;

				default:
					throw new NotSupportedException(model.SortByColumnIndex.ToString());
			}

			var searchResult = await this.CmpRepository.FindCompanyUsersAsync(model.SearchTerm,
				sortByProperty, model.IsSortByAsc,
				model.Skip, model.Take);

			var jsonResponse = new
			{
				recordsTotal = searchResult.TotalRecords,
				recordsFiltered = searchResult.TotalRecords,
				data = searchResult.Data
					.Select(cu => new 
						{
							cu.Id,
							cu.Login.Surname,
							cu.Login.FirstName,
							cu.Login.Patronymic,
							cu.Login.PhoneNumber,
							cu.Description
						})
					.ToArray()
			};

			return
				new JsonResult(jsonResponse);
		}
	}
}
