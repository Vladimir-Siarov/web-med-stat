using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyListModel : PageModel
	{
		private readonly ICompanyRepository _cmpRepository;


		public CompanyListModel(ILogger<CompanyListModel> logger,
			ICompanyRepository cmpRepository)
		{
			_cmpRepository = cmpRepository;
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

			return 
				new JsonResult(jsonResponse);
		}
	}
}
