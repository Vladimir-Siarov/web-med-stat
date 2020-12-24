using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

using MedStat.Core.BE.Company;
using MedStat.Core.Interfaces;
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
			/*
			var model = DataTableHelper.ParseDataTableRequest(this.Request);

			string sortByProperty;
			switch (model.SortByColumnIndex)
			{
				case 0:
					sortByProperty = nameof(CompanySearchInfo.Id);
					break;

				case 1:
					sortByProperty = nameof(CompanySearchInfo.Name);
					break;

				case 2:
					sortByProperty = nameof(CompanySearchInfo.Description);
					break;

				case 3:
					sortByProperty = nameof(CompanySearchInfo.UserCnt);
					break;

				case 4:
					sortByProperty = nameof(CompanySearchInfo.TrackedPersonCnt);
					break;

				default:
					throw new NotSupportedException(model.SortByColumnIndex.ToString());
			}

			var searchResult = await _cmpRepository.FindCompaniesAsync(model.SearchTerm,
				sortByProperty, model.IsSortByAsc,
				model.Skip, model.Take);

			var jsonResponse = new
			{
				recordsTotal = searchResult.TotalRecords,
				recordsFiltered = searchResult.TotalRecords,
				data = searchResult.Data.ToArray()
			};
			*/

			var jsonResponse = new
			{
				recordsTotal = 0,
				recordsFiltered = 0,
				data = new string[0]
			};

			return
				new JsonResult(jsonResponse);
		}
	}
}
