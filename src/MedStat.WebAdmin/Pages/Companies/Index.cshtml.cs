using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

using MedStat.Core.Repositories;
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

		public async Task<JsonResult> OnGetCompanyListAsync(string name)
		{
			var companies = (await this.CmpRepository.GetCompaniesAsync(name)).ToArray();
			var jsonResponse = new
			{
				recordsTotal = companies.Length,
				recordsFiltered = companies.Length,
				data = companies
			};

			return 
				new JsonResult(jsonResponse);
		}
	}
}
