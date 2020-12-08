using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedStat.Core.Info.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes;
using MedStat.WebAdmin.Models;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyListModel : CompanyBasePageModel
	{
		public CompanyListModel(ILogger<CompanyListModel> logger,
			CompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(logger, cmpRepository, cmpLocalizer)
		{
		}


		public void OnGet()
		{
		}


		// Grid Actions:

		public async Task<JsonResult> OnGetCompanyListAsync()
		{
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
					sortByProperty = nameof(CompanySearchInfo.AccountCnt);
					break;

				case 4:
					sortByProperty = nameof(CompanySearchInfo.TrackedPersonCnt);
					break;

				default:
					throw new NotSupportedException(model.SortByColumnIndex.ToString());
			}

			var searchResult = await this.CmpRepository.FindCompaniesAsync(model.SearchTerm,
				sortByProperty, model.IsSortByAsc, 
				model.Skip, model.Take);
			
			var jsonResponse = new
			{
				recordsTotal = searchResult.TotalRecords,
				recordsFiltered = searchResult.TotalRecords,
				data = searchResult.Data.ToArray()
			};

			return 
				new JsonResult(jsonResponse);
		}
	}
}
